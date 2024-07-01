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
                Id = s.Id,
                Name = s.Name,
                Subject = s.Subject,
            });

            return data;
        }

        /// <summary>
        /// A method to add new data 
        /// </summary>
        /// <param name="model"></param>
        public void Add(NotificationServiceModel model)
        {
            var newModel = new NotificationDataModel();
            _mapper.Map(model, newModel);
            newModel.CreatedBy = "Lance";
            newModel.CreatedDate = DateTime.Now;

            _notificationRepository.Add(newModel);
        }

        /// <summary>
        /// A method to update the matched data
        /// </summary>
        /// <param name="model"></param>
        public void Update(NotificationServiceModel model)
        {
            var existingData = _notificationRepository.RetrieveAll().Where(s => s.Id == model.Id).FirstOrDefault();
            _mapper.Map(model, existingData);
            existingData.UpdatedBy = "Lance";
            existingData.UpdatedDate = DateTime.Now;
            _notificationRepository.Update(existingData);
        }

        /// <summary>
        /// A method to delete the matched id
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            _notificationRepository.Delete(id);
        }

    }
}
