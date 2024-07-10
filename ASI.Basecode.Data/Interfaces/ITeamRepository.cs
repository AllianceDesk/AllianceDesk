using System.Collections.Generic;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ITeamRepository
    {
        IEnumerable<Team> RetrieveAll();
        bool TeamExists(string teamName);
        void AddTeam(Team team);
    }
}