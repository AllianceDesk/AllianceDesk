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
        IQueryable<TicketActivity> RetrieveAll();
        void Delete(Guid id);
        IQueryable<TicketActivity> GetActivitiesByTicketId(Guid ticketId);
        IQueryable<TicketActivity> GetActivitiesByTicketIds(List<Guid> ticketId);
        Task AddTicketActivitiesAsync(List<TicketActivity> ticketActivities);
    }
}
