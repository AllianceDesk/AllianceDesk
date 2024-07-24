using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    /// <summary>
    /// Interfaces of repository methods
    /// </summary>
    public interface INotificationRepository
    {

        IEnumerable<Notification> RetrieveAll();
        void Add(Notification notification);
        void Update(Notification model);
        Task AddNotificationsAsync(List<Notification> notificationList);
    }
}
