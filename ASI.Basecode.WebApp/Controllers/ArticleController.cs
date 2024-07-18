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
            ViewBag.AdminSidebar = "Index";
            var data = _articleService.RetrieveAll()
                                        .Where (a => a.Status == true)
                                        .Select(u => new ArticleViewModel
                                        {
                                            ArticleId = u.ArticleId,
                                            Title = u.Title,
                                            Body = u.Body,
                                            CategoryNavigation = u.CategoryNavigation,
                                            DateUpdated = u.DateUpdated,
                                        })
                                        .ToList();

            var viewModel = new ArticleViewModel
            {
                Articles = data
            };
            return View(viewModel);
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

        /// <summary>
        /// Return Article-Edit View
        /// </summary>
        /// <returns></returns>
        [HttpGet("/KnowledgeBase/Details")]
        public IActionResult DetailModal(string articleId)
        {
            var articleData = _articleService.GetArticles().Where(a => a.ArticleId == articleId).FirstOrDefault();
            if (articleData == null)
            {
                return NotFound();
            }
            var articleModel = new ArticleViewModel
            {
                ArticleId = articleId,
                Title = articleData.Title,
                Body = articleData.Body,
                DateUpdated = articleData.DateUpdated,
                CreatedBy = articleData.CreatedBy,
                CategoryNavigation = articleData.CategoryNavigation,
                CategoryId = articleData.CategoryId,
            };

            return PartialView("DetailModal", articleModel);
        }

        /// <summary>
        /// Return Article-Edit View
        /// </summary>
        /// <returns></returns>
        [HttpGet ("/KnowledgeBase/Edit")]
        public IActionResult EditModal(string articleId)
        {
            var articleData = _articleService.GetArticles().Where(a => a.ArticleId == articleId).FirstOrDefault();
            if (articleData == null)
            {
                return NotFound();
            }
            var articleModel = new ArticleViewModel
            {
                ArticleId = articleId,
                Title = articleData.Title,
                Body = articleData.Body,
                DateUpdated = articleData.DateUpdated,
                CreatedBy = articleData.CreatedBy,
                CategoryNavigation = articleData.CategoryNavigation,
                CategoryId = articleData.CategoryId,
            };

            var categories = _articleService.GetCategories()
                                   .Select(c => new SelectListItem
                                   {
                                       Value = c.CategoryId.ToString(),
                                       Text = c.CategoryName
                                   })
                                   .ToList();
            ViewBag.Categories = new SelectList(categories, "Value", "Text", articleModel.CategoryId);

            return PartialView("EditModal", articleModel);
        }

        /// <summary>
        /// Return 
        /// </summary>
        /// <returns></returns>
        [HttpPost("/KnowledgeBase/Edit")]
        public IActionResult PostEditModal(ArticleViewModel article)
        {
            _articleService.Update(article);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Return Article-Delete View
        /// </summary>
        /// <returns></returns>
        [HttpGet("/KnowledgeBase/Delete")]
        public IActionResult DeleteModal(string articleId)
        {
            var articleData = _articleService.GetArticles().Where(a => a.ArticleId == articleId).FirstOrDefault();
            var articleModel = new ArticleViewModel
            {
                ArticleId = articleId,
            };

            return PartialView("DeleteModal", articleModel);
        }

        /// <summary>
        /// Return 
        /// </summary>
        /// <returns></returns>
        [HttpPost("/KnowledgeBase/Delete")]
        public IActionResult PostDeleteModal(ArticleViewModel article)
        {
            _articleService.Delete(article);
            return RedirectToAction("Index");
        }
    }
}