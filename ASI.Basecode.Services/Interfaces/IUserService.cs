using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
        public interface IUserService
        {
                LoginResult AuthenticateUser(string userid, string password, ref User user);
                int GetLogInUserRole(string userName);
                void AddUser(UserViewModel model);
                User GetUserById(string id);
                void AddTeam(UserViewModel model);
                void UpdateUser(UserViewModel model);
                void DeleteUser(string userId);
                IEnumerable<TeamViewModel> GetTeams();
                IEnumerable<UserRole> GetUserRoles();
                IEnumerable<User> GetUsers();
                IEnumerable<User> GetAllUsers();
                IEnumerable<UserViewModel> GetAgents();
                string GeneratePassword();
                List<TicketActivityViewModel> GetRecentUserActivity();
                UserPreferenceViewModel GetPreferenceView();
                void UpdatePreference(UserPreferenceViewModel model);
        }
}
