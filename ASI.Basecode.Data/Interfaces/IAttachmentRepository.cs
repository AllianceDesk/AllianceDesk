using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IAttachmentRepository
    {
        IEnumerable<Attachment> GetAttachments();
        void AddAttachment(Attachment attachment);
        IEnumerable<Attachment> GetAttachmentsByTicketId(Guid TicketId);
    }
}
