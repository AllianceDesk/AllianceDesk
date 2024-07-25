using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    public class NotificationController : ControllerBase<NotificationController>
    {
        private readonly INotificationService _notificationService;
        private readonly ITicketService _ticketService;
        private readonly ISessionHelper _sessionHelper;
        private readonly IUserService _userService;
        /// <param name = "httpContextAccessor" ></ param >
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>

        public NotificationController(
            INotificationService notificationService,
            ISessionHelper sessionHelper,
            ITicketService ticketService,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _notificationService = notificationService;
            _sessionHelper = sessionHelper;
            _ticketService = ticketService;
            _userService = userService;
        }

        /// <summary>
        /// Controller for index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var currentUser = _sessionHelper.GetUserIdFromSession().ToString();

            var data = _notificationService.RetrieveAll()
                .Where(u => u.RecipientId == currentUser)
                .Select(u => new NotificationServiceModel
                {
                    NotificationId = u.NotificationId,
                    Title = u.Title,
                    Body = u.Body,
                    TicketId = u.TicketId,
                    RecipientId = u.RecipientId,
                    DateCreated = u.DateCreated,
                    TicketNumber = u.TicketNumber,
                })
                .ToList();

            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _userService.GetUserById(Guid.Parse(userId).ToString());
            if (user == null)
            {
                return NotFound();
            }

            var notifList = new NotificationServiceModel
            {
                UserNotifications = data
            };

            ViewBag.Role = user.RoleId;

            return View(notifList);
        }

        [HttpGet("/Notification/DetailModal")]
        public IActionResult DetailModal(string notificationId)
        {
            var data = _notificationService.RetrieveAll().Where(n => n.NotificationId == notificationId).FirstOrDefault();

            if (data == null)
            {
                return NotFound();
            }

            var viewModal = new NotificationServiceModel
            {
                NotificationId = data.NotificationId,
                Title = data.Title,
                Body = data.Body,
                TicketId = data.TicketId,
                RecipientId = data.RecipientId,
                DateCreated = data.DateCreated,
                TicketNumber = data.TicketNumber,
                TicketStatus = _ticketService.GetById(data.TicketId).Status,
                AgentName = _ticketService.GetById(data.TicketId).AgentName,
                RoleId = _userService.GetUserById(_sessionHelper.GetUserIdFromSession().ToString()).RoleId,
            };

            return PartialView("DetailModal", viewModal);
        }

        public IActionResult Notif()
        {
            return View();
        }
    }
}