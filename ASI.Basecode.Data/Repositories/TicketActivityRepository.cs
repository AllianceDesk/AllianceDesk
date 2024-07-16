using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class TicketActivityRepository : BaseRepository, ITicketActivityRepository
    {
        public TicketActivityRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public void Add(TicketActivity ticketActivity)
        {
            this.GetDbSet<TicketActivity>().Add(ticketActivity);
            UnitOfWork.SaveChanges();
        }

        public IEnumerable<TicketActivity> RetrieveAll()
        {
            return this.GetDbSet<TicketActivity>();
        }
    }
}
