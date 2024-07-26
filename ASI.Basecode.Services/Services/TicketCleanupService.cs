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
using System.Collections.Generic;

namespace ASI.Basecode.Services.Services
{
    public class TicketCleanupService : ITicketCleanupService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketActivityRepository _ticketActivityRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        public TicketCleanupService(ITicketRepository ticketRepository,
                                    ITicketActivityRepository ticketActivityRepository,
                                    INotificationRepository notificationRepository,
                                    IUserRepository userRepository)
        {
            _ticketRepository = ticketRepository;
            _ticketActivityRepository = ticketActivityRepository;
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
        }

        public async Task CleanupTicketsAsync()
        {
            var oneWeekAgo = DateTime.Now.AddDays(-10); // Calculate date threshold

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
                Body = $"Your ticket '{ticket.Title}' has been automatically closed because it has been 10 days since it was resolved."
            }).ToList();

            await _notificationRepository.AddNotificationsAsync(notifications);
        }

        public async Task NotifyAgentsonIdleTicketsAsync()
        {
            var oneWeekAgo = DateTime.Now.AddDays(-7); // Calculate date threshold

            // Retrieve unique Ticket IDs for tickets that have activities in the last 7 days
            var ticketIds = await _ticketActivityRepository.RetrieveAll()
                .Where(ta => ta.ModifiedAt > oneWeekAgo)
                .Select(ta => ta.TicketId)
                .Distinct()
                .ToListAsync();

            var tickets = _ticketRepository.RetrieveAll()
                .Where(t => ticketIds.Contains(t.TicketId))
                .ToList();


            var admins = _userRepository.GetUsers().Where(u => u.RoleId == 1).ToList();



            // Define the list to store notifications
            var notifications = new List<Notification>();

            // Iterate through the tickets to close
            foreach (var ticket in tickets)
            {
                if (ticket.AssignedAgent.HasValue)
                {
                    // Create the notification for the idle ticket for the assigned agent
                    var notification = new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = $"Ticket {ticket.TicketNumber} has been idle",
                        RecipientId = ticket.AssignedAgent.Value,  // Use the assigned agent's ID
                        TicketId = ticket.TicketId,
                        Body = $"Ticket '{ticket.Title}' has been idle for 7 days and is now being closed. Please review or reassign as needed."
                    };

                    // Add the notification to the list
                    notifications.Add(notification);
                }
            }


            // Add notifications to the repository
            await _notificationRepository.AddNotificationsAsync(notifications);
        }
    }
}
