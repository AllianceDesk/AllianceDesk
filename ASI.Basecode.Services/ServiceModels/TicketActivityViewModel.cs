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
        public Guid HistoryId { get; set; }
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string Date { get; set; }
        public byte OperationId { get; set; }
        public string OperationName { get; set; }
        public string Message { get; set; }
    }
}
