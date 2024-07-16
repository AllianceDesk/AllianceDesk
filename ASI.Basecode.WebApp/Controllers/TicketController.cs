using ASI.Basecode.Services.Interfaces;
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
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Controllers
{
    public class TicketController : ControllerBase<TicketController>
    {

        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public TicketController(ITicketService ticketService, IUserService userService,
            IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _ticketService = ticketService;
            _userService = userService;
        }

        #region Agent Methods

        [HttpGet("/Agent/Tickets")]
        public IActionResult AgentTickets()
        {
            // Replace with User.Identity.Name when authentication is implemented
            string agent = "3850590f-e5e1-468b-a9a2-420074e9073f";
            string agent2 = "aba6bb10-e42d-4714-907b-445f494e1dff";

            var tickets = _ticketService.RetrieveAll();

            var agentTickets = tickets.Where(t => t.AgentId == agent);

            // TO DO: Find a way to be able to see the date the ticket was assigned to the agent
            
            /*var updatedAgentTickets = new List<TicketViewModel>();

            foreach (var ticket in agentTickets)
            {
                var history = _ticketService.GetHistory(ticket.TicketId);

                if (history != null)
                {
                    var firstOperation2 = history.FirstOrDefault(s => s.OperationId == 2);

                    if (firstOperation2 != null)
                    {
                        // Create a new TicketViewModel with updated DateAssigned
                        var updatedTicket = new TicketViewModel
                        {
                            TicketId = ticket.TicketId,
                            
                            DateAssigned = firstOperation2.ModifiedAt
                        };
                        updatedAgentTickets.Add(updatedTicket);
                    }
                }
            }*/

            // Return the view with the updated agentTickets
            return View(agentTickets);
        }

        #endregion

        #region User Methods

        [HttpGet("/User/Tickets/{id}")]
        public IActionResult UserDetails(string id)
        {
            var ticket = _ticketService.GetById(id);

            ticket.TicketMessages = _ticketService.GetMessages(id);

            if (ticket == null)
            {
                return NotFound(); // Handle ticket not found scenario
            }

            return View(ticket);
        }

        [HttpPost("/User/Tickets/{id}/Message")]
        public IActionResult PostMessage(string id, TicketViewModel model)
        {
            var message = new TicketMessageViewModel
            {
                SentById = "857949FE-EC30-4C0B-A514-EB0FD9262738", // Replace with User.Identity.Name when authentication is implemented
                Message = model.NewMessageBody,
                PostedAt = DateTime.Now,
                TicketId = id
            };
           
            _ticketService.AddMessage(message);

            return RedirectToAction(nameof(UserDetails), new { id = message.TicketId });
        }

        #endregion
    }
}