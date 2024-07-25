using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class TeamViewModel
    {
        public Guid TeamId { get; set; }
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
