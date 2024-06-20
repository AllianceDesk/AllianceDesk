﻿using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class ArticleCategory
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public List<Article> Articles { get; set; } = new List<Article>();
    }
}