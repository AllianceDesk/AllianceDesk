using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class UserTicketViewModel
    {
        public IEnumerable<TicketViewModel> Tickets { get; set; }
        public TicketViewModel Ticket { get; set; }
    }
}
