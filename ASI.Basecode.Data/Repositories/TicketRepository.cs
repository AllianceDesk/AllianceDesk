using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<Ticket> RetrieveAll()
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

        public void Delete(String id)
        {
            var ticketToDelete = this.GetDbSet<Ticket>().FirstOrDefault(x => x.TicketId.ToString() == id);

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
    }
}
