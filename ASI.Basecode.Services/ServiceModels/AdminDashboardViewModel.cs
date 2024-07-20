using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class AdminDashboardViewModel
    {
        public Dictionary<string, int> TicketCountsByDay { get; set; }
        public UserViewModel User { get; set; }
        public ArticleViewModel Article { get; set; }
    }
} 
