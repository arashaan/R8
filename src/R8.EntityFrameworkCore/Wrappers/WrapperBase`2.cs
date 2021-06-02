using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using R8.Lib.Localization;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace R8.EntityFrameworkCore.Wrappers
{
    public class WrapperBase<TEntity, TStatus> : WrapperBase<TStatus>, IWrapperDatabaseBase where TEntity : class where TStatus : struct
    {
        public WrapperBase()
        {
        }

        public WrapperBase(TEntity entity)
        {
            Entity = entity;
        }

        public WrapperBase(TStatus status) : base(status)
        {
        }

        public WrapperBase(TStatus status, TEntity entity) : base(status)
        {
            Entity = entity;
        }

        public WrapperBase(TStatus status, ILocalizer localizer) : base(status, localizer)
        {
        }

        /// <summary>
        /// Gets or sets the message carries the result.
        /// </summary>
        [JsonProperty("message")]
        public virtual string Message { get; set; }

        /// <summary>
        /// Gets or sets the entity included in current wrapper
        /// </summary>
        /// <remarks>This property will be ignored on serialization.</remarks>
        [JsonIgnore]
        public virtual TEntity Entity { get; set; }

        public static implicit operator WrapperBase<TEntity, TStatus>(TStatus flag)
        {
            return new WrapperBase<TEntity, TStatus>(flag);
        }

        public static explicit operator TStatus(WrapperBase<TEntity, TStatus> src)
        {
            return src.Status;
        }

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

            //var entry = dbContext.Entry(this.Entity);
            //if (entry == null)
            //    return;

            //if (entry.State != EntityState.Added && entry.State != EntityState.Modified)
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

        public virtual void Deconstruct(out TStatus status, out TEntity source)
        {
            status = Status;
            source = this.Entity;
        }

        public override string ToString()
        {
            var text = Status.ToString();
            if (Entity != null)
                text += $" => {Entity}";

            return text;
        }
    }
}