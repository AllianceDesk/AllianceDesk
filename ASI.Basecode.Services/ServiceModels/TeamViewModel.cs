using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class TeamViewModel
    {
        public string TeamId { get; set; }
        [Required(ErrorMessage = "Team Name is required.")]
        public string TeamName { get; set; }
        public char FirstLetter => !string.IsNullOrEmpty(TeamName) ? TeamName.Trim()[0] : '?';
        [Required(ErrorMessage = "Team Description is required.")]
        public string TeamDescription { get; set; }
        [Required(ErrorMessage = "Department is required.")]
        public byte DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int TeamNumber { get; set; }
        public List<TeamViewModel> Teams { get; set; }
        public List<UserViewModel> Agents { get; set; }
    }
}
