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

        /// <summary>
        /// Returns Article View.
        /// </summary>
        /// <returns> Article View </returns>
        [HttpGet ("/KnowledgeBase")]
        public IActionResult Index()
        {
            var data = _articleService.RetrieveAll();
            return View(data);
        }

        /// <summary>
        /// Return Article-Create View
        /// </summary>
        /// <returns></returns>
        [HttpGet ("/KnowledgeBase/Add-Article")]
        public IActionResult Create()
        {
            var categories = _articleService.GetCategories()
                                   .Select(c => new SelectListItem
                                   {
                                       Value = c.CategoryId.ToString(),
                                       Text = c.CategoryName
                                   })
                                   .ToList();
            ViewBag.Categories = new SelectList(categories, "Value", "Text");

            return View();
        }

        [HttpPost]
        public IActionResult PostCreate(ArticleViewModel articleViewModel)
        {
            _articleService.Add(articleViewModel);
            return RedirectToAction("Index");
        }

    }
}