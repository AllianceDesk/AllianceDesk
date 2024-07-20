using ASI.Basecode.Data.Models;
using NUlid;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class TicketViewModel
    {
        public string TicketId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string CategoryId { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public string PriorityId { get; set; }
        
        public string StatusId { get; set; }

        public string CreatorId { get; set; }
        
        public string AgentId { get; set; }

        public string TeamId { get; set; }

        public DateTime DateCreated { get; set; }

        public List<Attachment> Attachments { get; set; } = new List<Attachment>();

        public string Priority { get; set; }

        public string Status { get; set; }

        public string Category { get; set; }

        public string Attachment { get; set; }

        public string FeedbackId { get; set; }

        public string AgentName { get; set; }

        public string CreatorName { get; set; }

        public string TeamName { get; set; }

        public IEnumerable<TicketActivityViewModel> TicketActivities { get; set; }

        public IEnumerable<TicketMessageViewModel> TicketMessages { get; set; }

        public string NewMessageBody { get; set; }

        public DateTime DateAssigned { get; set; }

        public TicketActivity LatestUpdate { get; set; }

        public string RelativeTime
        {
            get
            {
                var timespan = DateTime.Now - DateCreated;
                if (timespan.TotalMinutes < 1)
                    return "Just now";
                if (timespan.TotalMinutes < 60)
                    return $"{timespan.Minutes} minutes ago";
                if (timespan.TotalHours < 24)
                    return $"{timespan.Hours} hours ago";
                if (timespan.TotalDays < 7)
                    return $"{timespan.Days} days ago";
                if (timespan.TotalDays < 30)
                    return $"{timespan.Days / 7} weeks ago";
                if (timespan.TotalDays < 365)
                    return $"{timespan.Days / 30} months ago";
                return $"{timespan.Days / 365} years ago";
            }
        }
    }
}