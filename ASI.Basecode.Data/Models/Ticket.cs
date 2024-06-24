using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Ticket
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string PriorityId { get; set; }
        public string Attachment { get; set; }
        public string CreatedBy { get; set; }
        public string AssignedTo { get; set; }
        public string StatusId { get; set; }
        public string FeedbackId { get; set; }
        
        public TicketPriority Priority { get; set; }
        public User Creator { get; set; }
        public User AssignedAgent { get; set; }
        public TicketStatus Status { get; set; }
        public Feedback Feedback { get; set; }

        public List<TicketActivity> TicketActivities { get; set; } = new List<TicketActivity>();
    }
}