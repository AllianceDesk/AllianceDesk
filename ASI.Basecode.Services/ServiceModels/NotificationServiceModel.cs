using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    /// <summary>
    /// A model for service
    /// </summary>
    public class NotificationServiceModel
    {
        public string NotificationId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string TicketId { get; set; }
        public string DateCreated { get; set; }
        public string RecipientId { get; set; }
    }
}
