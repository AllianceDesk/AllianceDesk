using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface ITeamService{
        void AddTeam(TeamViewModel model);
        IEnumerable<TeamViewModel> GetTeams();
        IEnumerable<Department> GetDepartments();
        IEnumerable<UserViewModel> GetAgents();
        int GetTeamNumber(string teamId);
        string GetDepartmentName(byte departmentId);
        UserPreferenceViewModel GetUserPreference();
        void UpdatePreference(UserPreferenceViewModel model);
    }
}
