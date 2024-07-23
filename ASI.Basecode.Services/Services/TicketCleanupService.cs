using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Hosting;
using ASI.Basecode.Resources.Constants;

namespace ASI.Basecode.Services.Services
{
    public class TicketCleanupService : ITicketCleanupService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketActivityRepository _ticketActivityRepository;
        private readonly INotificationRepository _notificationRepository;
        public TicketCleanupService(ITicketRepository ticketRepository, ITicketActivityRepository ticketActivityRepository, INotificationRepository notificationRepository)
        {
            _ticketRepository = ticketRepository;
            _ticketActivityRepository = ticketActivityRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task CleanupTicketsAsync()
        {
            var oneWeekAgo = DateTime.Now.AddDays(-7); // Calculate date threshold

            // Retrieve unique Ticket IDs for tickets that are resolved and outdated
            var ticketIds = await _ticketActivityRepository.RetrieveAll()
                .Where(ta => ta.OperationId == 7 && ta.ModifiedAt < oneWeekAgo)
                .Select(ta => ta.TicketId)
                .Distinct()
                .ToListAsync();

            if (!ticketIds.Any())
            {
                Console.WriteLine("No tickets to process.");
                return;
            }

            // Retrieve tickets that need to be updated based on the ticket IDs
            var ticketsToClose = await _ticketRepository.RetrieveAll()
                .Where(t => ticketIds.Contains(t.TicketId) && t.StatusId == 4)
                .ToListAsync();

            if (!ticketsToClose.Any())
            {
                Console.WriteLine("No tickets to close.");
                return;
            }

            // Update ticket statuses
            foreach (var ticket in ticketsToClose)
            {
                ticket.StatusId = 5;
                ticket.DateClosed = DateTime.Now;
            }

            await _ticketRepository.UpdateTicketsAsync(ticketsToClose);

            // Create and add new TicketActivity entries
            var ticketActivities = ticketsToClose.Select(ticket => new TicketActivity
            {
                HistoryId = Guid.NewGuid(),
                TicketId = ticket.TicketId,
                OperationId = 5,
                ModifiedAt = DateTime.Now,
                ModifiedBy = Const.SystemId, // System ID for automated changes
                Message = "Ticket closed automatically by the system."
            }).ToList();

            await _ticketActivityRepository.AddTicketActivitiesAsync(ticketActivities);

            // Generate notifications for closed tickets
            var notifications = ticketsToClose.Select(ticket => new Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = $"Ticket {ticket.TicketNumber} was automatically closed",
                RecipientId = ticket.CreatedBy,
                TicketId = ticket.TicketId,
                Body = $"Your ticket '{ticket.Title}' has been automatically closed because it has been 7 days since it was resolved."
            }).ToList();

            await _notificationRepository.AddNotificationsAsync(notifications);

            Console.WriteLine($"Closed {ticketsToClose.Count} tickets.");
        }
    }
}
