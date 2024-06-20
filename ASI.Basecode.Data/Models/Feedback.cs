using System;

namespace ASI.Basecode.Data.Models
{
    public partial class Feedback
    {
        public int FeedbackId { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }
        public DateTime DateCreated { get; set; }

        public Ticket Ticket { get; set; }
        public User User { get; set; }
    }
}