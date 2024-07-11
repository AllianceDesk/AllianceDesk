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

        public ArticleService (IMapper mapper, IArticleRepository articleRepository, ICategoryRepository categoryRepository)
        {
            this.mapper = mapper;
            _articleRepository = articleRepository;
            _categoryRepository = categoryRepository;
        }
        public IEnumerable<ArticleViewModel> RetrieveAll()
        {



            var data = _articleRepository.RetrieveAll().Select(s => new ArticleViewModel
            { 
                ArticleId = s.ArticleId.ToString(),
                Title = s.Title,
                Body = s.Body,
                CategoryNavigation = _categoryRepository.RetrieveAll().Where(c => c.CategoryId == s.CategoryId).FirstOrDefault().CategoryName,
                
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

            // This is a temporary value for CreatedBy, replace when user authentication is implemented
            newArticle.CreatedBy = Guid.Parse("c9876543-b21d-43e5-a345-556642441234");
            newArticle.UpdatedBy = Guid.Parse("c9876543-b21d-43e5-a345-556642441234");

            newArticle.CategoryId = Convert.ToByte(article.CategoryId);


            _articleRepository.AddArticle(newArticle);
        }

        public IEnumerable<Category> GetCategories()
        {
            return _categoryRepository.RetrieveAll();
        }

    }
}
