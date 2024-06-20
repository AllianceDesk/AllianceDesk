using System;

namespace ASI.Basecode.Data.Models
{
    public partial class TicketHistory
    {
        public int HistoryId { get; set; }
        public int TicketId { get; set; }
        public int PerformedBy { get; set; }
        public DateTime PerformedAt { get; set; }
        public string EventType { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public Ticket Ticket { get; set; }

        // The user that made changes
        public User User { get; set; }
    }
}