using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Hosting;

namespace ASI.Basecode.Services.Services
{
    public class TicketCleanupService : ITicketCleanupService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketActivityRepository _ticketActivityRepository;
        public TicketCleanupService(ITicketRepository ticketRepository, ITicketActivityRepository ticketActivityRepository)
        {
            _ticketRepository = ticketRepository;
            _ticketActivityRepository = ticketActivityRepository;
        }

        public async Task CleanupTicketsAsync()
        {
            var oneWeekAgo = DateTime.Now.AddDays(-7); // Simplified date calculation

            // Retrieve all TicketActivities that have the "Resolved" operation and were modified more than a week ago
            var ticketIds = await _ticketActivityRepository.RetrieveAll()
                .Where(ta => ta.OperationId == 7 && ta.ModifiedAt < oneWeekAgo)
                .Select(ta => ta.TicketId)
                .Distinct()
                .ToListAsync(); // Fetch only unique Ticket IDs

            if (ticketIds.Count == 0)
            {
                Console.WriteLine("No tickets to process.");
                return;
            }

            // Retrieve tickets that need to be updated in a single query
            var ticketsToClose = await _ticketRepository.RetrieveAll()
                .Where(t => ticketIds.Contains(t.TicketId) && t.StatusId == 4)
                .ToListAsync(); // Fetch all relevant tickets

            if (ticketsToClose.Count == 0)
            {
                Console.WriteLine("No tickets to close.");
                return;
            }

            // Batch update tickets
            foreach (var ticket in ticketsToClose)
            {
                ticket.StatusId = 5;
            }

            // Assuming UpdateAsync can handle batch updates, otherwise, consider using a bulk update approach
            await _ticketRepository.UpdateTicketsAsync(ticketsToClose); // Update all tickets in a single call

            Console.WriteLine($"Closed {ticketsToClose.Count} tickets.");
        }
    }
}
