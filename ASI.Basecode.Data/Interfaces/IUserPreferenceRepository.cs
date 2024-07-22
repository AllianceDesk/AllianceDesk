using System.Collections.Generic;
using ASI.Basecode.Data.Models;
using System;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IUserPreferenceRepository
    {
        void Add(UserPreference userPreference);
        IEnumerable<UserPreference> RetrieveAll();
        void Delete(Guid id);
        UserPreference GetUserPreferencesByUserId(Guid userId);
    }
}
