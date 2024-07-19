using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IFavoriteRepository
    {
        IEnumerable<Favorite> RetrieveAll();
        void Add(Favorite favorite);
        void Delete(string articleId);
    }
}
