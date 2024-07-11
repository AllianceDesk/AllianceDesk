﻿using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IArticleService
    {
        IEnumerable<ArticleViewModel> RetrieveAll();
        void Add(ArticleViewModel article);
        IEnumerable<Category> GetCategories();
    }
}
