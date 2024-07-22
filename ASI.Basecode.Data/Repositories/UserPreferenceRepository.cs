using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Repositories
{
    public class UserPreferenceRepository: BaseRepository, IUserPreferenceRepository
    {
        public UserPreferenceRepository(IUnitOfWork unitOfWork): base(unitOfWork)
        {
            
        }

        public void Add(UserPreference userPreference)
        {
            this.GetDbSet<UserPreference>().Add(userPreference);
            UnitOfWork.SaveChanges();
        }

        public void Update(UserPreference userPreference)
        {
            this.GetDbSet<UserPreference>().Update(userPreference);
            UnitOfWork.SaveChanges();
        }
        
        public IEnumerable<UserPreference> RetrieveAll()
        {
           return this.GetDbSet<UserPreference>();
        }

        public UserPreference GetUserPreferencesByUserId(Guid userId)
        {
            return this.GetDbSet<UserPreference>().FirstOrDefault(x => x.UserId == userId);
        }
    }
}
