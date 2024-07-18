using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class FavoriteRepository : BaseRepository, IFavoriteRepository
    {

        public FavoriteRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IEnumerable<Favorite> RetrieveAll()
        {
            return this.GetDbSet<Favorite>();
        }

        public void Add(Favorite favorite)
        {
            this.GetDbSet<Favorite>().Add(favorite);
            UnitOfWork.SaveChanges();
        }

        public void Delete(Favorite favorite)
        {
            this.GetDbSet<Favorite>().Remove(favorite);
            UnitOfWork.SaveChanges();
        }
    }
}
