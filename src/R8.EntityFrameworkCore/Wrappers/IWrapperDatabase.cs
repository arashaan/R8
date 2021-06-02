using Microsoft.EntityFrameworkCore;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace R8.EntityFrameworkCore.Wrappers
{
    public interface IWrapperDatabaseBase
    {
        DatabaseSaveStatus? Save { get; }

        /// <summary>
        /// Save changes in Database.
        /// </summary>
        /// <param name="dbContext">A derived type of <see cref="DbContext"/>.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken" /> to observe while waiting for the task to complete. </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>
        ///     A task that represents the asynchronous save operation. The task result contains An <see cref="DatabaseSaveStatus"/>.
        /// </returns>
        /// <exception cref="DbUpdateException" />
        /// <exception cref="DbUpdateConcurrencyException" />
        Task SaveChangesAsync<TDbContext>(TDbContext dbContext, CancellationToken cancellationToken = default)
            where TDbContext : DbContext;
    }
}