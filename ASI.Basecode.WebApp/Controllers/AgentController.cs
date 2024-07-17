using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using ASI.Basecode.Data.Models;
using Microsoft.AspNetCore.Authorization;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("Agent")]
    public class AgentController : ControllerBase<AgentController>
    {
        private readonly IUserService _userService;
        private readonly ITicketService _ticketService;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public AgentController(IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IUserService userService,
                              ITicketService ticketService,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._userService = userService;
            this._ticketService = ticketService;
        }


        [HttpGet("Dashboard")]
        [AllowAnonymous]
        public ActionResult Dashboard()
        { 
           return this.View();  
        }

        [HttpGet("AssignedTickets")]
        [AllowAnonymous]
        public ActionResult AssignedTickets()
        {
            return this.View();
        }

        [HttpGet("TicketDetail")]
        [AllowAnonymous]
        public ActionResult TicketDetail()
        {
            return this.View();
        }

        [HttpGet("TicketAssignment")]
        [AllowAnonymous]
        public ActionResult TicketAssignment()
        {
            return this.View();
        }
    }
}
