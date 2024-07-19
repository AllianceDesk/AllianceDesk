using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class AnalyticsViewModel
    {
        public List<AgentTicketCountViewModel> Agents { get; set; }

        public List<TeamTicketCountViewModel> Teams { get; set; }
    }
} 
