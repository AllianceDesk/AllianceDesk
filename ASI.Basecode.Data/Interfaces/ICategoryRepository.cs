using System;
using System.Collections.Generic;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> RetrieveAll();

        Category GetCategoryById(byte categoryId);
    }
}