using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class UserActivityViewModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string DateTime { get; set; }
        public string TicketTitle { get; set; }
        public string TicketStatus { get; set; }

    }
}