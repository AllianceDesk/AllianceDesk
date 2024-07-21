using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IFeedbackRepository
    {
        IEnumerable<Feedback> RetrieveAll();
        void Add(Feedback feedback);
        Feedback GetFeedbackByTicketId(Guid id);
        IEnumerable<Feedback> GetFeedbackByTicketIds(List<Guid> ticketId);
    }
}
