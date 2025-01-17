﻿using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    /// <summary>
    /// Unit of Work Interface
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Gets the database context
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        DbContext Database { get; }
        /// <summary>
        /// Saves the changes to database
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        /// <returns></returns>
        Task SaveChangesAsync();
    }
}
