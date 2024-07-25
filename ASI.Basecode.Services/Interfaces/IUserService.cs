using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using static ASI.Basecode.Resources.Constants.Enums;
using System;

namespace ASI.Basecode.Services.Interfaces
{
        public interface IUserService
        {
                LoginResult AuthenticateUser(string userName, string password, ref User user);
                int GetLogInUserRole(string userName);
                void AddUser(UserViewModel model);
                User GetUserById(Guid userId);
                void AddTeam(UserViewModel model);
                void UpdateUser(UserViewModel model);
                void DeleteUser(Guid userId);
                IEnumerable<TeamViewModel> GetTeams();
                IEnumerable<UserRole> GetUserRoles();
                IEnumerable<User> GetUsers();
                IEnumerable<User> GetAllUsers();
                IEnumerable<UserViewModel> GetAgents();
                string GeneratePassword();
                List<TicketActivityViewModel> GetRecentUserActivity();
                List<TicketActivityViewModel> GetUserActivity(Guid userId);
                UserPreferenceViewModel GetUserPreference();
                void UpdatePreference(UserPreferenceViewModel model); 
        }
}
