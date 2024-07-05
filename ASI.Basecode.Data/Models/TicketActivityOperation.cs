using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class TicketActivityOperation
    {

        /*
         operation_id	name
        1	Update
        2	Assign
        3	Reassign
        4	Close
        5	Reopen
         */
        public TicketActivityOperation()
        {
            TicketActivities = new HashSet<TicketActivity>();
        }

        public byte OperationId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<TicketActivity> TicketActivities { get; set; }
    }
}
