using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class TicketActivityViewModel
    {
        public string HistoryId { get; set; }
        public string TicketId { get; set; }
        public string ModifiedBy { get; set; }
        
        public string ModifiedByName { get; set; }

        public DateTime ModifiedAt { get; set; }
        public byte OperationId { get; set; }
        public string OperationName { get; set; }
        public string message { get; set; }
    }
}
