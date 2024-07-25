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

        #region User Navigation

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
                    return Ok(new { success = true, message = "Preferences updated successfully" });
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                 .Select(e => e.ErrorMessage);
                    return BadRequest(new { success = false, errors });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user preferences");
                return StatusCode(500, new { success = false, message = "Error updating user preferences" });
            }
        }

        #endregion

        #region Tickets

        [HttpGet("Tickets")]
        public IActionResult Tickets(byte? status, string? searchTerm, string? sortOrder, int? page)
        {
            var userRole = _userService.GetUserById(_sessionHelper.GetUserIdFromSession()).RoleId;

            if (userRole == null || userRole == 0)
            {
                return RedirectToAction("Account", "Login");
            }

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

            var tickets = _ticketService.GetUserTickets(_sessionHelper.GetUserIdFromSession());

            if (status != 0)
            {
                tickets = tickets.Where(t => t.StatusId == status);
            }

            if (!String.IsNullOrEmpty(searchTerm))
            {
                tickets = tickets.Where(t => t.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

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
            List<string> emails = new List<string>();
            foreach (var agent in agents)
            {
                emails.Add(agent.Email);
            }



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
                Favorites = _articleService.RetrieveFavorites(),
                Emails = emails
            };

            return View(model);
        }

        [HttpGet("Tickets/{id}")]
        public IActionResult Ticket(Guid id)
        {
            var ticket = _ticketService.GetById(id);

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


        [HttpPost("Tickets/{id}/Edit")]
        public IActionResult EditTicketPost(UserTicketsViewModel model)
        {
            var ticket = _ticketService.GetById(model.Ticket.TicketId);
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

            // Add a new activity to the ticket
            // Add ticket activity
            TicketActivityViewModel newActivity = new TicketActivityViewModel();
            newActivity.TicketId = ticket.TicketId;
            newActivity.OperationId = 2;
            newActivity.UserId = _sessionHelper.GetUserIdFromSession();
            newActivity.Message = "Ticket was updated by user";

            _ticketService.AddActivity(newActivity);

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

            _ticketService.UpdateStatus(ticketId, 4);

            TicketActivityViewModel activity = new TicketActivityViewModel
            {
                TicketId = ticketId,
                OperationId = 5,
                Message = "Ticket was closed by user",
                ModifiedAt = DateTime.Now,
                UserId = _sessionHelper.GetUserIdFromSession()
            };

            _ticketService.AddActivity(activity);

            return RedirectToAction("Tickets", new { status = 4 });
        }


        [HttpPost("Tickets/{id}/Reopen")]
        public IActionResult ReopenTicket(string id)
        {
            var ticketId = Guid.Parse(id);
            var ticket = _ticketService.GetById(ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            _ticketService.UpdateStatus(ticketId, 2);

            TicketActivityViewModel activity = new TicketActivityViewModel
            {
                TicketId = ticketId,
                OperationId = 6,
                Message = "Ticket was reopened by user",
                ModifiedAt = DateTime.Now,
                UserId = _sessionHelper.GetUserIdFromSession()
            };

            _ticketService.AddActivity(activity);

            return RedirectToAction("Tickets", new { status = 2 });
        }

        #endregion

        #region KnowledgeBase
        [HttpGet("/KnowledgeBaseModal")]
        public IActionResult KnowledgeBaseModal()
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
        }
        #endregion
    }
}
