using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class Favorites
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ArticleId { get; set; }
    }
}
