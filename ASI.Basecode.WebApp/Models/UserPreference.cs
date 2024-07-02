using System;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Models
{
    public partial class UserPreference
    {
        public Guid PreferenceId { get; set; }
        public Guid UserId { get; set; }
        public bool InAppNotifications { get; set; }
        public bool EmailNotifications { get; set; }
        public byte DefaultTicketView { get; set; }

        public virtual User User { get; set; }
    }
}
