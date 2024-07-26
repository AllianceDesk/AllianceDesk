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
        private readonly ITicketRepository _ticketRepository;

        /// <summary>
        /// A constructor for service
        /// </summary>
        /// <param name="notificationRepository"></param>
        /// <param name="mapper"></param>
        public NotificationService(INotificationRepository notificationRepository, IMapper mapper, ITicketRepository ticketRepository)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _ticketRepository = ticketRepository;
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
                DateCreated = s.DateCreated.HasValue ? s.DateCreated.Value.ToString("MMM dd, yyyy hh:mm tt") : string.Empty,
                RecipientId = s.RecipientId.ToString(),
                TicketId = s.TicketId.ToString(),
                TicketNumber = _ticketRepository.RetrieveAll().Where(t => t.TicketId == s.TicketId).FirstOrDefault().TicketNumber,
            });

            return data;
        }

        /// <summary>
        /// A method to add new data 
        /// </summary>
        /// <param name="model"></param>
        public void Add(string ticketId, string userId)
        {
            if (ticketId == null)
            {
                throw new ArgumentNullException(nameof(ticketId), "TicketId cannot be null");
            }

            var ticketInfo = _ticketRepository.GetTicketById(Guid.Parse(ticketId));

            if (ticketInfo == null)
            {
                var newNotification = new Notification();
                newNotification.NotificationId = Guid.NewGuid();
                newNotification.TicketId = Guid.Parse(ticketId);
                newNotification.Title = ticketInfo.Title;
                newNotification.Body = ticketInfo.Description;
                newNotification.DateCreated = DateTime.Now;
                newNotification.RecipientId = Guid.Parse(userId);
                newNotification.Status = true;
                
                _notificationRepository.Add(newNotification);
            }
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
