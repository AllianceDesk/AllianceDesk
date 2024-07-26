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
using static ASI.Basecode.Resources.Constants.Enums;
using System.Collections.Generic;
using ASI.Basecode.Data.Repositories;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("Agent")]
    public class AgentController : ControllerBase<AgentController>
    {
        private readonly IUserService _userService;
        private readonly ITicketService _ticketService;
        private readonly INotificationService _notificationService;
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
                              INotificationService notificationService,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._userService = userService;
            this._ticketService = ticketService;
            this._notificationService = notificationService;
            this._sessionHelper = sessionHelper;
        }

        #region Agent's Navigation
        /// <summary>
        /// Goes to the Agent's Dashboard
        /// </summary>
        /// <param name="status">Resolved or Unresolved/param>
        /// <param name="searchTerm">Search Term on Search Bar</param>
        /// <param name="page">Current page</param>
        /// <returns></returns>
        [HttpGet("Dashboard")]
        public ActionResult Dashboard(string? status, string? searchTerm, int? page)
        {
            Guid userId = _sessionHelper.GetUserIdFromSession();
            var userRole = _userService.GetUserById(userId).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            var currentStatus = status ?? "All";
            var pageSize = 10;
            var currentPage = page ?? 1;
            var currentSearchTerm = searchTerm ?? "";

            // Fetch tickets from repository
            IEnumerable<TicketViewModel> tickets = _ticketService.GetAgentTickets(userId);

            if (currentStatus == "Unresolved")
            {
                tickets = tickets.Where(ticket => ticket.StatusId == 2);
            }
            else if (currentStatus == "Resolved")
            {
                tickets = tickets.Where(ticket => ticket.StatusId == 3 || ticket.StatusId == 4);
            }

            if (!String.IsNullOrEmpty(searchTerm))
            {
                tickets = tickets.Where(t => t.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Apply pagination
            var totalCount = tickets.Count();
            if (Math.Ceiling(tickets.Count() / (double)pageSize) > 1)
            {
                tickets = tickets.Skip((currentPage - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList()
                                 .AsQueryable();
            }

            tickets = tickets.ToList();

            var model = new AgentDashboardViewModel
            {
                Tickets = tickets,
                CurrentPage = currentPage,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                CurrentStatus = currentStatus,
                CurrentSearchTerm = currentSearchTerm,
            };

            ViewData["AgentSidebar"] = "Overview";
            return View(model);
        }

        /// <summary>
        /// Goes to the Agents profile.
        /// </summary>
        /// <returns></returns>
        [HttpGet("AgentProfile")]
        public ActionResult AgentProfile()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _userService.GetUserById(Guid.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = _userService.GetUserRoles()
                                        .Select(u => new SelectListItem
                                        {
                                            Value = u.RoleId.ToString(),
                                            Text = u.RoleName
                                        })
                                        .ToList();

            ViewBag.UserRoles = userRoles;

            var userModel = new UserViewModel
            {
                UserId = user.UserId,
                UserName = user.Username,
                Name = user.Name,
                Email = user.Email,
                Password = PasswordManager.DecryptPassword(user.Password),
                RoleId = user.RoleId,
                TeamId = user.TeamId,
                RoleName = _userService.GetUserRoles().FirstOrDefault(r => r.RoleId == user.RoleId)?.RoleName,
                TeamName = _userService.GetTeams().FirstOrDefault(t => t.TeamId == user.TeamId)?.TeamName
            };

            return View(userModel);
        }

        /// <summary>
        /// Post request for editing the agent's profile.
        /// </summary>
        /// <returns> Returns to the Agent's Teams Page </returns>
        [HttpPost("/AgentProfileEdit")]
        public IActionResult AgentProfileEdit(UserViewModel user)
        {
            if (user == null)
            {
                return NotFound();
            }
            _userService.UpdateUser(user);

            return RedirectToAction("Teams");
        }
        #endregion

        #region Ticket
        /// <summary>
        /// Gets the Ticket Details of the Ticket
        /// </summary>
        /// <param name="id">Ticket Id</param>
        /// <returns></returns>
        [HttpGet("Tickets/{id}")]
        public IActionResult Ticket(string id)
        {
            var ticket = _ticketService.GetById(Guid.Parse(id));

            if (ticket == null)
            {
                return NotFound(); // Handle ticket not found scenario
            }

            ViewData["AgentSidebar"] = "Tickets";

            return Json(new
            {
                user = ticket.CreatorName,
                title = ticket.Title,
                description = ticket.Description,
                dateCreated = ticket.DateCreated.ToString("MM dd yyyy hh:mm tt"),
                files = ticket.AttachmentStrings
            });
        }

        /// <summary>
        /// Goes to the Assigned Tickets Page
        /// </summary>
        /// <returns>Returns the Tickets Assigned of the Agent who is logged in</returns>
        [HttpGet("AssignedTickets")]
        public ActionResult AssignedTickets()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            ViewData["AgentSidebar"] = "Tickets";

            return this.View();
        }

        /// <summary>
        /// Goes to the Ticket Details Page
        /// </summary>
        /// <returns>Returns the Details of the Ticket</returns>
        [HttpGet("TicketDetail")]
        public ActionResult TicketDetail()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            ViewData["AgentSidebar"] = "Tickets";

            return this.View();
        }

        /// <summary>
        /// Goes to the Ticket Assignment Page
        /// </summary>
        /// <returns></returns>
        [HttpGet("TicketAssignment")]
        public ActionResult TicketAssignment()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            return this.View();
        }

        /// <summary>
        /// Resolves the ticket.
        /// </summary>
        /// <param name="ticketMessage">The ticket message.</param>
        /// <returns></returns>
        [HttpPost("ResolveTicket")]
        public IActionResult ResolveTicket(TicketMessageViewModel ticketMessage)
        {
            DateTime resolved = DateTime.Now;
            ticketMessage.PostedAt = resolved;
            var ticket = _ticketService.GetById(ticketMessage.TicketId);


            if (ticket == null)
            {
                return NotFound();
            }

            _ticketService.UpdateStatus(ticket.TicketId, 3);

            // Add Ticket Message
            _ticketService.AddMessage(ticketMessage);

            // Add Ticket Activity
            TicketActivityViewModel ticketActivity = new TicketActivityViewModel
            {
                TicketId = ticketMessage.TicketId,
                UserId = _sessionHelper.GetUserIdFromSession(),
                ModifiedAt = resolved,
                OperationId = 7,
                Message = $"Agent {ticket.AgentName} resolved the ticket"
            };

            _ticketService.AddActivity(ticketActivity);

            // Create Notification
            _notificationService.Add(ticket.TicketId.ToString(), ticket.CreatorId.ToString());

            return RedirectToAction("Dashboard");
        }

        #endregion

        #region User
        [HttpGet("Teams")]
        public ActionResult Teams(string searchString)
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            var teamId = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).TeamId;

            var agents = _userService.GetAgents()
                                        .Where(t => t.TeamId == teamId)
                                        .Select(u => new UserViewModel
                                        {
                                            Name = u.Name,
                                            Email = u.Email,
                                            RoleId = u.RoleId,
                                            UserId = u.UserId,
                                        });


            if (!String.IsNullOrEmpty(searchString))
            {
                agents = agents.Where(u => u.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            var viewModel = new UserViewModel
            {
                Users = agents.ToList()
            };

            ViewData["Search String"] = searchString;
            ViewData["AgentSidebar"] = "ViewUser";

            return View(viewModel);
        }
        /// <summary>
        /// Goes to the Add User Page
        /// </summary>
        /// <returns>Returns the Add Agent View</returns>
        [HttpGet("/AddUserAgent")]
        public IActionResult AddUserAgent()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

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

        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> Redirects to the Teams Page</returns>
        [HttpPost]
        [Route("AddUserAgent")]
        public IActionResult PostUserAddAgent(UserViewModel user)
        {
            _userService.AddUser(user);

            return RedirectToAction("Teams");
        }


        /// <summary>
        /// Goes to the Teams Page
        /// </summary>
        /// <returns></returns>
        [HttpGet("ViewTeams")]
        public IActionResult ViewTeams()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            ViewBag.IsLoginOrRegister = false;
            var teams = _userService.GetTeams()
                                   .Select(t => new SelectListItem
                                   {
                                       Value = t.TeamId.ToString(),
                                       Text = t.TeamName
                                   })
                                   .ToList();
            ViewBag.Teams = new SelectList(teams, "Value", "Text");

            ViewData["AgentSidebar"] = "ViewUser";
            return View();
        }


        /// <summary>
        /// Post Request for Adding a Team
        /// </summary>
        /// <returns> View User </returns>
        [HttpPost("/AddTeamAgent")]
        public IActionResult PostTeamAddAgent(UserViewModel team)
        {
            _userService.AddTeam(team);

            return RedirectToAction("Teams");
        }

        /// <summary>
        /// Goes to the Agent Details Page
        /// </summary>
        /// <param name="UserId">UserId of the Agents we want to view the details</param>
        /// <returns></returns>
        [HttpGet("/AgentDetails")]
        public IActionResult AgentDetails(string UserId)
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            Guid agentId = Guid.Parse(UserId);

            var user = _userService.GetUserById(agentId);

            if (user == null)
            {
                return NotFound();
            }

            var team = _userService.GetTeams().FirstOrDefault(t => t.TeamId.Equals(user.TeamId));
            var userModel = new UserViewModel
            {
                UserId = agentId,
                Name = user.Name,
                Email = user.Email,
                RoleId = user.RoleId,
                TeamName = _userService.GetTeams().FirstOrDefault(t => t.TeamId == user.TeamId)?.TeamName,
                RecentUserActivities = _userService.GetUserActivity(agentId),
            };

            return PartialView("AgentDetails", userModel);
        }

        /// <summary>
        /// Goes to the Edit Agent Pagee
        /// </summary>
        /// <param name="UserId">UserId of the Agents we want to edit</param>
        /// <returns></returns>
        [HttpGet("/AgentEdit")]
        public IActionResult AgentEdit(string UserId)
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            var user = _userService.GetUsers().FirstOrDefault(x => x.UserId.ToString() == UserId);
            if (user == null)
            {
                return NotFound();
            }

            var userModel = new UserViewModel
            {
                UserId = user.UserId,
                UserName = user.Username,
                Name = user.Name,
                Email = user.Email,
                Password = PasswordManager.DecryptPassword(user.Password),
                RoleId = user.RoleId,
                TeamId = user.TeamId,
                RoleName = _userService.GetUserRoles().FirstOrDefault(r => r.RoleId == user.RoleId)?.RoleName,
                TeamName = _userService.GetTeams().FirstOrDefault(t => t.TeamId == user.TeamId)?.TeamName
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

        /// <summary>
        /// Post Request for Editing the Agent
        /// </summary>
        /// <returns> Redirects to the Team's Page</returns>
        [HttpPost("/AgentEdit")]
        public IActionResult PostUserEdit(UserViewModel user)
        {
            _userService.UpdateUser(user);

            return RedirectToAction("Teams");
        }

        /// <summary>
        /// Post Request for Deleting the Agent
        /// </summary>
        /// <returns> Redirects to the Team's Page</returns>
        [HttpPost("/AgentDelete")]
        public IActionResult AgentDelete(string UserId)
        {
            _userService.DeleteUser(Guid.Parse(UserId));

            return RedirectToAction("Teams");
        }
        #endregion

        #region Analytics

        /// <summary>
        /// Goes to the Ticket Summary Page
        /// </summary>
        /// <returns></returns>
        [HttpGet("TicketSummary")]
        public ActionResult TicketSummary()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            ViewData["AgentSidebar"] = "Analytics";
            return this.View();
        }

        /// <summary>
        /// Goes to the Performance Report Page
        /// </summary>
        /// <returns></returns>
        [HttpGet("PerformanceReport")]
        public ActionResult PerformanceReport()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _userService.GetUserById(Guid.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = _userService.GetUserRoles()
                                        .Select(u => new SelectListItem
                                        {
                                            Value = u.RoleId.ToString(),
                                            Text = u.RoleName
                                        })
                                        .ToList();

            ViewData["UserRoles"] = userRoles;

            var userModel = new UserViewModel
            {
                UserId = user.UserId,
                UserName = user.Username,
                Name = user.Name,
                Email = user.Email,
                Password = PasswordManager.DecryptPassword(user.Password),
                RoleId = user.RoleId,
                TeamId = user.TeamId,
                RoleName = _userService.GetUserRoles().FirstOrDefault(r => r.RoleId == user.RoleId)?.RoleName,
                TeamName = _userService.GetTeams().FirstOrDefault(t => t.TeamId == user.TeamId)?.TeamName
            };

            return View(userModel);
        }

        #endregion
    }
}
