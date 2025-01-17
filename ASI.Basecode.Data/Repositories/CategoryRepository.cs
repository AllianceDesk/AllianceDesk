﻿using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {

        public CategoryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IEnumerable<Category> RetrieveAll()
        {
            return this.GetDbSet<Category>();
        }

        public Category GetCategoryById(byte categoryId)
        {
            return this.GetDbSet<Category>().FirstOrDefault(x => x.CategoryId == categoryId);
        }
    }
}
