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
        public string TicketId { get; set; }

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

        public string CreatorId { get; set; }

        public string AgentId { get; set; }

        public string TeamId { get; set; }
        public DateTime DateCreated { get; set; }

        public List<IFormFile> AttachmentFiles { get; set; } = new List<IFormFile>();

        public List<string> AttachmentStrings { get; set; } = new List<string>();

        public string Priority { get; set; }

        public string Status { get; set; }

        public string Category { get; set; }

        public string FeedbackId { get; set; }

        public string AgentName { get; set; }

        public string CreatorName { get; set; }

        public string TeamName { get; set; }

        [FileSize(5)]
        [FileTypes(new[] { "image/jpeg", "image/png", "image/gif" })]
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

            foreach (var file in files)
            {
                if (file.Length > _maxSizeInMb * 1024 * 1024)
                {
                    return new ValidationResult($"Each file cannot be larger than {_maxSizeInMb} MB.");
                }
            }

            return ValidationResult.Success;
        }


    }

    public class FileTypesAttribute : ValidationAttribute
    {
        private readonly string[] _allowedTypes;

        public FileTypesAttribute(string[] allowedTypes)
        {
            _allowedTypes = allowedTypes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var files = value as List<IFormFile>;
            if (files == null)
                return ValidationResult.Success;

            foreach (var file in files)
            {
                if (file.Length > 0 && !_allowedTypes.Contains(file.ContentType))
                {
                    return new ValidationResult("Only image files are allowed (JPEG, PNG, GIF).");
                }
            }

            return ValidationResult.Success;
        }
    }
}

