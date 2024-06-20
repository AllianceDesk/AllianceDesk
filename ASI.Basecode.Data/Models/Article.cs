using System;

namespace ASI.Basecode.Data.Models
{
    public partial class Article
    {
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int CategoryId { get; set; }
        public ArticleCategory Category { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool isPublished { get; set; }

        public int AuthorId { get; set; }
        public User Author { get; set; }

        public DateTime LastUpdatedAt { get; set;}
    }
}