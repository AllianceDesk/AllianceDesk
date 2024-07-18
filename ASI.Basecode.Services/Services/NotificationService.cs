using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// A constructor for service
        /// </summary>
        /// <param name="notificationRepository"></param>
        /// <param name="mapper"></param>
        public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// A method to retrieve the matched data from data model
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NotificationServiceModel> RetrieveAll()
        {
            var data = _notificationRepository.RetrieveAll().Select(s => new NotificationServiceModel
            {
                NotificationId = s.NotificationId.ToString(),
                Title = s.Title,
                Body = s.Body,
                DateCreated = s.DateCreated.ToString(),
            });

            return data;
        }

        /// <summary>
        /// A method to add new data 
        /// </summary>
        /// <param name="model"></param>
        public void Add(NotificationServiceModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "ArticleViewModel cannot be null");
            }

            var newNotificaiton = new Notification();
            newNotificaiton.NotificationId = Guid.NewGuid();
            newNotificaiton.Title = model.Title;
            newNotificaiton.Body = model.Body;
            newNotificaiton.DateCreated = DateTime.Now;
            newNotificaiton.RecipientId = Guid.Parse(model.RecipientId);
            newNotificaiton.Status = true;

            _notificationRepository.Add(newNotificaiton);
        }

        /// <summary>
        /// A method to update the matched data
        /// </summary>
        /// <param name="model"></param>

        public void Delete (string notificationId)
        {
            var _existingData = _notificationRepository.RetrieveAll().Where(d => d.NotificationId.ToString() == notificationId).FirstOrDefault();
            if (_existingData != null)
            {
                _existingData.Status = false;
                _notificationRepository.Update(_existingData);
            }
        }

    }
}
