using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class UserPreferenceViewModel
    {
        public Guid PreferenceId { get; set; }
        public Guid UserId { get; set; }
        public bool InAppNotifications { get; set; }
        public bool EmailNotifications { get; set; }
        public string DefaultTicketView { get; set; }
        public byte DefaultTicketPerPage { get; set; }
        public virtual User User { get; set; }
    }
}
