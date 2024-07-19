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
using ASI.Basecode.Services.Manager;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("Agent")]
    public class AgentController : ControllerBase<AgentController>
    {
        private readonly IUserService _userService;
        private readonly ITicketService _ticketService;
        private readonly ISessionHelper _sessionHelper;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public AgentController(IHttpContextAccessor httpContextAccessor,
                            ISessionHelper sessionHelper,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IUserService userService,
                              ITicketService ticketService,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._userService = userService;
            this._ticketService = ticketService;
            this._sessionHelper = sessionHelper;
        }


        [HttpGet("Dashboard")]
        [AllowAnonymous]
        public ActionResult Dashboard()
        {
            ViewBag.AgentSidebar = "Overview";
            return this.View();  
        }

        [HttpGet("AssignedTickets")]
        [AllowAnonymous]
        public ActionResult AssignedTickets()
        {
            ViewBag.AgentSidebar = "Tickets";
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

        [HttpGet("AgentProfile")]
        [AllowAnonymous]
        public ActionResult AgentProfile()
        {
 
            return this.View();
        }

        [HttpGet("PerformanceReport")]
        [AllowAnonymous]
        public ActionResult PerformanceReport()
        {
            return this.View();
        }

        [HttpGet("Teams")]
        [AllowAnonymous]
        public ActionResult Teams()
        {
            ViewBag.IsLoginOrRegister = false;
            ViewBag.AgentSidebar = "ViewUser";
            var users = _userService.GetUsers()
                                        .Select(u => new UserViewModel
                                        {
                                            Name = u.Name,
                                            Email = u.Email,
                                            RoleId = u.RoleId,
                                            UserId = u.UserId.ToString(),
                                        })
                                        .ToList();

            var viewModel = new UserViewModel
            {
                Users = users
            };

            return View(viewModel);
        }

        [HttpGet("/AddUserAgent")]
        [AllowAnonymous]
        public IActionResult AddUserAgent()
        {
            var teams = _userService.GetTeams()
                                    .Select(t => new SelectListItem
                                    {
                                        Value = t.TeamId.ToString(),
                                        Text = t.TeamName
                                    })
                                    .ToList();

            var userRoles = _userService.GetUserRoles()
                                    .Select(u => new SelectListItem
                                    {
                                        Value = u.RoleId.ToString(),
                                        Text = u.RoleName
                                    })
                                    .ToList();

            ViewBag.Teams = teams;
            ViewBag.UserRoles = userRoles;

            return PartialView("AddUserAgent");
        }


        [HttpPost]
        [Route("AddUserAgent")]
        [AllowAnonymous]
        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> View User </returns>
        public IActionResult PostUserAddAgent(UserViewModel user)
        {
            _userService.AddUser(user);

            return RedirectToAction("Teams");
        }

        [HttpGet("TicketSummary")]
        [AllowAnonymous]
        public ActionResult TicketSummary()
        {
            ViewBag.AgentSidebar = "Analytics";
            return this.View();
        }

        [HttpGet("ViewTeams")]
        [AllowAnonymous]
        public IActionResult ViewTeams()
        {
            ViewBag.IsLoginOrRegister = false;
            var teams = _userService.GetTeams()
                                   .Select(t => new SelectListItem
                                   {
                                       Value = t.TeamId.ToString(),
                                       Text = t.TeamName
                                   })
                                   .ToList();
            ViewBag.Teams = new SelectList(teams, "Value", "Text");
            ViewBag.AgentSidebar = "ViewUser";
            return View();
        }

        [HttpPost("/AddTeamAgent")]
        [AllowAnonymous]
        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> View User </returns>
        public IActionResult PostTeamAddAgent(UserViewModel team)
        {
            _userService.AddTeam(team);

            return RedirectToAction("Teams");
        }


        [HttpGet("/AgentDetails")]
        [AllowAnonymous]
        public IActionResult AgentDetails(string UserId)
        {
            var data = _userService.GetUsers().FirstOrDefault(x => x.UserId.ToString() == UserId);
            if (data == null)
            {
                return NotFound(); 
            }
            var team = _userService.GetTeams().FirstOrDefault(t => t.TeamId.Equals(data.TeamId));
            var userModel = new UserViewModel
            {
                UserId = UserId,
                Name = data.Name,
                Email = data.Email,
                RoleId = data.RoleId,
                TeamName = team?.TeamName 
            };

            return PartialView("AgentDetails", userModel);
        }

        [HttpGet("/AgentEdit")]
        [AllowAnonymous]
        /// <summary>
        /// Go to the User Details View
        /// </summary>
        /// <returns> User Details</returns>
        /// 
        public IActionResult AgentEdit(string UserId)
        {
            var user = _userService.GetUsers().FirstOrDefault(x => x.UserId.ToString() == UserId);
            if (user == null)
            {
                return NotFound();
            }

            var userModel = new UserViewModel
            {
                UserId = user.UserId.ToString(),
                UserName = user.Username,
                Name = user.Name,
                Email = user.Email,
                Password = PasswordManager.DecryptPassword(user.Password),
                RoleId = user.RoleId,
                TeamId = user.TeamId.ToString(),
                RoleName = _userService.GetUserRoles().FirstOrDefault(r => r.RoleId == user.RoleId)?.RoleName,
                TeamName = _userService.GetTeams().FirstOrDefault(t => t.TeamId == user.TeamId.ToString())?.TeamName
            };

            var teams = _userService.GetTeams()
                                   .Select(t => new SelectListItem
                                   {
                                       Value = t.TeamId.ToString(),
                                       Text = t.TeamName
                                   })
                                   .ToList();

            var userRoles = _userService.GetUserRoles()
                                   .Select(u => new SelectListItem
                                   {
                                       Value = u.RoleId.ToString(),
                                       Text = u.RoleName
                                   })
                                   .ToList();

            // Pass data to ViewBag
            ViewBag.Teams = new SelectList(teams, "Value", "Text", userModel.TeamId);
            ViewBag.UserRoles = new SelectList(userRoles, "Value", "Text", userModel.RoleId);

            return PartialView("AgentEdit", userModel);
        }

        [HttpPost("/AgentEdit")]
        [AllowAnonymous]
        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> View User </returns>
        public IActionResult PostUserEdit(UserViewModel user)
        {
            _userService.UpdateUser(user);

            return RedirectToAction("Teams");
        }


        [HttpPost("/AgentDelete")]
        [AllowAnonymous]
        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> View User </returns>
        public IActionResult AgentDelete(string UserId)
        {
            _userService.DeleteUser(UserId);

            return RedirectToAction("Teams");
        }
    }
}
