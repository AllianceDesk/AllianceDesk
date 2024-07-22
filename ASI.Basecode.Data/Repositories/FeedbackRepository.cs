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
    public class FeedbackRepository : BaseRepository, IFeedbackRepository
    {
        public FeedbackRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IEnumerable<Feedback> RetrieveAll()
        {
            return this.GetDbSet<Feedback>();
        }

        public void Add(Feedback feedback)
        {
            this.GetDbSet<Feedback>().Add(feedback);
            UnitOfWork.SaveChanges();
        }

        public Feedback GetFeedbackByTicketId(Guid id)
        {
            return this.GetDbSet<Feedback>().Where(f => f.TicketId == id).FirstOrDefault();
        }

        public IEnumerable<Feedback> GetFeedbackByTicketIds(List<Guid> ticketId)
        {
            return this.GetDbSet<Feedback>().Where(f => ticketId.Contains(f.TicketId));
        }
    }
}
