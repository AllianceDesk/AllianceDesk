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
            var data = _notificationService.RetrieveAll().Where(u => u.RecipientId == _sessionHelper.GetUserIdFromSession().ToString())
                        .Select(u => new NotificationServiceModel
                        {
                            NotificationId = u.NotificationId,
                            Title = u.Title,
                            Body = u.Body,
                            TicketId = u.TicketId,
                            RecipientId = u.RecipientId,
                            DateCreated = u.DateCreated,
                            TicketNumber = _ticketService.RetrieveAll().Where(t => t.TicketId.ToString() == u.TicketId).FirstOrDefault().TicketNumber,
                        });

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

            ViewBag.Role = user.RoleId;
            return View(data);
        }
    }
}