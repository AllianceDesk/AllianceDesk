using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class TeamRepository : BaseRepository, ITeamRepository
    {

        public TeamRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IEnumerable<Team> RetrieveAll()
        {
            return this.GetDbSet<Team>();
        }

        public bool TeamExists(string teamName)
        {
            return this.GetDbSet<Team>().Any(x => x.TeamName == teamName);
        }

        public void AddTeam(Team team)
        {
            this.GetDbSet<Team>().Add(team);
            UnitOfWork.SaveChanges();
        }
    }
}
