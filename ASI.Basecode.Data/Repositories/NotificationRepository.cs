using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;


namespace ASI.Basecode.Data.Repositories
{
    public class NotificationRepository: BaseRepository, INotificationRepository
    {
        public NotificationRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        /// <summary>
        /// A method to retrieve all data from in memory or database
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Notification> RetrieveAll()
        {
            return this.GetDbSet<Notification>();
        }

        public void Add(Notification notification)
        {
            this.GetDbSet<Notification>().Add(notification);
            UnitOfWork.SaveChanges();
        }

        public void Update(Notification notification)
        {
            this.GetDbSet<Notification>().Update(notification);
            UnitOfWork.SaveChanges();
        }
        
        public async Task AddNotificationsAsync(List<Notification> notificationList)
        {
            foreach (var notification in notificationList)
            {
                this.GetDbSet<Notification>().Add(notification);
                await UnitOfWork.SaveChangesAsync();
            }
        }
    }
}
