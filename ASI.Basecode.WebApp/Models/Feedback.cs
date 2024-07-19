using System;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Models
{
    public partial class Feedback
    {
        public Guid FeedbackId { get; set; }
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public byte Rating { get; set; }
        public string Comments { get; set; }
        public DateTime? DateCreated { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual User User { get; set; }
    }
}
