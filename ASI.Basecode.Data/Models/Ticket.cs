using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Ticket
    {
        public int TicketId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int PriorityId { get; set; }
        public string Attachment { get; set; }
        public int CreatedBy { get; set; }
        public int? AssignedTo { get; set; }
        public int StatusId { get; set; }
        public int FeedbackId { get; set; }
        
        public Priority Priority { get; set; }
        public User Creator { get; set; }
        public User Assignee { get; set; }
        public Status Status { get; set; }
        public Feedback Feedback { get; set; }

        public List<TicketHistory> TicketHistories { get; set; } = new List<TicketHistory>();
    }
}