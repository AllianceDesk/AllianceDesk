using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class TicketPriority
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}