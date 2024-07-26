using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;

namespace ASI.Basecode.Data.Repositories
{
    public class TicketRepository : BaseRepository, ITicketRepository
    {

        public TicketRepository(IUnitOfWork unitOfWork): base(unitOfWork)
        {

        }

        public IQueryable<Ticket> RetrieveAll()
        {
            return this.GetDbSet<Ticket>();
        }

        public void Add(Ticket ticket)
        {
            this.GetDbSet<Ticket>().Add(ticket);
            UnitOfWork.SaveChanges();
        }

        public void Update(Ticket ticket)
        {
            this.GetDbSet<Ticket>().Update(ticket);
            UnitOfWork.SaveChanges();
        }

        public async Task UpdateTicketsAsync(List<Ticket> tickets)
        {
            foreach (var ticket in tickets)
            {
                this.GetDbSet<Ticket>().Update(ticket);
                await UnitOfWork.SaveChangesAsync();
            }
        }

        public void Delete(Guid id)
        {
            var ticketToDelete = this.GetDbSet<Ticket>().FirstOrDefault(x => x.TicketId == id);

            if (ticketToDelete != null)
            {
                this.GetDbSet<Ticket>().Remove(ticketToDelete);
                UnitOfWork.SaveChanges();
            }
        }

        public Ticket GetTicketById(Guid id)
        {
            return this.GetDbSet<Ticket>().FirstOrDefault(x => x.TicketId == id);
        }

        public IQueryable<Ticket> GetUserTicketsById(Guid id)
        {
           return this.GetDbSet<Ticket>().Where(x => x.CreatedBy == id);
        }

        public IQueryable<Ticket> GetAgentTicketsById(Guid id)
        {
            return this.GetDbSet<Ticket>().Where(x => x.AssignedAgent == id);
        }
        
        public IQueryable<Ticket> GetWeeklyTickets(DateTime startOfWeek, DateTime endOfWeek)
        {
            return this.GetDbSet<Ticket>().Where(x => x.DateCreated >= startOfWeek && x.DateCreated <= endOfWeek);
        }
    }
}
