﻿using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using ASI.Basecode.Data.Models;
using Newtonsoft.Json.Linq;
using System;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("Admin")]
    public class AdminController : ControllerBase<AdminController>
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
        public AdminController(IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IUserService userService,
                              ITicketService ticketService,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._userService = userService;
            this._ticketService = ticketService;
        }

        #region Users

        /// <summary>
        /// Returns User View
        /// </summary>
        /// <returns> Home View </returns>
        [HttpGet("Users")]
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


        [HttpGet("Users/{id}")]
        [AllowAnonymous]
        /// <summary>
        /// Go to the User Details View
        /// </summary>
        /// <returns> User Details</returns>
        /// 
        public IActionResult UserDetails(string id)
        {
            var data = _userService.GetUsers().Where(x => x.UserId.ToString() == id).FirstOrDefault();
            var team = _userService.GetTeams().Where(t => t.TeamId.Equals(data.TeamId)).FirstOrDefault();

            var userModel = new UserViewModel
            {
                UserId = id,
                Name = data.Name,
                Email = data.Email,
                RoleId = data.RoleId,
                TeamName = team?.TeamName // This will be null if team is null
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
        [Route("/AddUser")]
        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> View User </returns>
        public IActionResult PostUserAdd(UserViewModel user)
        {
            _userService.AddUser(user);

            return RedirectToAction("ViewUser");
        }

        [HttpGet]
        [Route("UserEdit")]
        /// <summary>
        /// Go to the User Details View
        /// </summary>
        /// <returns> User Details</returns>
        /// 
        public IActionResult UserEdit(string UserId)
        {
            // Fetch user data
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

            return PartialView("UserEdit", userModel);
        }

        [HttpPost]
        [Route("UserEdit")]
        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> View User </returns>
        public IActionResult PostUserEdit(UserViewModel user)
        {
            _userService.UpdateUser(user);

            return RedirectToAction("ViewUser");
        }

        [HttpGet]
        [Route("UserDelete")]
        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> View User </returns>
        public IActionResult UserDelete(string UserId)
        {
            var userToDelete = new UserViewModel
            {
                UserId = UserId,
            };

            return PartialView("UserDelete", userToDelete);
        }

        [HttpPost]
        [Route("UserDelete")]
        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> View User </returns>
        public IActionResult PostUserDelete(string UserId)
        {
            _userService.DeleteUser(UserId);

            return RedirectToAction("ViewUser");
        }
    
        [HttpGet]
        [Route("Teams")]
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

        #endregion

        #region Tickets
        /// <summary>
        /// Returns Tickets View.
        /// </summary>
        /// <returns> Tickets View </returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("Tickets/{id?}")]
        public ActionResult Tickets(string? id, string? status)
        {
            ViewBag.IsLoginOrRegister = false;
            ViewBag.AdminSidebar = "Tickets";

            if (status != null)
            {
                ViewBag.ShowStatus = status;

                if (status == "Resolved")
                {
                    var resolvedTickets = _ticketService.RetrieveAll()
                        .Where(t => t.StatusId == "4" || t.StatusId == "5")
                        .OrderByDescending(t => t.DateCreated);

                    return View("/Views/Admin/Tickets.cshtml", resolvedTickets);
                }

                var unresolvedTickets = _ticketService.RetrieveAll()
                    .Where(t => t.StatusId == "1" || t.StatusId == "2" || t.StatusId == "3")
                    .OrderByDescending(t => t.DateCreated);

                return View("/Views/Admin/Tickets.cshtml", unresolvedTickets);
            }

            if (id != null)
            {

                var ticket = _ticketService.RetrieveAll()
                    .Where(t => t.TicketId.ToString() == id)
                    .FirstOrDefault();
                
                Console.WriteLine(ticket);

                return this.View("/Views/Admin/TicketDetail.cshtml", ticket);
            }
            else
            {
                var tickets = _ticketService.RetrieveAll()
                    .OrderByDescending(t => t.DateCreated);

                // Handle the case where no ID is provided
                return this.View("/Views/Admin/Tickets.cshtml", tickets);
            }
        }

        [HttpGet("Tickets/Assignment")]
        [AllowAnonymous]
        public ActionResult TicketAssignment(string id)
        {
            var ticket = _ticketService.RetrieveAll()
                .Where(t => t.TicketId.ToString() == id)
                .FirstOrDefault();

            var tickets = _ticketService.RetrieveAll();

            // Retrieve all agents (assuming RoleId 2 corresponds to agents)

            var agents = _userService.GetAgents().ToList();

            foreach (var agent in agents)
            {
                if (agent.TeamId != null)
                {
                    agent.TeamName = _userService.GetTeams().Where(t => t.TeamId.ToString() == agent.TeamId).FirstOrDefault().TeamName;
                }
            }

            var currentAgentId = ticket.AgentId.ToString();
            var availableAgents = agents.Where(agent => agent.UserId != currentAgentId).ToList();

            var assignedTicketCounts = agents
                .Select(agent => new
                {
                    Agent = agent,
                    TicketCount = tickets.Count(t => t.AgentId.ToString() == agent.UserId.ToString())
                })
                .OrderByDescending(agent => agent.TicketCount)
                .ToList();

            var model = new AgentAssignmentViewModel
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                CreatedAt = ticket.DateCreated,
                Description = ticket.Description,
                Agents = availableAgents,
                AssignedTicketCounts = assignedTicketCounts
                    .Select(agent => new AgentTicketCountViewModel
                    {
                        Agent = agent.Agent,
                        TicketCount = agent.TicketCount
                    })
                    .ToList()
            };

            ViewBag.AdminSidebar = "Tickets";
            return View(model);
        }


        [HttpPost("Tickets/Assignment"), ActionName("TicketAssignment")]
        [AllowAnonymous]
        public ActionResult PostTicketAssignment([FromBody] AgentAssignmentViewModel model)
        {
            _ticketService.AssignAgent(model.TicketId, model.SelectedAgentId);


            return RedirectToAction("Tickets", new { id = model.TicketId });
        }
        #endregion
    }
}
