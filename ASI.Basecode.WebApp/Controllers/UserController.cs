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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("User")]
    public class UserController : ControllerBase<UserController>
    {
        private readonly IUserService _userService;
        private readonly ITicketService _ticketService;
        private readonly ISessionHelper _sessionHelper;
        private readonly INotificationService _notificationService;
        private readonly IArticleService _articleService;
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
                              IArticleService articleService,
                              INotificationService notificationService,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._userService = userService;
            this._ticketService = ticketService;
            this._notificationService = notificationService;
            this._sessionHelper = sessionHelper;
            this._articleService = articleService;
        }

        [HttpGet("Preferences")]
        public IActionResult GetPreference()
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 3)
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
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 3)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            var userPreference = _userService.GetUserPreference();
            var pageSize = 10;

            if (userPreference != null)
            {
                pageSize = userPreference.DefaultTicketPerPage;

                if (status == null)
                {
                    status ??= userPreference switch
                    {
                        { DefaultTicketView: "Open" } => 1,
                        { DefaultTicketView: "Assigned" } => 2,
                        { DefaultTicketView: "In Progress" } => 3,
                        { DefaultTicketView: "Resolved" } => 4,
                        { DefaultTicketView: "Closed" } => 5,
                        _ => status
                    };
                }
            }

            var tickets = _ticketService.GetUserTickets(_sessionHelper.GetUserIdFromSession(), status, searchTerm, sortOrder, page);

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
                Agents = agents,
                Favorites = _articleService.RetrieveFavorites(),
            };

            return View(model);
        }

        [HttpGet("Tickets/{id}")]
        public IActionResult Ticket(string ticketId)
        {
            var ticket = _ticketService.GetById(Guid.Parse(ticketId));

            if (ticket == null)
            {
                return NotFound(); // Handle ticket not found scenario
            }

            return Json(new
            {
                user = ticket.CreatorName,
                title = ticket.Title,
                description = ticket.Description,
                dateCreated = ticket.DateCreated.ToString("MM/dd/yyyy hh:mm tt"),
                latestUpdateMessage = ticket.LatestUpdate.Message,
                latestUpdateDate = ticket.LatestUpdate.ModifiedAt.ToString("MM/dd/yyyy hh:mm tt"),
                files = ticket.AttachmentStrings
            });
        }

        [HttpPost("Tickets/Create")]
        public async Task<IActionResult> TicketCreate(TicketViewModel ticket)
        {
            if (ModelState.IsValid)
            {
                await _ticketService.AddAsync(ticket);
                return RedirectToAction("Tickets");
            }

            return View(ticket);
        }

        [HttpPost("Tickets/{id}/Delete")]
        public IActionResult TicketDelete(string id)
        {
            Guid ticketId = Guid.Parse(id);
            var ticket = _ticketService.GetById(ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            _ticketService.Delete(ticketId);

            return RedirectToAction("Tickets");
        }

        [HttpGet("Tickets/{id}/Edit")]
        public IActionResult TicketEdit(string id)
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole != 3)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            var ticketId = Guid.Parse(id);
            var ticket = _ticketService.GetById(ticketId);

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
            var ticketId = Guid.Parse(id);
            var ticket = _ticketService.GetById(ticketId);
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
            var ticketId = Guid.Parse(id);
            var ticket = _ticketService.GetById(ticketId);

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
            var ticketId = Guid.Parse(id);
            var ticket = _ticketService.GetById(ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            _ticketService.UpdateStatus(ticketId, 5);

            return RedirectToAction("Tickets");
        }

        [HttpPost("Tickets/{id}/Reopen")]
        public IActionResult TicketReopen(string id)
        {
            var ticketId = Guid.Parse(id);
            var ticket = _ticketService.GetById(ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            _ticketService.UpdateStatus(ticketId, 3);

            return RedirectToAction("Tickets");
        }

        /*[HttpGet("/KnowledgeBase")]
        public IActionResult KnowledgeBase()
        {

            var data = _articleService.RetrieveAll()
                                        .Select(u => new ArticleViewModel
                                        {
                                            ArticleId = u.ArticleId,
                                            Title = u.Title,
                                            Body = u.Body,
                                            CategoryNavigation = u.CategoryNavigation,
                                            DateUpdated = u.DateUpdated,
                                        })
                                        .OrderBy(u => u.Title)
                                        .ToList();

            var viewModel = new UserTicketsViewModel
            {
                Articles = data
            };
            return View(viewModel);
        }*/
    }
}
