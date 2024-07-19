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
        public IActionResult Tickets(string? status, int? page, string? searchTerm)
        {

            var tickets = _ticketService.RetrieveAll();

            var userTickets = tickets
                .Where(t => t.CreatorId == _sessionHelper.GetUserIdFromSession().ToString())
                .OrderByDescending(t => t.DateCreated)
                .AsEnumerable();

            // Replace this later to retrieve from the user preferences
            var pageSize = 5;

            var statuses = _ticketService.GetStatuses()
                                   .Select(c => new SelectListItem
                                   {
                                       Value = c.StatusId.ToString(),
                                       Text = c.StatusName
                                   })
                                   .ToList();

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


            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                userTickets = userTickets.Where(t => t.StatusId == status);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                userTickets = userTickets.Where(t => t.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                                     t.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            var CurrentPage = page ?? 1;
            var count = userTickets.Count();


            if (Math.Ceiling(userTickets.Count() / (double)pageSize) > 1)
            {
                userTickets = userTickets.Skip((CurrentPage - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToList();
            }

            // Pass data to ViewBag
            ViewBag.Statuses = new SelectList(statuses, "Value", "Text");
            ViewBag.Categories = new SelectList(categories, "Value", "Text");
            ViewBag.Priorities = new SelectList(priorities, "Value", "Text");
            ViewBag.CurrentStatus = string.IsNullOrEmpty(status) ? "All" : status;
            ViewBag.CurrentPage = CurrentPage;
            ViewBag.TotalPages = Math.Ceiling(count / (double)pageSize);
            ViewBag.CurrentSearchTerm = searchTerm;

            var model = new UserTicketViewModel
            {
                Tickets = userTickets,
                Ticket = new TicketViewModel(),
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
        public IActionResult TicketEditPost(string id, UserTicketViewModel model)
        {
            var ticket = _ticketService.GetById(id); // Ensure this matches with the ID being passed
            if (ticket == null)
            {
                return NotFound();
            }

            ticket.Title = model.Ticket.Title;
            ticket.Description = model.Ticket.Description;
            ticket.CategoryId = model.Ticket.CategoryId;
            ticket.PriorityId = model.Ticket.PriorityId;

            _ticketService.Update(ticket);
            return RedirectToAction("Tickets");
        }
    }
}
