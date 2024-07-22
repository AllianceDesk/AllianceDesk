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
    public class TicketStatusRepository : BaseRepository, ITicketStatusRepository
    {

        public TicketStatusRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IEnumerable<TicketStatus> RetrieveAll()
        {
            return this.GetDbSet<TicketStatus>();
        }

        public TicketStatus GetStatusById(byte statusId)
        {
            return this.GetDbSet<TicketStatus>().FirstOrDefault(x => x.StatusId == statusId);
        }
    }
}
