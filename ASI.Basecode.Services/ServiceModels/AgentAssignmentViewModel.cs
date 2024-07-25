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
        public Guid TicketId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public List<UserViewModel> Agents { get; set; } // Assuming User is your user model
        public Dictionary<Guid, int> TicketCount { get; set; }

        //For submitting the form
        public Guid SelectedAgentId { get; set; }
    }
}