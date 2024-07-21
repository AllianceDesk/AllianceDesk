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
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._userService = userService;
            this._ticketService = ticketService;
            _sessionHelper = sessionHelper;
        }

        [HttpGet("Tickets")]
        public IActionResult Tickets(byte? status, string? searchTerm, string? sortOrder, int? page)
        {
            var tickets = _ticketService.GetUserTickets(_sessionHelper.GetUserIdFromSession(), status, searchTerm, sortOrder, page);

            var currentPage = page ?? 1;
            var currentStatus = status ?? 0;
            var count = tickets.Count();
            var pageSize = 5;

            var statuses = _ticketService.GetStatuses()
                .Select(c => new KeyValuePair<string, string>(c.StatusId.ToString(), c.StatusName))
                .ToList();

            var categories = _ticketService.GetCategories()
                .Select(c => new KeyValuePair<string, string>(c.CategoryId.ToString(), c.CategoryName))
                .ToList();

            var priorities = _ticketService.GetPriorities()
                .Select(p => new KeyValuePair<string, string>(p.PriorityId.ToString(), p.PriorityName))
                .ToList();

            if (Math.Ceiling(tickets.Count() / (double )pageSize) > 1)
            {
                tickets = tickets.Skip((currentPage - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToList();
            }

            // Create view model and return view
            var model = new UserTicketsViewModel
            {
                Tickets = tickets,
                CurrentPage = currentPage,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                CurrentStatus = currentStatus,
                CurrentSearchTerm = string.IsNullOrEmpty(searchTerm) ? "" : searchTerm,
                Statuses = statuses,
                Categories = categories,
                Priorities = priorities,  
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
