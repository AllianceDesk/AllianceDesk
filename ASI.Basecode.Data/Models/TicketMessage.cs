using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class TicketMessage
    {
        public Guid MessageId { get; set; }
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public string MessageBody { get; set; }
        public DateTime PostedAt { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual User User { get; set; }
    }
}
