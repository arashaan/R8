using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using R8.Lib;
using R8.Lib.Enums;
using R8.Lib.MethodReturn;
using R8.Lib.Validatable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace R8.EntityFrameworkCore
{
    /// <summary>
    /// An pre-filled derived class of <see cref="DbContext"/>
    /// </summary>
    public abstract class DbContextBase : DbContext
    {
        protected DbContextBase() : this(new DbContextOptions<DbContextBase>())
        {
        }

        protected DbContextBase(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            if (Database.IsSqlServer())
                modelBuilder.AddCustomDbFunctions();

            base.OnModelCreating(modelBuilder);
        }

        public bool Add<TSource>(TSource entity, Guid userId, out ValidatableResultCollection errors) where TSource : IEntityBase
        {
            var isValid = entity.TryValidate(out errors);
            if (!isValid)
                return false;

            var entry = base.Add(entity);
            GenerateAudit(entry, AuditFlags.Created, userId);
            return true;
        }

        private static void GenerateAudit(EntityEntry entry, AuditFlags flag, Guid userId)
        {
            var remoteIpAddress = HttpExtensions.GetIPAddress();
            var localIpAddress = HttpExtensions.GetLocalIPAddress();
            entry.GenerateAudit(flag, userId, remoteIpAddress, localIpAddress, null);
        }

        public bool UnHide<TSource>(TSource entity, Guid userId) where TSource : IEntityBase
        {
            if (!entity.IsDeleted)
                return false;

            entity.IsDeleted = false;
            var entry = base.Update(entity);
            GenerateAudit(entry, AuditFlags.UnDeleted, userId);
            return true;
        }

        public bool Update<TSource>(TSource entity, Guid userId, out ValidatableResultCollection errors) where TSource : IEntityBase
        {
            var isValid = entity.TryValidate(out errors);
            if (!isValid)
                return false;

            var entry = base.Update(entity);
            GenerateAudit(entry, AuditFlags.Changed, userId);
            return true;
        }

        public bool ToggleHiding<TSource>(TSource entity, Guid userId) where TSource : IEntityBase
        {
            var flag = entity.IsDeleted
                ? AuditFlags.UnDeleted
                : AuditFlags.Deleted;
            entity.IsDeleted = !entity.IsDeleted;
            var entry = base.Update(entity);
            GenerateAudit(entry, flag, userId);
            return true;
        }

        public bool Hide<TSource>(TSource entity, Guid userId) where TSource : IEntityBase
        {
            if (entity.IsDeleted)
                return false;

            entity.IsDeleted = true;
            var entry = base.Update(entity);
            GenerateAudit(entry, AuditFlags.Deleted, userId);
            return true;
        }

        public new async Task<DatabaseSaveState> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // TODO looking for a way to show possible exception for async method
            base.ChangeTracker.DetectChanges();
            var canSave = CanSave(out var changesCount, out var entries);
            if (!canSave)
                return DatabaseSaveState.NoNeedToSave;

            DatabaseSaveState result;
            try
            {
                base.ChangeTracker.AutoDetectChangesEnabled = false;
                var changesInDatabase = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                if (changesInDatabase > 0)
                {
                    result = changesInDatabase == changesCount
                        ? DatabaseSaveState.Saved
                        : DatabaseSaveState.SavedWithErrors;
                }
                else
                {
                    result = DatabaseSaveState.NotSaved;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result = DatabaseSaveState.SaveFailure;
            }
            finally
            {
                base.ChangeTracker.AutoDetectChangesEnabled = true;
            }

            return result;
        }

        private bool CanSave(out int changesCount, out List<EntityEntry> entries)
        {
            entries = null;
            changesCount = 0;
            var allEntries = base.ChangeTracker.Entries().ToList();
            if (!allEntries.Any())
                return false;

            entries = allEntries
                .Where(x => x.State != EntityState.Detached && x.State != EntityState.Unchanged)
                .ToList();
            changesCount = entries?.Count ?? 0;
            return changesCount != 0;
        }

        public new DatabaseSaveState SaveChanges(out Exception? saveException)
        {
            saveException = null;
            var canSave = CanSave(out var changesCount, out var entries);
            if (!canSave)
                return DatabaseSaveState.NoNeedToSave;

            base.ChangeTracker.DetectChanges();
            DatabaseSaveState result;
            try
            {
                base.ChangeTracker.AutoDetectChangesEnabled = false;
                var changesInDatabase = base.SaveChanges();
                if (changesInDatabase > 0)
                {
                    result = changesInDatabase == changesCount
                        ? DatabaseSaveState.Saved
                        : DatabaseSaveState.SavedWithErrors;
                }
                else
                {
                    result = DatabaseSaveState.NotSaved;
                }
            }
            catch (Exception ex)
            {
                saveException = ex;
                result = DatabaseSaveState.SaveFailure;
            }
            finally
            {
                base.ChangeTracker.AutoDetectChangesEnabled = true;
            }

            return result;
        }

        public static bool TryValidate<T>(List<T> responses, out ValidatableResultCollection errors) where T : IResponseBaseDatabase
        {
            errors = new ValidatableResultCollection();
            if (responses == null || responses.Count == 0)
                return false;

            var newList = responses
                .Cast<IResponseBaseDatabase>()
                .Where(responseBaseDatabase => responseBaseDatabase.GetType()
                    .GetPublicProperties()
                    .Any(propertyInfo => propertyInfo.Name == nameof(ResponseBase<object, string>.Result) &&
                                         propertyInfo.GetValue(responseBaseDatabase) is IEntityBase))
                .ToList();
            if (newList.Count == 0)
                return false;

            foreach (var value in from response in newList
                                  let prop = response.GetType()
                                      .GetPublicProperties()
                                      .Find(propertyInfo => propertyInfo.Name == nameof(ResponseBase<object, string>.Result) &&
                                                            propertyInfo.GetValue(response) is IEntityBase)
                                  select (IEntityBase)prop.GetValue(response)
                into value
                                  where value != null
                                  select value)
            {
                value.TryValidate(out var tempErrors);
                errors.AddRange(tempErrors);
            }

            return errors?.Count == 0;
        }
    }
}