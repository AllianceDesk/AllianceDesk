using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ASI.Basecode.WebApp.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    public class AdminController : ControllerBase<AdminController>
    {
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
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {

        }

        /// <summary>
        /// Returns Home View.
        /// </summary>
        /// <returns> Home View </returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ViewUser()
        {
            ViewBag.IsLoginOrRegister = false;
            ViewBag.AdminSidebar = "ViewUser";
            return this.View();
        }

        /// <summary>
        /// Returns Tickets View.
        /// </summary>
        /// <returns> Tickets View </returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("TicketsAll/{id?}")]
        public ActionResult TicketsAll(string? id)
        {
            ViewBag.IsLoginOrRegister = false;
            ViewBag.AdminSidebar = "Tickets";

            if (id != null)
            {
                // Handle the case where an ID is provided
                ViewBag.TicketId = id;
                return this.View("/Views/Admin/TicketDetail.cshtml");
            }
            else
            {
                // Handle the case where no ID is provided
                return this.View("/Views/Admin/TicketsAll.cshtml");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TicketAssignment()
        {
            ViewBag.AdminSidebar = "Tickets";
            return this.View();
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult TicketReassignment()
        {
            ViewBag.AdminSidebar = "Tickets";
            return this.View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TicketResolved()
        {
            ViewBag.AdminSidebar = "Tickets";
            return this.View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TicketOccupied()
        {
            ViewBag.AdminSidebar = "Tickets";
            return this.View();
        }
    }
}
