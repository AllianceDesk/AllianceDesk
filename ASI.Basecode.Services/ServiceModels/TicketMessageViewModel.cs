using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class TicketMessageViewModel
    {
        public Guid MessageId { get; set; }
        public Guid TicketId { get; set; }
        public Guid SentById { get; set; }
        public string SentByName { get; set; }
        public string Message { get; set; }
        public DateTime PostedAt { get; set; }
    }
}
