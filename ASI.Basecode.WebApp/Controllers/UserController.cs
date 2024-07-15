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

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("User")]
    public class UserController : ControllerBase<UserController>
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
        public UserController(IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IUserService userService,
                              ITicketService ticketService,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._userService = userService;
            this._ticketService = ticketService;
        }


        [HttpGet("Tickets")]
        public IActionResult Tickets(string? status)
        {
            // Replace with User.Identity.Name when authentication is implemented
            string user = "857949fe-ec30-4c0b-a514-eb0fd9262738";
            string user2 = "90122701-1c8c-40a4-8936-7717Cfaa9c14";

            var tickets = _ticketService.RetrieveAll();


            var userTickets = tickets
                .Where(t => t.CreatorId == user)
                .OrderByDescending(t => t.DateCreated)
                .ToList();


            if (userTickets.Count == 0)
            {
                Console.WriteLine("No tickets found for user");
                return NotFound();
            }

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

            // Pass data to ViewBag
            ViewBag.Statuses = new SelectList(statuses, "Value", "Text");
            ViewBag.Categories = new SelectList(categories, "Value", "Text");
            ViewBag.Priorities = new SelectList(priorities, "Value", "Text");

            if (!string.IsNullOrEmpty(status))
            {
                ViewBag.CurrentStatus = status;

                var model = new TicketPageViewModel
                {
                    Tickets = userTickets.Where(t => t.StatusId == status),
                    Ticket = new TicketViewModel() // Initialize a new ticket for the form
                };

                return View(model);
            }
            else
            {
                ViewBag.CurrentStatus = "All";

                var model = new TicketPageViewModel
                {
                    Tickets = userTickets,
                    Ticket = new TicketViewModel() // Initialize a new ticket for the form
                };

                return View(model);
            }

        }

        [HttpPost("Ticket/Create")]
        public IActionResult TicketCreate(TicketViewModel ticket)
        {
            // Replace with User.Identity.Name when authentication is implemented
            ticket.CreatorId = "857949FE-EC30-4C0B-A514-EB0FD9262738";
            _ticketService.Add(ticket);

            return RedirectToAction("Tickets");
        }

        [HttpPost("Ticket/{id}/Delete")]
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


        [HttpGet("Ticket/{id}/Edit")]
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

        
        [HttpPost("Ticket/{id}/Edit"), ActionName("TicketEdit")]
        public IActionResult TicketEditPost(string id, TicketPageViewModel model)
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
