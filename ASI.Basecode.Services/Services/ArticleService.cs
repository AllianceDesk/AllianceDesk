using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Data.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using ASI.Basecode.Services.Interfaces;

namespace ASI.Basecode.Services.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IMapper mapper;
        private readonly IArticleRepository _articleRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISessionHelper _sessionHelper;

        public ArticleService (IMapper mapper, IArticleRepository articleRepository, ICategoryRepository categoryRepository, ISessionHelper sessionHelper)
        {
            this.mapper = mapper;
            _articleRepository = articleRepository;
            _categoryRepository = categoryRepository;
            _sessionHelper = sessionHelper;
        }
        public IEnumerable<ArticleViewModel> RetrieveAll()
        {

            var data = _articleRepository.RetrieveAll().Select(s => new ArticleViewModel
            { 
                ArticleId = s.ArticleId.ToString(),
                Title = s.Title,
                Body = s.Body,
                CategoryNavigation = _categoryRepository.RetrieveAll().Where(c => c.CategoryId == s.CategoryId).FirstOrDefault().CategoryName,
                DateUpdated = s.DateUpdated.ToString(),
                
            });

            return data;
        }

        public void Add(ArticleViewModel article)
        {
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article), "ArticleViewModel cannot be null");
            }

            var newArticle = new Article();
            newArticle.ArticleId = Guid.NewGuid();
            newArticle.Title = article.Title;
            newArticle.Body = article.Body;
            newArticle.DateCreated = DateTime.Now;
            newArticle.DateUpdated = DateTime.Now;

            newArticle.CreatedBy = _sessionHelper.GetUserIdFromSession();
            newArticle.UpdatedBy = _sessionHelper.GetUserIdFromSession();

            newArticle.CategoryId = Convert.ToByte(article.CategoryId);


            _articleRepository.AddArticle(newArticle);
        }

        public IEnumerable<Category> GetCategories()
        {
            return _categoryRepository.RetrieveAll();
        }

        public IEnumerable<Article> GetArticles()
        {
            return _articleRepository.RetrieveAll();
        }

    }
}
