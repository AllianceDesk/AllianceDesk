using System;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Models
{
    public partial class Notification
    {
        public Guid NotificationId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public Guid TicketId { get; set; }
        public Guid RecipientId { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool Status { get; set; }

        public virtual User Recipient { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}
