using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class UserViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

       [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmation Password is required.")]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
        public string ConfirmPassword { get; set; }

        public byte RoleId { get; set; }
        public string RoleName { get; set; }

        [Required(ErrorMessage = "Team Name is required.")]
        public string TeamName { get; set; }
        public string TeamId { get; set; }

        public List<UserViewModel> Users { get; set; }

    }
}