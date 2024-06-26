using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Interfaces
{
    /// <summary>
    /// Interfaces of repository methods
    /// </summary>
    public interface INotificationRepository
    {

        IEnumerable<NotificationDataModel> RetrieveAll();
        void Add(NotificationDataModel model);
        void Update(NotificationDataModel model);
        void Delete(int id);
    }
}
