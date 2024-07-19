using System;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Models
{
    public partial class TicketStatus
    {
        public TicketStatus()
        {
            Tickets = new HashSet<Ticket>();
        }

        public byte StatusId { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
