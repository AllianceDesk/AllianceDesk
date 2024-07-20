using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ITicketActivityRepository
    {
        void Add(TicketActivity ticketActivity);
        IEnumerable<TicketActivity> RetrieveAll();
        void Delete(Guid id);
        IEnumerable<TicketActivity> GetActivitiesByTicketId(Guid ticketId);

        IEnumerable<TicketActivity> GetActivitiesByTicketIds(List<Guid> ticketId);
    }
}
