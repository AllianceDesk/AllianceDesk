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
    public class AttachmentRepository : BaseRepository, IAttachmentRepository
    {
        public AttachmentRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
        public IEnumerable<Attachment> GetAttachments()
        {
            return this.GetDbSet<Attachment>();
        }

        public void AddAttachment(Attachment attachment)
        {
            this.GetDbSet<Attachment>().Add(attachment);
            UnitOfWork.SaveChanges();
        }

        public IEnumerable<Attachment> GetAttachmentsByTicketId(Guid ticketId)
        {
            return this.GetDbSet<Attachment>().Where(u => u.TicketId == ticketId);
        }
    }
}
