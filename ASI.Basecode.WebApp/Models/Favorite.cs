using System;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Models
{
    public partial class Favorite
    {
        public Guid FavoriteId { get; set; }
        public Guid UserId { get; set; }
        public Guid ArticleId { get; set; }

        public virtual Article Article { get; set; }
        public virtual User User { get; set; }
    }
}
