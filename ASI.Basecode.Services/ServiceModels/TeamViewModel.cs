using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class TeamViewModel
    {
        public string TeamId { get; set; }
        public string TeamName { get; set; }
        public char FirstLetter => !string.IsNullOrEmpty(TeamName) ? TeamName.Trim()[0] : '?';
        public string TeamDescription { get; set; }
        public byte DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int TeamNumber { get; set; }
        public List<TeamViewModel> Teams { get; set; }
        public List<UserViewModel> Agents { get; set; }
    }
}
