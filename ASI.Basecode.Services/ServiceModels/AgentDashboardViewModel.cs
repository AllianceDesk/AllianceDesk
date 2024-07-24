using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class AgentDashboardViewModel
    {
        public IEnumerable<TicketViewModel> Tickets { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string CurrentStatus { get; set; }
        public string CurrentSearchTerm { get; set; }

    }
}
