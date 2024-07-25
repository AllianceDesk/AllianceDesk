using ASI.Basecode.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace ASI.Basecode.Services.ServiceModels
{
    public class TicketViewModel
    {
        public Guid TicketId { get; set; }
        public string TicketNumber { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public byte CategoryId { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public byte PriorityId { get; set; }
       
        public byte StatusId { get; set; }
        
        public Guid CreatorId { get; set; }
        
        public Guid? AgentId { get; set; }
        
        public Guid? TeamId { get; set; }
        
        public DateTime DateCreated { get; set; }
        
        public List<IFormFile> AttachmentFiles { get; set; } = new List<IFormFile>();
        
        public List<string> AttachmentStrings { get; set; } = new List<string>();
        
        public string Priority { get; set; }
        
        public string Status { get; set; }
        
        public string Category { get; set; }
        
        public string FeedbackId { get; set; }
        
        public string CreatorName { get; set; }
        
        public string TeamName { get; set; }

        public string AgentName { get; set; }

        [FileSize(5)]
        public IEnumerable<TicketActivityViewModel> TicketHistory { get; set; }
        
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
        
        public FeedbackViewModel Feedback { get; set; }
    }


    /// <summary>
    /// File Size Validation
    /// </summary>
    /// <seealso cref="System.ComponentModel.DataAnnotations.ValidationAttribute" />
    public class FileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxSizeInMb;

        public FileSizeAttribute(int maxSizeInMb)
        {
            _maxSizeInMb = maxSizeInMb;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var files = value as List<IFormFile>;
            if (files == null)
                return ValidationResult.Success;

            long totalMb = 0;
            foreach (var file in files)
            {
                totalMb += file.Length;
            }

            if (totalMb > _maxSizeInMb * 1024 * 1024)
            {
                return new ValidationResult($"Total file size of images cannot be larger than {_maxSizeInMb} MB.");
            }

            return ValidationResult.Success;
        }
    }
}

