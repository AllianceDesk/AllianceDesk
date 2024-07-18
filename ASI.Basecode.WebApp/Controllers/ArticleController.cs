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
        public IActionResult CreateModal()
        {
            ViewBag.AdminSidebar = "Index";
            var categories = _articleService.GetCategories()
                                   .Select(c => new SelectListItem
                                   {
                                       Value = c.CategoryId.ToString(),
                                       Text = c.CategoryName
                                   })
                                   .ToList();
            ViewBag.Categories = new SelectList(categories, "Value", "Text");

            return PartialView("CreateModal");
        }

        [HttpPost("/KnowledgeBase/Add-Article")]
        public IActionResult PostCreate(ArticleViewModel articleViewModel)
        {
            _articleService.Add(articleViewModel);
            return RedirectToAction("Index");
        }

        [HttpGet ("/KnowledgeBase/Article-Detail")]
        public IActionResult DetailModal(string articleId)
        {
            var articleData = _articleService.RetrieveAll().Where(a => a.ArticleId.ToString() == articleId).FirstOrDefault();
            if (articleData == null)
            {
                return NotFound();
            }

            var viewModel = new ArticleViewModel
            {
                ArticleId = articleId,
                Title = articleData.Title,
                Body = articleData.Body,
                UpdatedBy = articleData.UpdatedBy,
                DateUpdated = articleData.DateUpdated,
            };

            return PartialView("DetailModal", viewModel);
        }

        [HttpGet("/KnowledgeBase/Article-Edit")]
        public IActionResult EditModal(string articleId)
        {
            var articleData = _articleService.RetrieveAll().Where(a => a.ArticleId.ToString() == articleId).FirstOrDefault();
            if (articleData == null)
            {
                return NotFound();
            }

            var viewModel = new ArticleViewModel
            {
                ArticleId = articleData.ArticleId,
                Title = articleData.Title,
                Body = articleData.Body,
                CategoryNavigation = articleData.CategoryNavigation,
                UpdatedBy = articleData.UpdatedBy,
                CategoryId = articleData.CategoryId,
            };

            var categories = _articleService.GetCategories()
                                   .Select(c => new SelectListItem
                                   {
                                       Value = c.CategoryId.ToString(),
                                       Text = c.CategoryName
                                   })
                                   .ToList();
            ViewBag.Categories = new SelectList(categories, "Value", "Text", viewModel.CategoryId);

            return PartialView("EditModal", viewModel);
        }

        [HttpPost("/KnowledgeBase/Article-Edit")]
        public IActionResult PostEditModal(ArticleViewModel article)
        {
            if (article == null)
            {
                return NotFound();
            }
            _articleService.Update(article);

            return RedirectToAction("Index");
        }

        [HttpGet("/KnowledgeBase/Article-Delete")]
        public IActionResult DeleteModal(string articleId)
        {
            var articleData = _articleService.RetrieveAll().Where(a => a.ArticleId.ToString() == articleId).FirstOrDefault();
            if (articleData == null)
            {
                return NotFound();
            }

            var viewModel = new ArticleViewModel
            {
                ArticleId= articleData.ArticleId,
            };

            return PartialView("DeleteModal", viewModel);
        }

        [HttpPost("/KnowledgeBase/Article-Delete")]
        public IActionResult PostDeleteModal(ArticleViewModel article)
        {
            if (article == null)
            {
                return NotFound();
            }
            _articleService.Delete(article);

            return RedirectToAction("Index");
        }

    }
}