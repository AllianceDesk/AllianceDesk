using ASI.Basecode.Data.Interfaces;
using Basecode.Data.Repositories;
using System.Collections.Generic;
using ASI.Basecode.Data.Models;
using System.Linq;


namespace ASI.Basecode.Data.Repositories
{
    public class TicketMessageRepository : BaseRepository, ITicketMessageRepository
    {
        public TicketMessageRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public void Add(TicketMessage ticketMessage)
        {
            this.GetDbSet<TicketMessage>().Add(ticketMessage);
            UnitOfWork.SaveChanges();
        }

        public IEnumerable<TicketMessage> RetrieveAll()
        {
            return this.GetDbSet<TicketMessage>();
        }

        public IEnumerable<TicketMessage> GetMessagesByTicketId(string ticketId)
        { 
            return this.GetDbSet<TicketMessage>().Where(x => x.TicketId.ToString() == ticketId);
        }
    }
}