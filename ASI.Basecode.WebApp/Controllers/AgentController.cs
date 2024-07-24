﻿using ASI.Basecode.Services.Interfaces;
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
        public ActionResult Dashboard(string? status, string? searchTerm, string? sortOrder, int? page)
        {
            Guid guid = _sessionHelper.GetUserIdFromSession();
            var userRole = _userService.GetUserById(guid.ToString()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            var currentStatus = status ?? "Unresolved";
            var tickets = _ticketService.GetAgentTickets(guid, currentStatus, searchTerm, sortOrder, page);

            var pageSize = 10;
            var currentPage = page ?? 1;
            var currentSearchTerm = searchTerm ?? "";

            var count = tickets.Count();
            var agents = _userService.GetAgents();

            if (Math.Ceiling(tickets.Count() / (double)pageSize) > 1)
            {
                tickets = tickets.Skip((currentPage - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList()
                                 .AsQueryable();
            }

            // Create view model and return view
            var model = new AgentDashboardViewModel
            {
                Tickets = tickets,
                CurrentPage = currentPage,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                CurrentStatus = currentStatus,
                CurrentSearchTerm = currentSearchTerm,
            };

            ViewBag.AgentSidebar = "Overview";
            return View(model);
        }

        [HttpGet("AssignedTickets")]
        public ActionResult AssignedTickets()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession().ToString()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            ViewBag.AgentSidebar = "Tickets";
            return this.View();
        }

        [HttpGet("TicketDetail")]
        public ActionResult TicketDetail()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession().ToString()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            return this.View();
        }

        [HttpGet("TicketAssignment")]
        public ActionResult TicketAssignment()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession().ToString()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            return this.View();
        }

        [HttpGet("AgentProfile")]
        public ActionResult AgentProfile()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession().ToString()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

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

            return View(userModel);
        }

        [HttpPost("/AgentProfileEdit")]
        [AllowAnonymous]
        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> View User </returns>
        public IActionResult AgentProfileEdit(UserViewModel user)
        {
            if (user == null)
            {
                return NotFound();
            }
            _userService.UpdateUser(user);

            return RedirectToAction("Teams");
        }


        [HttpGet("PerformanceReport")]
        public ActionResult PerformanceReport()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession().ToString()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

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

            return View(userModel);
        }

        [HttpGet("Teams")]
        public ActionResult Teams()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession().ToString()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            ViewBag.IsLoginOrRegister = false;
            ViewBag.AgentSidebar = "ViewUser";
            var teamId = _userService.GetUserById(_sessionHelper.GetUserIdFromSession().ToString()).TeamId;

            var agents = _userService.GetAgents()
                                        .Where(t => t.TeamId == teamId.ToString())
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
                Users = agents
            };

            if (teamId == null)
            {
                viewModel = null;
            }

            return View(viewModel);
        }

        [HttpGet("/AddUserAgent")]
        public IActionResult AddUserAgent()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession().ToString()).RoleId;

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


        [HttpPost]
        [Route("AddUserAgent")]
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
        public ActionResult TicketSummary()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession().ToString()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            ViewBag.AgentSidebar = "Analytics";
            return this.View();
        }

        [HttpGet("ViewTeams")]
        public IActionResult ViewTeams()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession().ToString()).RoleId;

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
            ViewBag.AgentSidebar = "ViewUser";
            return View();
        }

        [HttpPost("/AddTeamAgent")]
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
        public IActionResult AgentDetails(string UserId)
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession().ToString()).RoleId;

            if (userRole != 2)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            var data = _userService.GetUserById(UserId);
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
                TeamName = _userService.GetTeams().FirstOrDefault(t => t.TeamId == data.TeamId.ToString())?.TeamName,
                RecentUserActivities = _userService.GetUserActivity(UserId),
            };

            return PartialView("AgentDetails", userModel);
        }

        [HttpGet("/AgentEdit")]
        /// <summary>
        /// Go to the User Details View
        /// </summary>
        /// <returns> User Details</returns>
        /// 
        public IActionResult AgentEdit(string UserId)
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession().ToString()).RoleId;

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
