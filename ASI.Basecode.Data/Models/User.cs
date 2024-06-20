using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }

        // public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        // public string UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }

        public List<Ticket> CreatedTickets { get; set; } = new List<Ticket>();
        public List<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
        public List<TicketHistory> TicketHistories { get; set; } = new List<TicketHistory>();
        public List<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public List<UserPreference> UserPreferences { get; set; } = new List<UserPreference>();
    }
}