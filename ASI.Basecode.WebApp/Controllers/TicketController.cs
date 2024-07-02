﻿using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Data.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;



namespace ASI.Basecode.WebApp.Controllers
{
    public class TicketController : ControllerBase<TicketController>
    {

        private readonly ITicketService _ticketService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public TicketController(ITicketService ticketService,
            IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _ticketService = ticketService;
        }

        #region Admin Methods
        [HttpGet("/Admin/Tickets")]
        public IActionResult AdminTickets()
        {
            var data = _ticketService.RetrieveAll();
            return View(data);
        }

        #endregion

        #region User Methods

        [HttpGet("/User/Tickets")]
        public IActionResult UserTickets(string? status)
        {
            string user = "857949fe-ec30-4c0b-a514-eb0fd9262738"; // Replace with User.Identity.Name when authentication is implemented
            string user2 = "90122701-1c8c-40a4-8936-7717Cfaa9c14";

            var tickets = _ticketService.RetrieveAll();

            var userTickets = tickets.Where(t => t.CreatorId == user).ToList();


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

            // Pass data to ViewBag
            ViewBag.Statuses = new SelectList(statuses, "Value", "Text");

            
           return View(userTickets);
            
        }


        [HttpGet("/User/Tickets/Create")]
        /// <summary>
        /// Go to the Create a Ticket View
        /// </summary>
        /// <returns></returns>
        public IActionResult UserCreate()
        {
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
            ViewBag.Categories = new SelectList(categories, "Value", "Text");
            ViewBag.Priorities = new SelectList(priorities, "Value", "Text");

            return View();
        }

        [HttpPost("/User/Tickets/Create")]
        public IActionResult PostUserCreate(TicketViewModel ticket)
        {
            // Replace with User.Identity.Name when authentication is implemented
            ticket.CreatorId = "857949FE-EC30-4C0B-A514-EB0FD9262738";
            _ticketService.Add(ticket);
            
            return RedirectToAction(nameof(UserTickets));
        }
        #endregion


        [HttpGet("Admin/Tickets/{id}")]
        public IActionResult AdminViewTicket(string id)
        {
            var ticket = _ticketService.GetById(id);

            if (ticket == null)
            {
                return NotFound(); // Handle ticket not found scenario
            }

            return View("Details", ticket);
        }

       
        /*#region GET Methods        
        /// <summary>
        /// Return the Index View with a list of Tickets
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        
        public IActionResult Index()
        {
            var data = _ticketService.RetrieveAll();
            return View(data);
        }

        

        /// <summary>
        /// Get the Full Details of a Ticket using the Ticket ID
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Details(string id)
        {
            *//*var ticket = _ticketService.RetrieveAll().FirstOrDefault(x => x.Id.Equals(id));

            if (ticket == null)
            {
                return NotFound();
            }


            TicketPriority priority = _ticketService.GetPriorityById(ticket.PriorityId);

            TicketStatus status = _ticketService.GetStatusById(ticket.StatusId);

            ticket.StatusName = status.Name;
            ticket.PriorityName = priority.Name;*//*

            return View(null);
        }


        /// <summary>
        /// Go to the Edit View of a Ticket
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(string Id)
        {
            var data = _ticketService.RetrieveAll().Where(x => x.Id.Equals(Id)).FirstOrDefault();

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        /// <summary>
        /// Go to the Delete View of a Ticket
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Delete(string Id)
        {
            var data = _ticketService.RetrieveAll().Where(x => x.Id.Equals(Id)).FirstOrDefault();

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        #endregion

        #region POST METHODS        
        /// <summary>
        /// Posts the Creation of a Ticket.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        /// <param name="attachments">The attachments.</param>
        /// <returns></returns>
       

        /// <summary>
        /// Posts the changes or updates of a Ticket.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostUpdate(TicketViewModel ticket)
        {
            _ticketService.Update(ticket);

            // Add a check if the ticket was successfully updated

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Posts the deletion of a Ticket.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostDelete(string Id)
        {
            _ticketService.Delete(Id);

            // Add a check if the ticket was successfully deleted

            return RedirectToAction("Index");
        }

        #endregion

        /// <summary>
        /// Determines whether [is image file] [the specified file].
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        ///   <c>true</c> if [is image file] [the specified file]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsImageFile(IFormFile file)
        {
            if (file == null)
                return false;

            string[] allowedImageTypes = { "image/jpeg", "image/png", "image/gif" };
            return allowedImageTypes.Contains(file.ContentType);
        }*/
    }
}