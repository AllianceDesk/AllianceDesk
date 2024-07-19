using System;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Models
{
    public partial class TicketPriority
    {
        public TicketPriority()
        {
            Tickets = new HashSet<Ticket>();
        }

        public byte PriorityId { get; set; }
        public string PriorityName { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
