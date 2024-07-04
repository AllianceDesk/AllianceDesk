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
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using ASI.Basecode.Services.Services;



namespace ASI.Basecode.WebApp.Controllers
{
    public class ArticleController : ControllerBase<ArticleController>
    {

        private readonly IArticleService _articleService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public ArticleController(IArticleService articleService,
            IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _articleService = articleService;
        }


/*        #region GET Methods       */ 
        /// <summary>
        /// Return the Index View with a list of Tickets
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var data = _articleService.RetrieveAll();
            return View(data);
        }

        
        /*
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