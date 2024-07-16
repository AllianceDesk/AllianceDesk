using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class AgentAssignmentViewModel
    {   
        public string TicketId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public List<UserViewModel> Agents { get; set; } // Assuming User is your user model
        public List<AgentTicketCountViewModel> AssignedTicketCounts { get; set; }

        //For submitting the form
        public string SelectedAgentId { get; set; }
    }

    public class AgentTicketCountViewModel
    {
        public UserViewModel Agent { get; set; }
        public int TicketCount { get; set; }
    }
}