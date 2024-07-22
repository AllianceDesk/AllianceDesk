using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class OverAllTicketCountViewModel
    {
        public Dictionary<string, int> TicketCountsByCategory { get; set; }
        public Dictionary<string, int> TicketCountsByStatus { get; set; }
        public Dictionary<string, int> TicketCountsByPriority { get; set; }
        public Dictionary<string, int> TicketCountsByDay { get; set; }
        public int TotalTicketCount { get; set; }
        public List<TicketActivityViewModel> RecentUserActivities { get; set; }
    }
} 
