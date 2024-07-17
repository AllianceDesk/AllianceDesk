using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    public class NotificationController : ControllerBase<NotificationController>
    {
        private readonly INotificationService _notificationService;
        private readonly ISessionHelper _sessionHelper;
        /// <param name = "httpContextAccessor" ></ param >
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>

        public NotificationController(
            INotificationService notificationService,
            ISessionHelper sessionHelper,
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _notificationService = notificationService;
            _sessionHelper = sessionHelper;
        }

        /// <summary>
        /// Controller for index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var data = _notificationService.RetrieveAll().Where(u => u.RecipientId == _sessionHelper.GetUserIdFromSession().ToString());
            return View(data);
        }
    }
}
