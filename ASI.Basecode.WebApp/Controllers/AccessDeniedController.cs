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
    public class AccessDeniedController : ControllerBase<AccessDeniedController>
    {
        private readonly ISessionHelper _sessionHelper;
        private readonly IUserService _userService;
        /// <param name = "httpContextAccessor" ></ param >
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>

        public AccessDeniedController(
            ISessionHelper sessionHelper,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _sessionHelper = sessionHelper;
            _userService = userService;
        }

        /// <summary>
        /// Controller for index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {

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

            ViewBag.RoleId = user.RoleId;

            return View();
        }
    }
}