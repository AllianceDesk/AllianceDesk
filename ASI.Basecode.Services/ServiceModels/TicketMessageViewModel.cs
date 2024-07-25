using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class TicketMessageViewModel
    {
        public Guid MessageId { get; set; }
        
        [Required]
        public Guid TicketId { get; set; }
        public Guid SentById { get; set; }
        public string SentByName { get; set; }
        
        [Required]
        public string Message { get; set; }
        public DateTime PostedAt { get; set; }
    }
}
