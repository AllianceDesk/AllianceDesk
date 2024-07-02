using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Team
    {
        public Team()
        {
            Tickets = new HashSet<Ticket>();
            Users = new HashSet<User>();
        }

        public Guid TeamId { get; set; }
        public string TeamName { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
