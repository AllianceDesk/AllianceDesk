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
        public string MessageId { get; set; }
        public string TicketId { get; set; }
        public string SentById { get; set; }
        public string SentByName { get; set; }
        public string Message { get; set; }
        public DateTime PostedAt { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual User User { get; set; }
    }
}
