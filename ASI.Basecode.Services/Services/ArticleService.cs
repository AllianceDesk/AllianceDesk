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
        private readonly IMapper _mapper;
        private readonly IArticleRepository _articleRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISessionHelper _sessionHelper;

        public ArticleService ( IMapper mapper, 
                                IArticleRepository articleRepository, 
                                ICategoryRepository categoryRepository, 
                                ISessionHelper sessionHelper,
                                IUserRepository userRepository)
        {
            _mapper = mapper;
            _articleRepository = articleRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
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
                DateUpdated = s.DateUpdated.HasValue ? s.DateUpdated.Value.ToString("MMM dd") : string.Empty,
                CreatedBy = _userRepository.GetUsers().Where(u => u.UserId == s.CreatedBy).FirstOrDefault().Username,
                Status = s.Status,
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
            newArticle.Status = true;

            // This is a temporary value for CreatedBy, replace when user authentication is implemented
            newArticle.CreatedBy = _sessionHelper.GetUserIdFromSession();
            newArticle.UpdatedBy = _sessionHelper.GetUserIdFromSession();

            newArticle.CategoryId = Convert.ToByte(article.CategoryId);


            _articleRepository.AddArticle(newArticle);
        }

        public void Update (ArticleViewModel article)
        {
            var existingData = _articleRepository.RetrieveAll().Where(a => a.ArticleId.ToString() == article.ArticleId).FirstOrDefault();
            if (existingData != null)
            {
                _mapper.Map(article, existingData);
                existingData.Title = article.Title;
                existingData.Body = article.Body;
                existingData.CategoryId = article.CategoryId;
                existingData.DateUpdated = DateTime.Now;
                existingData.UpdatedBy = _sessionHelper.GetUserIdFromSession();
                _articleRepository.UpdateArticle(existingData);
            }
        }

        public void Delete(ArticleViewModel article)
        {
            var existingData = _articleRepository.RetrieveAll().Where(a => a.ArticleId.ToString() == article.ArticleId).FirstOrDefault();
            if (existingData != null)
            {
                _mapper.Map (existingData, article);
                existingData.Status = false;
                _articleRepository.UpdateArticle(existingData);
            }
        }

        public IEnumerable<Category> GetCategories()
        {
            return _categoryRepository.RetrieveAll();
        }

        public IEnumerable<ArticleViewModel> GetArticles()
        {
            var data = _articleRepository.RetrieveAll().Select(s => new ArticleViewModel
            {
                ArticleId = s.ArticleId.ToString(),
                Title = s.Title,
                Body = s.Body,
                CategoryNavigation = _categoryRepository.RetrieveAll().Where(c => c.CategoryId == s.CategoryId).FirstOrDefault().CategoryName,
                DateUpdated = s.DateUpdated.HasValue ? s.DateUpdated.Value.ToString("MMM dd yyyy") : string.Empty,
                CreatedBy = _userRepository.GetUsers().Where(u => u.UserId == s.CreatedBy).FirstOrDefault().Username,
                CategoryId = s.CategoryId,
            });

            return data;
        }
    }
}
