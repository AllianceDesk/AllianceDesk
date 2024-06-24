using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public  class TicketViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string PriorityId { get; set; }
        public string Attachment { get; set; }
        public string CreatedBy { get; set; }
        public string StatusId { get; set; }
        public string FeedbackId { get; set; }
    }
}
