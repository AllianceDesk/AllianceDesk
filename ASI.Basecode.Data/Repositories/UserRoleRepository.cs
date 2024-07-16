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
    public class UserRoleRepository : BaseRepository, IUserRoleRepository
    {

        public UserRoleRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IEnumerable<UserRole> RetrieveAll()
        {
            return this.GetDbSet<UserRole>();
        }
    }
}