using System;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Models
{
    public partial class TicketActivity
    {
        public Guid HistoryId { get; set; }
        public Guid TicketId { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public byte OperationId { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public virtual User ModifiedByNavigation { get; set; }
        public virtual TicketActivityOperation Operation { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}
