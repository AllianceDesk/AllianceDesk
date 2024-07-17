/*using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    public class NotificationController : ControllerBase<NotificationController>
    {
        private readonly INotificationService _notificationService;
        /// <param name = "httpContextAccessor" ></ param >
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>

        public NotificationController(
            INotificationService notificationService,
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Controller for index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var data = _notificationService.RetrieveAll();
            return View(data);
        }

        /// <summary>
        /// Controller for create
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Controller for details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Details(int id)
        {
            var data = _notificationService.RetrieveAll().Where(x => x.Id == id).FirstOrDefault();
            return View(data);
        }

        /// <summary>
        /// Controller for Edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var data = _notificationService.RetrieveAll().Where(x => x.Id == id).FirstOrDefault();
            return View(data);

        }

        /// <summary>
        /// Controller for Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var data = _notificationService.RetrieveAll().Where(x => x.Id == id).FirstOrDefault();
            return View(data);
        }

        /// <summary>
        /// Controller for PostCreate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostCreate(NotificationServiceModel model)
        {
            _notificationService.Add(model);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Controller for PostUpdate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostUpdate(NotificationServiceModel model)
        {
            _notificationService.Update(model);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Controller for PostDelete
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostDelete(int Id)
        {
            _notificationService.Delete(Id);
            return RedirectToAction("Index");
        }




    }
}
*/