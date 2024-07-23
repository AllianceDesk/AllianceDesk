using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("User")]
    public class UserController : ControllerBase<UserController>
    {
        private readonly IUserService _userService;
        private readonly ITicketService _ticketService;
        private readonly ISessionHelper _sessionHelper;
        private readonly INotificationService _notificationService;
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
                              IUserService userService,
                              ITicketService ticketService,
                              ISessionHelper sessionHelper,
                              INotificationService notificationService,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._userService = userService;
            this._ticketService = ticketService;
            this._notificationService = notificationService;
            _sessionHelper = sessionHelper;
        }

        [HttpGet("Preferences")]
        public IActionResult GetPreference()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession().ToString()).RoleId;
            
            if(userRole != 3)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            var preference = _userService.GetUserPreference();

            return Json(new
            {
                Name = preference.Name,
                Email = preference.Email,
                InAppNotifications = preference.InAppNotifications,
                EmailNotifications = preference.EmailNotifications,
                DefaultTicketView = preference.DefaultTicketView,
                DefaultTicketPerPage = preference.DefaultTicketPerPage,
            });
        }

        [HttpPost("UpdatePreferences")]
        public IActionResult UpdatePreferencePost([FromBody] UserPreferenceViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _userService.UpdatePreference(model);
                    // TODO: Add Toastr notification for this
                    return Ok(new { message = "Preferences updated successfully" });
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                 .Select(e => e.ErrorMessage);
                    return BadRequest(new { errors });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user preferences");
                return StatusCode(500, "Error updating user preferences");
            }
        }

        [HttpGet("Tickets")]
        public IActionResult Tickets(byte? status, string? searchTerm, string? sortOrder, int? page)
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession().ToString()).RoleId;

            if (userRole != 3)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            var userPreference = _userService.GetUserPreference();

            if (status == null)
            {
                switch (userPreference.DefaultTicketView)
                {
                    case "Open":
                        status = 1;
                        break;
                    case "Assigned":
                        status = 2;
                        break;
                    case "In Progress":
                        status = 3;
                        break;
                    case "Resolved":
                        status = 4;
                        break;
                    case "Closed":
                        status = 5;
                        break;
                }
            }

            var tickets = _ticketService.GetUserTickets(_sessionHelper.GetUserIdFromSession(), status, searchTerm, sortOrder, page);

            var pageSize = userPreference.DefaultTicketPerPage;
            var currentPage = page ?? 1;
            var currentStatus = status ?? 0;
            var currentSearchTerm = searchTerm ?? "";
            var count = tickets.Count();
           
            var statuses = _ticketService.GetStatuses()
                .Select(c => new KeyValuePair<string, string>(c.StatusId.ToString(), c.StatusName))
                .ToList();

            var categories = _ticketService.GetCategories()
                .Select(c => new KeyValuePair<string, string>(c.CategoryId.ToString(), c.CategoryName))
                .ToList();

            var priorities = _ticketService.GetPriorities()
                .Select(p => new KeyValuePair<string, string>(p.PriorityId.ToString(), p.PriorityName))
                .ToList();

            var agents = _userService.GetAgents();

            if (Math.Ceiling(tickets.Count() / (double)pageSize) > 1)
            {
                tickets = tickets.Skip((currentPage - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList()
                                 .AsQueryable();
            }

            // Create view model and return view
            var model = new UserTicketsViewModel
            {
                Tickets = tickets,
                CurrentPage = currentPage,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                CurrentStatus = currentStatus,
                CurrentSearchTerm = currentSearchTerm,
                Statuses = statuses,
                Categories = categories,
                Priorities = priorities,
                Agents = agents
            };

            return View(model);
        }

        [HttpPost("Tickets/Create")]
        public IActionResult TicketCreate(TicketViewModel ticket)
        {
            _ticketService.Add(ticket);
            return RedirectToAction("Tickets");
        }

        [HttpPost("Tickets/{id}/Delete")]
        public IActionResult TicketDelete(string id)
        {

            var ticket = _ticketService.GetById(id);
 
            if (ticket == null)
            {
                return NotFound();
            }

            _ticketService.Delete(id);

            return RedirectToAction("Tickets");
        }

        [HttpGet("Tickets/{id}/Edit")]
        public IActionResult TicketEdit(string id)
        {
            var ticket = _ticketService.GetById(id);

            if (ticket == null)
            {
                return NotFound(); // Handle ticket not found scenario
            }

            var categories = _ticketService.GetCategories()
                                   .Select(c => new SelectListItem
                                   {
                                       Value = c.CategoryId.ToString(),
                                       Text = c.CategoryName
                                   })
                                   .ToList();

            var priorities = _ticketService.GetPriorities()
                                           .Select(p => new SelectListItem
                                           {
                                               Value = p.PriorityId.ToString(),
                                               Text = p.PriorityName
                                           })
                                           .ToList();
            // Pass data to ViewBag
            ViewBag.Categories = new SelectList(categories, "Value", "Text", ticket.CategoryId.ToString());
            ViewBag.Priorities = new SelectList(priorities, "Value", "Text", ticket.PriorityId.ToString());

            return Json(new
            {
                id = ticket.TicketId,
                title = ticket.Title,
                description = ticket.Description,
                categoryId = ticket.CategoryId,
                priorityId = ticket.PriorityId
            });
        }


        [HttpPost("Tickets/{id}/Edit"), ActionName("TicketEdit")]
        public IActionResult TicketEditPost(string id, UserTicketsViewModel model)
        {
            var ticket = _ticketService.GetById(id); // Ensure this matches with the ID being passed
            if (ticket == null)
            {
                return NotFound();
            }

            // Update ticket properties from the model
            ticket.Title = model.Ticket.Title;
            ticket.Description = model.Ticket.Description;
            ticket.CategoryId = model.Ticket.CategoryId;
            ticket.PriorityId = model.Ticket.PriorityId;

            // Call service to update the ticket
            _ticketService.Update(ticket);

            // Redirect to the Tickets action
            return RedirectToAction("Tickets");
        }

        [HttpPost("Tickets/{id}/Feedback")]
        public IActionResult TicketFeedback(string id, UserTicketsViewModel model)
        {
            model.Feedback.TicketId = Guid.Parse(id);
            var ticket = _ticketService.GetById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            _ticketService.AddFeedback(model.Feedback);

            return RedirectToAction("Tickets");
        }

        [HttpPost("Tickets/{id}/Close")]
        public IActionResult TicketClose(string id)
        {
            var ticket = _ticketService.GetById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            _ticketService.UpdateStatus(id, 5);

            return RedirectToAction("Tickets");
        }

        [HttpPost("Tickets/{id}/Reopen")]
        public IActionResult TicketReopen(string id)
        {
            var ticket = _ticketService.GetById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            _ticketService.UpdateStatus(id, 3);

            return RedirectToAction("Tickets");
        }
    }
}
