using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class AgentTicketCountViewModel
    {
        public UserViewModel Agent { get; set; }

        public Dictionary<string, int> TicketCountsByCategory { get; set; }

        public Dictionary<string, int> TicketCountsByStatus { get; set; }

        public int TotalTicketCount;
    }
}
