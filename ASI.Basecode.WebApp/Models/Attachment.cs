using System;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Models
{
    public partial class Attachment
    {
        public Guid AttachmentId { get; set; }
        public Guid TicketId { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }
        public Guid UploadedBy { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual User UploadedByNavigation { get; set; }
    }
}
