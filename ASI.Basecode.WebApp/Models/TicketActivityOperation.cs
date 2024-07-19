using System;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Models
{
    public partial class TicketActivityOperation
    {
        public TicketActivityOperation()
        {
            TicketActivities = new HashSet<TicketActivity>();
        }

        public byte OperationId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<TicketActivity> TicketActivities { get; set; }
    }
}
