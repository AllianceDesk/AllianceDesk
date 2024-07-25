using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    /// <summary>
    /// Interfaces of service methods
    /// </summary>
    public interface INotificationService
    {
        IEnumerable<NotificationServiceModel> RetrieveAll();
        void Add(string ticketId, string userId);
        void Delete(string notificationId);
    }
}
