using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Repositories
{
    public class ArticleRepository : BaseRepository, IArticleRepository
    {
        public ArticleRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IEnumerable<Article> RetrieveAll()
        {
            return this.GetDbSet<Article>();
        }

        public void AddArticle(Article article)
        {
            article.ArticleId = Guid.NewGuid();
            article.DateCreated = DateTime.Now;
            article.DateUpdated = DateTime.Now;

            this.GetDbSet<Article>().Add(article);
            UnitOfWork.SaveChanges();
        }

    }
}
