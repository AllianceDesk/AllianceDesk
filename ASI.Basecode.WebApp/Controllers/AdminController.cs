using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("Admin")]
    public class AdminController : ControllerBase<AdminController>
    {
        private readonly IUserService _userService;
        private readonly ITicketService _ticketService;
        private readonly ISessionHelper _sessionHelper;
        private readonly IArticleService _articleService;
        private readonly ITeamService _teamService;
        
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
                              ITicketService ticketService,
                              ISessionHelper sessionHelper,
                              IArticleService articleService,
                              ITeamService teamService,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._userService = userService;
            this._ticketService = ticketService;
            this._sessionHelper = sessionHelper;
            this._articleService = articleService;
            this._teamService = teamService;
        }

        /// <summary>
        /// Admin Dashboard
        /// </summary>
        /// <returns>Returns Admin DashboardView Model</returns>
        [HttpGet("/Dashboard")]
        public ActionResult Dashboard()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 1)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            ViewBag.Name = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).Name;
            ViewBag.AdminSidebar = "Overview";
            var model = new AdminDashboardViewModel
            {
                TicketCountsByDay = _ticketService.GetTicketVolume(),
                TopAgents = _ticketService.GetWeeklyTopResolvers(),
                FavoriteArticles = _articleService.RetrieveFavorites(),
            };
            return this.View(model);
        }

        #region Tickets

        /// <param name="id">Ticket Id</param>
        /// <param name="status">Resolved or Unresolved</param>
        /// <returns>
        /// Tickets when id is null and a specific ticket when id is provided
        /// </returns>
        [HttpGet]
        [Route("Tickets/{id?}")]
        public IActionResult Tickets(string? id, string? status)
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 1)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            ViewBag.IsLoginOrRegister = false;
            ViewBag.AdminSidebar = "Tickets";

            // Retrieve all tickets once
            var allTickets = _ticketService.GetAllTickets();

            // Handle the case where status is provided
            if (status != null)
            {
                ViewBag.ShowStatus = status;

                IEnumerable<TicketViewModel> filteredTickets = status switch
                {
                    "Resolved" => allTickets.Where(t => t.StatusId == 4 || t.StatusId == 5),
                    _ => allTickets.Where(t => t.StatusId == 1 || t.StatusId == 2 || t.StatusId == 3)
                };

                filteredTickets = filteredTickets.OrderByDescending(t => t.DateCreated);
                return View("/Views/Admin/Tickets.cshtml", filteredTickets);
            }

            // Handle the case where id is provided
            if (id != null)
            {
                Guid ticketId = Guid.Parse(id);

                var ticket = _ticketService.GetById(ticketId);
                return View("/Views/Admin/TicketDetail.cshtml", ticket);
            }

            // Handle the case where no id and status are provided
            var tickets = allTickets.OrderByDescending(t => t.DateCreated);
            return View("/Views/Admin/Tickets.cshtml", tickets);
        }

        /// <summary>
        /// Returns a list of agents except the current agent if the ticket was already assigend to an agent.
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        [HttpGet("Tickets/Assignment/{id}")]
        public IActionResult TicketAssignment(string id)
        {
            Guid ticketId = Guid.Parse(id);
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 1)
            {
                return RedirectToAction("Index", "AccessDenied");
            }
            
            var ticket = _ticketService.GetById(ticketId);
            var agents = _userService.GetAgents().ToList();
            var teams = _userService.GetTeams().ToDictionary(u => u.TeamId, u => u.TeamName);
            Dictionary<Guid, int> ticketCount = new Dictionary<Guid, int>();

            if (ticket.AgentId != null)
            {
                agents = agents.Where(agent => agent.UserId != ticket.AgentId).ToList();
            }

            foreach (var agent in agents)
            {

                if (agent.TeamId.HasValue)
                {
                    agent.TeamName = teams.TryGetValue(agent.TeamId.Value, out var teamName) ? teamName : null;
                }

                //Get the ticket counts for each agent that is still not resolved
                var agentTickets = _ticketService.GetAgentTickets(agent.UserId)
                    .ToList()
                    .Where(a => a.StatusId != 5 && a.StatusId != 4);
                
                ticketCount.Add(agent.UserId, agentTickets.Count());
            }

            var model = new AgentAssignmentViewModel
            {
                TicketId = ticket.TicketId,
                TicketNumber = ticket.TicketNumber,
                Title = ticket.Title,
                CreatedAt = ticket.DateCreated,
                Description = ticket.Description,
                Agents = agents,
                TicketCount = ticketCount
            };

            ViewBag.AdminSidebar = "Tickets";
            return View(model);
        }


        /// <summary>
        /// Assigns the agent to the ticket.
        /// </summary>
        /// <param name="model">Agent Assignment View Model</param>
        /// <returns></returns>
        [HttpPost("Tickets/Assignment"), ActionName("TicketAssignment")]
        public IActionResult PostTicketAssignment([FromBody] AgentAssignmentViewModel model)
        {
            _ticketService.AssignAgent(model.TicketId, model.SelectedAgentId);
            return RedirectToAction("Tickets", new { id = model.TicketId });
        }

        /// <summary>
        /// Updates the priority of the ticket
        /// </summary>
        /// <param name="ticket">The ticket to be modified</param>
        /// <returns></returns>
        [HttpPost("Tickets/{id}/UpdatePriority")]
        public IActionResult UpdatePriority(TicketViewModel ticket)
        {
            var existingTicket = _ticketService.GetById(ticket.TicketId);

            existingTicket.PriorityId = ticket.PriorityId;

            if (existingTicket != null)
            {
                _ticketService.Update(existingTicket);

                return RedirectToAction("Tickets", new { id = ticket.TicketId });
            }

            ViewBag.ErrorMessage = "Update failed";
            return View(ticket);
        }
        #endregion

        #region Users

        /// <summary>
        /// Go to the User Directory
        /// </summary>
        /// <returns> Returns all the user </returns>
        [HttpGet("ViewUser")]
        public ActionResult ViewUser(string searchString)
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 1)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            ViewBag.IsLoginOrRegister = false;
            ViewBag.AdminSidebar = "ViewUser";
            ViewBag.SelectRoles = new List<string> { "All User", "User", "Agent", "Admin" };
            var users = _userService.GetAllUsers()
                                        .Select(u => new UserViewModel
                                        {
                                            Name = u.Name,
                                            Email = u.Email,
                                            RoleId = u.RoleId,
                                            UserId = u.UserId,
                                        })
                                        .OrderBy(u => u.Name)
                                        .ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                users = _userService.GetAllUsers()
                                        .Where (u => u.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                                        .Select(u => new UserViewModel
                                        {
                                            Name = u.Name,
                                            Email = u.Email,
                                            RoleId = u.RoleId,
                                            UserId = u.UserId,
                                        })
                                        .OrderBy(u => u.Name)
                                        .ToList();
            }

            var viewModel = new UserViewModel
            {
                Users = users
            };
            ViewBag.SearchString = searchString;

            return View(viewModel);
        }

        /// <summary>
        /// Go to the User Details View
        /// </summary>
        /// <returns> User Details</returns>
        /// 
        [HttpGet("/UserDetails")]
        public IActionResult UserDetails(string UserId)
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 1)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            var data = _userService.GetAllUsers().FirstOrDefault(x => x.UserId.ToString() == UserId);
            if (data == null)
            {
                return NotFound(); 
            }

            var team = _userService.GetTeams().FirstOrDefault(t => t.TeamId.Equals(data.TeamId));
            var userModel = new UserViewModel
            {
                UserId = Guid.Parse(UserId),
                Name = data.Name,
                Email = data.Email,
                RoleId = data.RoleId,
                TeamName = _userService.GetTeams().FirstOrDefault(t => t.TeamId == data.TeamId.ToString())?.TeamName,
                RecentUserActivities = _userService.GetUserActivity(UserId),
            };

            return PartialView("UserDetails", userModel);
        }


        /// <summary>
        /// Go to the Add a User View
        /// </summary>
        /// <returns> Add User</returns>
        [HttpGet("/AddUser")]
        public IActionResult AddUser()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 1)
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

            // Pass data to ViewBag
            ViewBag.Teams = new SelectList(teams, "Value", "Text");
            ViewBag.UserRoles = new SelectList(userRoles, "Value", "Text");
            ViewBag.Password = _userService.GeneratePassword();

            return PartialView("AddUser");
        }


        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> View User </returns>
        [HttpPost("/AddUser")]
        public IActionResult PostUserAdd(UserViewModel user)
        {
            _userService.AddUser(user);

            return RedirectToAction("ViewUser");
        }


        /// <summary>
        /// Go to the User Edit View
        /// </summary>
        /// <returns> Returns to Edit User </returns>
        [HttpGet("/UserEdit")]
        public IActionResult UserEdit(string UserId, bool resetPassword)
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 1)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            // Fetch user data
            var user = _userService.GetAllUsers().FirstOrDefault(x => x.UserId.ToString() == UserId);
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
                RoleName = _userService.GetUserRoles().FirstOrDefault(r => r.RoleId == user.RoleId).RoleName,
                TeamName = _userService.GetTeams().FirstOrDefault(t => t.TeamId == user.TeamId)?.TeamName
            };

            if (resetPassword)
            {
                userModel.Password = _userService.GeneratePassword();
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

            // Pass data to ViewBag
            ViewBag.Teams = new SelectList(teams, "Value", "Text", userModel.TeamId);
            ViewBag.UserRoles = new SelectList(userRoles, "Value", "Text", userModel.RoleId);
            ViewBag.ResetPassword = false;

            return PartialView("UserEdit", userModel);
        }


        /// <summary>
        /// Post Request for Editing User
        /// </summary>
        /// <returns> Returns to View User </returns>
        [HttpPost("/UserEdit")]
        public IActionResult PostUserEdit(UserViewModel user)
        {
            _userService.UpdateUser(user);

            return RedirectToAction("ViewUser");
        }

        /// <summary>
        /// Post Request for Deleting User
        /// </summary>
        /// <returns> Returns to View User </returns>
        [HttpGet("/UserDelete")]
        public IActionResult UserDelete(string UserId)
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 1)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            var userToDelete = new UserViewModel
            {
                UserId = Guid.Parse(UserId),
            };

            return PartialView("UserDelete", userToDelete);
        }

        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> Returns to View User </returns>
        [HttpPost("/UserDelete")]
        public IActionResult PostUserDelete(string UserId)
        {
            _userService.DeleteUser(Guid.Parse(UserId));

            return RedirectToAction("ViewUser");
        }


        /// <summary>
        /// Go to the Teams View
        /// </summary>
        [HttpGet("/ViewTeams")]
        public IActionResult ViewTeams()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 1)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            var teams = _teamService.GetTeams()
                                        .Select(u => new TeamViewModel
                                        {
                                            TeamName = u.TeamName,
                                            TeamId = u.TeamId,
                                            TeamDescription = u.TeamDescription,
                                            DepartmentId = u.DepartmentId,
                                            DepartmentName = _teamService.GetDepartmentName(u.DepartmentId),
                                            TeamNumber = _teamService.GetTeamNumber(u.TeamId.ToString()),
                                        })
                                        .OrderBy(t => t.TeamName)
                                        .ToList();

            var viewModel = new TeamViewModel
            {
                Teams = teams
            };
            ViewBag.AdminSidebar = "ViewUser";

            return View(viewModel);
        }

        /// <summary>
        /// Go to the Team Details View
        /// </summary>
        /// <returns>Team Details</returns>
        /// 
        [HttpGet("/TeamDetail")]
        public IActionResult TeamDetail(string teamId)
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession().ToString()).RoleId;

            if (userRole != 1)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            var data = _userService.GetAllUsers()
                        .Where(x => x.TeamId.ToString() == teamId)
                        .Select (u => new UserViewModel{
                            Name = u.Name,
                            UserName = u.Username,
                            Email = u.Email,
                            RoleId = u.RoleId,
                        })
                        .ToList();
            var teamInfo = _teamService.GetTeams().Where(t => t.TeamId.ToString() == teamId).FirstOrDefault();
            if (data == null)
            {
                return NotFound();
            }
            var agentModel = new TeamViewModel
            {
                TeamDescription = teamInfo.TeamDescription,
                TeamName = teamInfo.TeamName,
                DepartmentName = _teamService.GetDepartmentName(teamInfo.DepartmentId),
                Agents = data,
                TeamNumber = data.Count(),
            };

            return PartialView("TeamDetail", agentModel);
        }

        /// <summary>
        /// Go to the Add Team View
        /// </summary>
        /// <returns>The Add Team View</returns>
        [HttpGet("/AddTeam")]
        public IActionResult AddTeam()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 1)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            var departments = _teamService.GetDepartments()
                                   .Select(u => new SelectListItem
                                   {
                                       Value = u.DepartmentId.ToString(),
                                       Text = u.DepartmentName
                                   })
                                   .ToList();

            // Pass data to ViewBag
            ViewBag.Departments = new SelectList(departments, "Value", "Text");

            return PartialView("AddTeam");
        }

        /// <summary>
        /// Post Request for Adding a Team
        /// </summary>
        /// <returns> Redirects to View Teams</returns>
        [HttpPost("/AddTeam")]
        public IActionResult PostTeamAdd(UserViewModel team)
        {
            _userService.AddTeam(team);

            return RedirectToAction("ViewTeams");
        }

        #endregion

        #region Analytics        
        /// <summary>
        /// Go to Overall Metrics on Analytics
        /// </summary>
        /// <returns>Analytics of the all the tickets</returns>
        [HttpGet("AnalyticsOverallMetrics")]
        public IActionResult AnalyticsOverallMetrics()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 1)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            var now = DateTime.Now;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday - 4).Date;
            var endOfWeek = now.Date.AddDays(1);

            ViewBag.AdminSidebar = "Analytics";
            var weeklyTickets = _ticketService.GetWeeklyTickets(startOfWeek, endOfWeek);

            var categories = _ticketService.GetCategories().ToList();
            var statuses = _ticketService.GetStatuses().ToList();
            var priorities = _ticketService.GetPriorities().ToList();

            // Count tickets by category
            var ticketCountsByCategory = categories.ToDictionary(
                category => category.CategoryName,
                category => weeklyTickets.Count(t => t.CategoryId == category.CategoryId)
            );

            // Count tickets by status
            var ticketCountsByStatus = statuses.ToDictionary(
                status => status.StatusName,
                status => weeklyTickets.Count(t => t.StatusId == status.StatusId)
            );

            var ticketCountsByPriority = priorities.ToDictionary(
                priority => priority.PriorityName,
                priority => weeklyTickets.Count(t => t.PriorityId == priority.PriorityId)
            );

            var model = new OverAllTicketCountViewModel
            {
                TicketCountsByCategory = ticketCountsByCategory,
                TicketCountsByStatus = ticketCountsByStatus,
                TicketCountsByPriority = ticketCountsByPriority,
                TicketCountsByDay = _ticketService.GetTicketVolume(),
                TotalTicketCount = weeklyTickets.Count(),
                RecentUserActivities = _userService.GetRecentUserActivity(),
            };


            return View("Views/Admin/AnalyticsOverallMetrics.cshtml", model);
        }

        /// <summary>
        /// Go to Agent Metrics on Analytics
        /// </summary>
        /// <returns>Analytics based on Agent</returns>
        [HttpGet("AnalyticsAgentMetric")]
        public IActionResult AgentMetric()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 1)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            ViewBag.AdminSidebar = "Analytics";

            var now = DateTime.Now;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday - 4).Date;
            var endOfWeek = now.Date.AddDays(1);

            var agents = _userService.GetAgents().ToList();
            var teams = _userService.GetTeams().ToList();

            var categories = _ticketService.GetCategories().ToList();
            var statuses = _ticketService.GetStatuses().ToList();
            var priorities = _ticketService.GetPriorities().ToList();

            var tickets = _ticketService.GetAllTickets();

            var ticketsByAgentQuery = tickets
                .Where(t => t.DateCreated >= startOfWeek && t.DateCreated < endOfWeek)
                .GroupBy(t => t.AgentId)
                .Select(g => new
                {
                    AgentId = g.Key,
                    Tickets = g.ToList()
                });

            // Materialize the query into a dictionary
            var ticketsByAgent = ticketsByAgentQuery
                .ToDictionary(
                    g => g.AgentId,
                    g => g.Tickets
                );


            var agentTicketCounts = agents.Select(agent =>
            {
                var agentTickets = ticketsByAgent.ContainsKey(agent.UserId)
                    ? ticketsByAgent[agent.UserId]
                    : new List<TicketViewModel>();

                // Count tickets by category
                var ticketCountsByCategory = categories.ToDictionary(
                    category => category.CategoryName,
                    category => agentTickets.Count(t => t.Category == category.CategoryName)
                );

                // Count tickets by status
                var ticketCountsByStatus = statuses.ToDictionary(
                    status => status.StatusName,
                    status => agentTickets.Count(t => t.Status == status.StatusName)
                );

                var ticketCountByPriority = priorities.ToDictionary(
                    priority => priority.PriorityName,
                    priority => agentTickets.Count(t => t.Priority == priority.PriorityName)
                );

                return new AnalyticsAgentMetricViewModel
                {
                    Agent = new UserViewModel
                    {
                        UserId = agent.UserId,
                        Name = agent.Name,
                    },

                    TicketCountsByCategory = ticketCountsByCategory,
                    TicketCountsByStatus = ticketCountsByStatus,
                    TicketCountsByPriority = ticketCountByPriority
                };
            });

            return View("Views/Admin/AnalyticsAgentMetric.cshtml", agentTicketCounts);
        }

        /// <summary>
        /// Go to Team Metrics on Analytics
        /// </summary>
        /// <returns>Analytics based on Teams</returns>
        [HttpGet("AnalyticsTeamMetric")]
        public IActionResult TeamMetrics()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 1)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            ViewBag.AdminSidebar = "Analytics";

            var now = DateTime.Now;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday - 4).Date;
            var endOfWeek = now.Date.AddDays(1);

            /*var weeklyTickets = _ticketService.GetWeeklyTickets(startOfWeek, endOfWeek)*/
            ;

            var agents = _userService.GetAgents().ToList();
            var teams = _userService.GetTeams().ToList();

            var categories = _ticketService.GetCategories().ToList();
            var statuses = _ticketService.GetStatuses().ToList();
            var priorities = _ticketService.GetPriorities().ToList();

            var tickets = _ticketService.GetAllTickets();

            var ticketsByAgentQuery = tickets
                .Where(t => t.DateCreated >= startOfWeek && t.DateCreated < endOfWeek)
                .GroupBy(t => t.AgentId)
                .Select(g => new
                {
                    AgentId = g.Key,
                    Tickets = g.ToList()
                });

            // Materialize the query into a dictionary
            var ticketsByAgent = ticketsByAgentQuery
                .ToDictionary(
                    g => g.AgentId,
                    g => g.Tickets
                );

            var agentTicketCounts = agents.Select(agent =>
            {
                var agentTickets = ticketsByAgent.ContainsKey(agent.UserId)
                    ? ticketsByAgent[agent.UserId]
                    : new List<TicketViewModel>();

                // Count tickets by category
                var ticketCountsByCategory = categories.ToDictionary(
                    category => category.CategoryName,
                    category => agentTickets.Count(t => t.CategoryId == category.CategoryId)
                );

                // Count tickets by status
                var ticketCountsByStatus = statuses.ToDictionary(
                    status => status.StatusName,
                    status => agentTickets.Count(t => t.StatusId == status.StatusId)
                );

                var ticketCountByPriority = priorities.ToDictionary(
                    priority => priority.PriorityName,
                    priority => agentTickets.Count(t => t.PriorityId == priority.PriorityId)
                );

                return new AnalyticsAgentMetricViewModel
                {
                    Agent = new UserViewModel
                    {
                        UserId = agent.UserId,
                        Name = agent.Name,
                    },

                    TicketCountsByCategory = ticketCountsByCategory,
                    TicketCountsByStatus = ticketCountsByStatus,
                    TicketCountsByPriority = ticketCountByPriority
                };
            }).ToList();

            var teamTicketCounts = teams.Select(team =>
            {
                var teamAgents = agents.Where(a => a.TeamId == team.TeamId).ToList();

                var teamTickets = teamAgents
                    .SelectMany(a => ticketsByAgent.ContainsKey(a.UserId) ? ticketsByAgent[a.UserId] : new List<TicketViewModel>())
                    .ToList();

                // Count tickets by category
                var ticketCountsByCategory = categories.ToDictionary(
                    category => category.CategoryName,
                    category => teamTickets.Count(t => t.Category == category.CategoryName)
                );

                // Count tickets by status
                var ticketCountsByStatus = statuses.ToDictionary(
                    status => status.StatusName,
                    status => teamTickets.Count(t => t.Status == status.StatusName)
                );

                var ticketCountByPriority = priorities.ToDictionary(
                    priority => priority.PriorityName,
                    priority => teamTickets.Count(t => t.Priority == priority.PriorityName)
                );

                return new AnalyticsTeamMetricViewModel
                {
                    Team = team,
                    TicketCountsByCategory = ticketCountsByCategory,
                    TicketCountsByStatus = ticketCountsByStatus,
                    TicketCountsByPriority = ticketCountByPriority
                };
            });

            return View("Views/Admin/AnalyticsTeamMetric.cshtml", teamTicketCounts);
        }
        #endregion
    }
}
