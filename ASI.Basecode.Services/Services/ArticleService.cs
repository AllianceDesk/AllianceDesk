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
        private readonly ISessionHelper _sessionHelper;
        private readonly IUserRepository _userRepository;
        private readonly IFavoriteRepository _favoriteRepository;

        public ArticleService(IMapper mapper,
                                IArticleRepository articleRepository,
                                ICategoryRepository categoryRepository,
                                ISessionHelper sessionHelper,
                                IUserRepository userRepository,
                                IFavoriteRepository favoriteRepository)
        {
            _mapper = mapper;
            _articleRepository = articleRepository;
            _categoryRepository = categoryRepository;
            _sessionHelper = sessionHelper;
            _userRepository = userRepository;
            _favoriteRepository = favoriteRepository;
        }
        public IEnumerable<ArticleViewModel> RetrieveAll()
        {

            var data = _articleRepository.RetrieveAll().Where(a => a.Status == true).Select(s => new ArticleViewModel
            {
                ArticleId = s.ArticleId.ToString(),
                Title = s.Title,
                Body = s.Body,
                CategoryNavigation = _categoryRepository.RetrieveAll().Where(c => c.CategoryId == s.CategoryId).FirstOrDefault().CategoryName,
                DateUpdated = s.DateUpdated.HasValue ? s.DateUpdated.Value.ToString("MMM dd yyyy") : string.Empty,
                UpdatedBy = _userRepository.GetUsers().Where(c => c.UserId == s.UpdatedBy).FirstOrDefault().Username,
                CategoryId = s.CategoryId,
            });

            return data;
        }

        public void Add(ArticleViewModel article)
        {
            var articleExist = _articleRepository.RetrieveAll().Any(a => a.Title == article.Title && a.Body == article.Body);
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article), "ArticleViewModel cannot be null");
            }
            
            if (!articleExist)
            {
                var newArticle = new Article();
                newArticle.ArticleId = Guid.NewGuid();
                newArticle.Title = article.Title;
                newArticle.Body = article.Body;
                newArticle.DateCreated = DateTime.Now;
                newArticle.DateUpdated = DateTime.Now;
                newArticle.Status = true;

                newArticle.CreatedBy = _sessionHelper.GetUserIdFromSession();
                newArticle.UpdatedBy = _sessionHelper.GetUserIdFromSession();

                newArticle.CategoryId = Convert.ToByte(article.CategoryId);

                _articleRepository.AddArticle(newArticle);
            }
        }

        public void Update(ArticleViewModel article)
        {
            var existingData = _articleRepository.RetrieveAll().Where(a => a.ArticleId.ToString() == article.ArticleId).FirstOrDefault();
            if (existingData != null)
            {
                _mapper.Map(article, existingData);
                existingData.UpdatedBy = _sessionHelper.GetUserIdFromSession();
                existingData.Title = article.Title;
                existingData.Body = article.Body;
                existingData.CategoryId = article.CategoryId;
                existingData.DateUpdated = DateTime.Now;
                _articleRepository.UpdateArticle(existingData);
            }
        }

        public void Delete(ArticleViewModel article)
        {
            var existingData = _articleRepository.RetrieveAll().Where(a => a.ArticleId.ToString() == article.ArticleId).FirstOrDefault();
            if (existingData != null)
            {
                existingData.Status = false;
                existingData.UpdatedBy = _sessionHelper.GetUserIdFromSession();
                _articleRepository.UpdateArticle(existingData);
            }
        }

        public IEnumerable<Category> GetCategories()
        {
            return _categoryRepository.RetrieveAll();
        }

        public IEnumerable<ArticleViewModel> RetrieveFavorites()
        {
            var data = _favoriteRepository.RetrieveAll().Where(f => f.UserId == _sessionHelper.GetUserIdFromSession()).Select(s => new ArticleViewModel
            {
                ArticleId = s.ArticleId.ToString(),
                Title = _articleRepository.RetrieveAll().Where(a => a.ArticleId == s.ArticleId).FirstOrDefault().Title,
                Body = _articleRepository.RetrieveAll().Where(a => a.ArticleId == s.ArticleId).FirstOrDefault().Body,
                CategoryNavigation = _categoryRepository.RetrieveAll().Where(c => 
                                    c.CategoryId == _articleRepository.RetrieveAll().Where(a => a.ArticleId == s.ArticleId).FirstOrDefault().CategoryId)
                                    .FirstOrDefault().CategoryName,
                DateUpdated = _articleRepository.RetrieveAll().Where(a => a.ArticleId == s.ArticleId).FirstOrDefault().DateUpdated.HasValue ? 
                                    _articleRepository.RetrieveAll().Where(a => a.ArticleId == s.ArticleId).FirstOrDefault().DateUpdated.Value.ToString("MMM dd yyyy") : string.Empty,
                UpdatedBy = _userRepository.GetUsers().Where(c => c.UserId == 
                                    _articleRepository.RetrieveAll().Where(a => a.ArticleId == s.ArticleId)
                                    .FirstOrDefault().UpdatedBy).FirstOrDefault().Username,
                CategoryId = _articleRepository.RetrieveAll().Where(a => a.ArticleId == s.ArticleId).FirstOrDefault().CategoryId,
            });

            return data;
        }

        public int GetUserFavoriteCount()
        {
            return _favoriteRepository.RetrieveAll().Count(a => a.UserId == _sessionHelper.GetUserIdFromSession());
        }

        public void AddFavorite(string articleId)
        {
            var existingFavorite = _favoriteRepository.RetrieveAll().Any(f => f.ArticleId.ToString() == articleId && f.UserId.ToString() == _sessionHelper.GetUserIdFromSession().ToString());
            if (!existingFavorite)
            {
                var newFavorite = new Favorite();

                newFavorite.FavoriteId = Guid.NewGuid();
                newFavorite.UserId = _sessionHelper.GetUserIdFromSession();
                newFavorite.ArticleId = Guid.Parse(articleId);

                _favoriteRepository.Add(newFavorite);
            }
        }

        public void DeleteFavorite(string articleId)
        {
            var existingFavorite = _favoriteRepository.RetrieveAll().Any(f => f.ArticleId.ToString() == articleId && f.UserId.ToString() == _sessionHelper.GetUserIdFromSession().ToString());
            if (existingFavorite)
            {
                _favoriteRepository.Delete(articleId);
            }
        }
    }
}
