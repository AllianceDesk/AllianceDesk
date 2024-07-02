using System.Collections.Generic;
using System;

namespace ASI.Basecode.Data.Models
{
    /// <summary>
    /// Model for Notifications
    /// </summary>
    public class NotificationDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}