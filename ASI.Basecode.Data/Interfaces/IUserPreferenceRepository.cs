using System.Collections.Generic;
using ASI.Basecode.Data.Models;
using System;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IUserPreferenceRepository
    {
        void Add(UserPreference userPreference);
        void Update(UserPreference userPreference);
        IEnumerable<UserPreference> RetrieveAll();
        UserPreference GetUserPreferencesByUserId(Guid userId);
    }
}
