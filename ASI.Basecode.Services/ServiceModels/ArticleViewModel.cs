using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class ArticleViewModel
    {
        public string ArticleId { get; set; }

        [Required(ErrorMessage = "Article Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Article Content is required")]
        public string Body { get; set; }
        public byte? CategoryId { get; set; }
        public string DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public string DateUpdated { get; set; }
        public string UpdatedBy { get; set; }

        public string CategoryNavigation { get; set; }
    }
}
