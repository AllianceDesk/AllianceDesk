using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    public class AdminController : ControllerBase<AdminController>
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public AdminController(IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IUserService userService,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._userService = userService;
        }

        /// <summary>
        /// Returns User View
        /// </summary>
        /// <returns> Home View </returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ViewUser()
        {
            ViewBag.IsLoginOrRegister = false;
            ViewBag.AdminSidebar = "ViewUser";
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

        /// <summary>
        /// Returns Tickets View.
        /// </summary>
        /// <returns> Tickets View </returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("TicketsAll/{id?}")]
        public ActionResult TicketsAll(string? id)
        {
            ViewBag.IsLoginOrRegister = false;
            ViewBag.AdminSidebar = "Tickets";

            if (id != null)
            {
                // Handle the case where an ID is provided
                ViewBag.TicketId = id;
                return this.View("/Views/Admin/TicketDetail.cshtml");
            }
            else
            {
                // Handle the case where no ID is provided
                return this.View("/Views/Admin/TicketsAll.cshtml");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TicketAssignment()
        {
            ViewBag.AdminSidebar = "Tickets";
            return this.View();
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult TicketReassignment()
        {
            ViewBag.AdminSidebar = "Tickets";
            return this.View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TicketResolved()
        {
            ViewBag.AdminSidebar = "Tickets";
            return this.View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TicketOccupied()
        {
            ViewBag.AdminSidebar = "Tickets";
            return this.View();
        }

        [HttpGet]
        [Route("UserDetails")]
        /// <summary>
        /// Go to the User Details View
        /// </summary>
        /// <returns> User Details</returns>
        /// 
        public IActionResult UserDetails(string UserId)
        {
            var data = _userService.GetUsers().Where(x => x.UserId.ToString() == UserId).FirstOrDefault();
            var team = _userService.GetTeams().Where(t => t.TeamId.Equals(data.TeamId)).FirstOrDefault();

            var userModel = new UserViewModel
            {
                Name = data.Name,
                Email = data.Email,
                RoleId = data.RoleId,
                TeamName = team.TeamName,
            };

            return PartialView("UserDetails", userModel);
        }

        [HttpGet("/AddUser")]
        /// <summary>
        /// Go to the Add a User View
        /// </summary>
        /// <returns> Add User</returns>
        public IActionResult AddUser()
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

            // Pass data to ViewBag
            ViewBag.Teams = new SelectList(teams, "Value", "Text");
            ViewBag.UserRoles = new SelectList(userRoles, "Value", "Text");

            return PartialView("AddUser");
        }

        [HttpPost]
        [Route("AddUser")]
        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> View User </returns>
        public IActionResult PostUserAdd(UserViewModel user)
        {
            _userService.AddUser(user);

            return RedirectToAction("ViewUser");
        }

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

            return View();
        }

        [HttpGet("/AddTeam")]
        /// <summary>
        /// Go to the Add a User View
        /// </summary>
        /// <returns> Add User</returns>
        public IActionResult AddTeam()
        {
            return PartialView("AddTeam");
        }

        [HttpPost("/AddTeam")]
        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> View User </returns>
        public IActionResult PostTeamAdd(UserViewModel team)
        {
            _userService.AddTeam(team);

            return RedirectToAction("ViewTeams");
        }
    }
}
