using System;

namespace ASI.Basecode.Data.Models
{
    public partial class Article
    {
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Category { get; set; }
        public DateTime DateCreated { get; set; }
    }
}