using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Status
    {
        public int StatusId { get; set; }
        public string Name { get; set; }

        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}