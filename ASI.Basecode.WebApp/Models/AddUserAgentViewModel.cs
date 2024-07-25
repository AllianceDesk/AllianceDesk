using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Models
{
    public class AddUserAgentViewModel
    {
        public IEnumerable<SelectListItem> Teams { get; set; }
        public IEnumerable<SelectListItem> UserRoles { get; set; }
        public int SelectedRoleId { get; set; } // For binding the selected role, if needed
        public int SelectedTeamId { get; set; } // For binding the selected team, if needed
    }
}
