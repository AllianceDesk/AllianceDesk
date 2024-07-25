using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ITicketRepository
    {
        IQueryable<Ticket> RetrieveAll();
        
        void Add(Ticket ticket);

        void Update(Ticket ticket);

        Task UpdateTicketsAsync(List<Ticket> tickets);

        void Delete(Guid id);

        Ticket GetTicketById(Guid id);

        IEnumerable<Ticket> GetUserTicketsById(Guid id);

        IEnumerable<Ticket> GetAgentTicketsById(Guid id);
        
        IQueryable<Ticket> GetWeeklyTickets(DateTime startOfWeek, DateTime endOfWeek);
    }
}