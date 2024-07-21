using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class FeedbackViewModel
    {
        public Guid FeedbackId { get; set; }
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public byte Rating { get; set; }
        public string Comments { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
