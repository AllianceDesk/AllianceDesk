using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;


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
        public TicketController(ITicketService sampleCrudService,
            IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _ticketService = sampleCrudService;
        }



        public IActionResult Index()
        {
            var data = _ticketService.RetrieveAll();
            return View(data);
        }

        /// <summary>
        /// Return Create View
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details(string Id)
        {
            var data = _ticketService.RetrieveAll().Where(x => x.Id.Equals(Id)).FirstOrDefault();
            return View(data);
        }

        [HttpGet]
        public IActionResult Edit(string Id)
        {
            var data = _ticketService.RetrieveAll().Where(x => x.Id.Equals(Id)).FirstOrDefault();
            return View(data);
        }

        [HttpGet]
        public IActionResult Delete(string Id)
        {
            var data = _ticketService.RetrieveAll().Where(x => x.Id.Equals(Id)).FirstOrDefault();
            return View(data);
        }

        [HttpPost]
        public IActionResult PostCreate(TicketViewModel ticket)
        {
            _ticketService.Add(ticket);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult PostUpdate(TicketViewModel ticket)
        {
            _ticketService.Update(ticket);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult PostDelete(string Id)
        {
            _ticketService.Delete(Id);
            return RedirectToAction("Index");
        }
    }
}