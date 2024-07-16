﻿using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ASI.Basecode.WebApp.Controllers
{
    public class UserController : ControllerBase<UserController>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public UserController(IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {

        }

        public IActionResult Index()
        {
            ViewBag.IsLoginOrRegister = false;
            return View();
        }

        public IActionResult OpenTickets()
        {
            ViewBag.IsLoginOrRegister = false;
            return View();
        }

        public IActionResult InProgress()
        {
            ViewBag.IsLoginOrRegister = false;
            return View();
        }

        public IActionResult Resolved()
        {
            ViewBag.IsLoginOrRegister = false;
            return View();
        }

        public IActionResult CloseTickets()
        {
            ViewBag.IsLoginOrRegister = false;
            return View();
        }

        public IActionResult UserLayout()
        {
            ViewBag.IsLoginOrRegister = false;
            return View();
        }



    }
}
