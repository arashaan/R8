using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace R8.EntityFrameworkCore.Wrappers
{
    public class WrapperCollectionBase<TStatus> : WrapperBase<TStatus>, IWrapperDatabaseBase
    {
        public WrapperCollectionBase()
        {
        }

        public WrapperCollectionBase(IEntityBaseIdentifier entity)
        {
            Entities.Add(entity);
        }

        public WrapperCollectionBase(IEnumerable<IEntityBaseIdentifier> collection)
        {
            if (collection?.Any() == true)
                Entities.AddRange(collection);
        }

        public List<IEntityBaseIdentifier> Entities { get; set; } = new List<IEntityBaseIdentifier>();

        public DatabaseSaveStatus? Save { get; private set; }

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
        public async Task SaveChangesAsync<TDbContext>(TDbContext dbContext, CancellationToken cancellationToken = default) where TDbContext : DbContext
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            //var entities = this.Entities.Select(dbContext.Entry).ToList();
            //if (!entities.Any())
            //    return;

            //if (!entities.Any(entry => entry.State == EntityState.Added || entry.State == EntityState.Modified))
            //    return;

            dbContext.ChangeTracker.DetectChanges();
            var entries = dbContext.ChangeTracker.Entries()
                .Where(x => x.State != EntityState.Detached && x.State != EntityState.Unchanged)
                .ToList();
            if (!entries.Any())
                return;

            var changesBeforeSave = entries.Count;
            var result = new DatabaseSaveStatus
            {
                EntityEntries = entries,
                ChangesBeforeSave = changesBeforeSave
            };

            try
            {
                dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                var changesAfterSave = await dbContext
                    .SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);
                if (changesAfterSave > 0)
                {
                    result.Save = changesAfterSave < changesBeforeSave
                        ? DatabaseSaveState.SavedPartially
                        : DatabaseSaveState.Saved;
                    result.ChangesAfterSave = changesAfterSave;
                }
                else
                {
                    result.Save = DatabaseSaveState.NotSaved;
                }
            }
            catch (Exception ex)
            {
                result.Save = DatabaseSaveState.SaveFailure;

                if (ex is DbUpdateConcurrencyException concurrencyException)
                    result.DbUpdateConcurrencyException = concurrencyException;

                if (ex is DbUpdateException dbUpdateException)
                    result.DbUpdateException = dbUpdateException;
            }
            finally
            {
                dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
            }

            this.Save = result;
        }
    }
}