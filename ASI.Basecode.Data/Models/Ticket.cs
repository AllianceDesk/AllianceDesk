using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Ticket
    {
        public Ticket()
        {
            Attachments = new HashSet<Attachment>();
            Feedbacks = new HashSet<Feedback>();
            Notifications = new HashSet<Notification>();
            TicketActivities = new HashSet<TicketActivity>();
            TicketMessages = new HashSet<TicketMessage>();
        }

        public Guid TicketId { get; set; }
        public string TicketNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte CategoryId { get; set; }
        public byte PriorityId { get; set; }
        public byte StatusId { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? AssignedAgent { get; set; }
        public Guid? AssignedTeam { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateClosed { get; set; }
        public virtual User AssignedAgentNavigation { get; set; }
        public virtual Team AssignedTeamNavigation { get; set; }
        public virtual Category Category { get; set; }
        public virtual User CreatedByNavigation { get; set; }
        public virtual TicketPriority Priority { get; set; }
        public virtual TicketStatus Status { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<TicketActivity> TicketActivities { get; set; }
        public virtual ICollection<TicketMessage> TicketMessages { get; set; }
    }
}
