using System;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Models
{
    public partial class User
    {
        public User()
        {
            ArticleCreatedByNavigations = new HashSet<Article>();
            ArticleUpdatedByNavigations = new HashSet<Article>();
            Attachments = new HashSet<Attachment>();
            Favorites = new HashSet<Favorite>();
            Feedbacks = new HashSet<Feedback>();
            TicketActivities = new HashSet<TicketActivity>();
            TicketAssignedAgentNavigations = new HashSet<Ticket>();
            TicketCreatedByNavigations = new HashSet<Ticket>();
            TicketMessages = new HashSet<TicketMessage>();
            UserPreferences = new HashSet<UserPreference>();
        }

        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte RoleId { get; set; }
        public Guid? TeamId { get; set; }

        public virtual UserRole Role { get; set; }
        public virtual Team Team { get; set; }
        public virtual ICollection<Article> ArticleCreatedByNavigations { get; set; }
        public virtual ICollection<Article> ArticleUpdatedByNavigations { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<TicketActivity> TicketActivities { get; set; }
        public virtual ICollection<Ticket> TicketAssignedAgentNavigations { get; set; }
        public virtual ICollection<Ticket> TicketCreatedByNavigations { get; set; }
        public virtual ICollection<TicketMessage> TicketMessages { get; set; }
        public virtual ICollection<UserPreference> UserPreferences { get; set; }
    }
}
