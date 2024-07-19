﻿using ASI.Basecode.Data.Interfaces;
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

        public ArticleService (IMapper mapper, 
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
            var articleExist = _articleRepository.RetrieveAll().Where(a => a.Title == article.Title && a.Body == article.Body);
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article), "ArticleViewModel cannot be null");
            }
            
            if (articleExist == null)
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

        public void Update (ArticleViewModel article)
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

        public void AddFavorite (ArticleViewModel article)
        {
            var existingFavorite = _favoriteRepository.RetrieveAll().Where(f => f.ArticleId.ToString() == article.ArticleId && f.UserId.ToString() == _sessionHelper.GetUserIdFromSession().ToString());
            if (existingFavorite == null)
            {
                var newFavorite = new Favorite();

                newFavorite.FavoriteId = Guid.NewGuid();
                newFavorite.UserId = _sessionHelper.GetUserIdFromSession();
                newFavorite.ArticleId = Guid.Parse(article.ArticleId);

                _favoriteRepository.Add(newFavorite);
            }
        }

        public void RemoveFavorite (ArticleViewModel article)
        {
            var existingFavorite = _favoriteRepository.RetrieveAll().Where(f => f.ArticleId.ToString() == article.ArticleId && f.UserId.ToString() == _sessionHelper.GetUserIdFromSession().ToString());
            if (existingFavorite != null)
            {
                _favoriteRepository.Delete(article.ArticleId);
            }
        }
    }
}
