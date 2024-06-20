using System;

namespace ASI.Basecode.Data.Models
{
    public partial class TicketActivity
    {
        public int ActivityId { get; set; }
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public int PerformedBy { get; set; }
        public User User { get; set; }


        public DateTime PerformedAt { get; set; }
        public int OperationId { get; set; }
        public TicketOperation Operation { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}