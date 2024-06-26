using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class NotificationRepository: INotificationRepository
    {
        private readonly List<NotificationDataModel> _data = new List<NotificationDataModel>();
        private int _nextId = 1;

        /// <summary>
        /// A method to retrieve all data from in memory or database
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NotificationDataModel> RetrieveAll()
        {
            return _data;
        }

        /// <summary>
        /// A method responsible for adding to the in memory or database
        /// </summary>
        /// <param name="model"></param>
        public void Add(NotificationDataModel model)
        {
            model.Id = _nextId++;
            _data.Add(model);
        }

        /// <summary>
        /// A method responsible for editing/updating the content from in memory or database
        /// </summary>
        /// <param name="model"></param>
        public void Update(NotificationDataModel model)
        {
            var existingData = _data.Where(x => x.Id == model.Id).FirstOrDefault();
            if (existingData != null)
            {
                existingData = model;
            }
        }

        /// <summary>
        /// A method to delete data from in memory or database
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var existingData = _data.Where(x => x.Id == id).FirstOrDefault();
            if (existingData != null)
            {
                _data.Remove(existingData);
            }
        }
    }
}
