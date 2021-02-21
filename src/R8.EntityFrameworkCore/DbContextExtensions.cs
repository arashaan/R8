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
    public static class DbContextBaseExtensions
    {
        public static void ScanConfigurations<TDbContext>(this ModelBuilder modelBuilder, TDbContext dbContext) where TDbContext : DbContext
        {
            modelBuilder.ApplyConfigurationsFromAssembly(dbContext.GetType().Assembly);
        }

        public static bool UnHide<TDbContext, TSource>(this TDbContext dbContext, TSource entity, Guid userId) where TDbContext : DbContext where TSource : IEntityBase
        {
            if (!entity.IsDeleted)
                return false;

            entity.IsDeleted = false;
            var entry = dbContext.Update(entity);
            entry.GenerateAudit(AuditFlags.UnDeleted, userId);

            return true;
        }

        public static bool ToggleHiding<TDbContext, TSource>(this TDbContext dbContext, TSource entity, Guid userId) where TDbContext : DbContext where TSource : IEntityBase
        {
            var flag = entity.IsDeleted
                ? AuditFlags.UnDeleted
                : AuditFlags.Deleted;
            entity.IsDeleted = !entity.IsDeleted;
            var entry = dbContext.Update(entity);
            entry.GenerateAudit(flag, userId);

            return true;
        }

        public static void GenerateAudit(this EntityEntry entry, AuditFlags flag, Guid userId)
        {
            var remoteIpAddress = HttpExtensions.GetIPAddress();
            var localIpAddress = HttpExtensions.GetLocalIPAddress();
            entry.GenerateAudit(flag, userId, remoteIpAddress, localIpAddress, null);
        }

        public static bool Update<TDbContext, TSource>(this TDbContext dbContext, TSource entity, Guid userId, out ValidatableResultCollection errors) where TDbContext : DbContext where TSource : IEntityBase
        {
            var isValid = entity.TryValidate(out errors);
            if (!isValid)
                return false;

            var entry = dbContext.Update(entity);
            entry.GenerateAudit(AuditFlags.Changed, userId);

            return true;
        }

        public static bool Hide<TDbContext, TSource>(this TDbContext dbContext, TSource entity, Guid userId) where TDbContext : DbContext where TSource : IEntityBase
        {
            if (entity.IsDeleted)
                return false;

            entity.IsDeleted = true;
            var entry = dbContext.Update(entity);
            entry.GenerateAudit(AuditFlags.Deleted, userId);

            return true;
        }

        public static bool Add<TDbContext, TSource>(this TDbContext dbContext, TSource entity, Guid userId, out ValidatableResultCollection errors) where TDbContext : DbContext where TSource : IEntityBase
        {
            var isValid = entity.TryValidate(out errors);
            if (!isValid)
                return false;

            var entry = dbContext.Add(entity);
            entry.GenerateAudit(AuditFlags.Created, userId);
            return true;
        }

        public static async Task<DatabaseSaveState> SaveAsync<TDbContext>(this TDbContext dbContext, CancellationToken cancellationToken = default) where TDbContext : DbContext
        {
            // TODO looking for a way to show possible exception for async method
            dbContext.ChangeTracker.DetectChanges();
            var canSave = dbContext.CanSave(out var changesCount, out var entries);
            if (!canSave)
                return DatabaseSaveState.NoNeedToSave;

            DatabaseSaveState result;
            try
            {
                dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                var changesInDatabase = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
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
                dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
            }

            return result;
        }

        public static DatabaseSaveState Save<TDbContext>(this TDbContext dbContext, out Exception? saveException) where TDbContext : DbContext
        {
            saveException = null;
            var canSave = dbContext.CanSave(out var changesCount, out var entries);
            if (!canSave)
                return DatabaseSaveState.NoNeedToSave;

            dbContext.ChangeTracker.DetectChanges();
            DatabaseSaveState result;
            try
            {
                dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                var changesInDatabase = dbContext.SaveChanges();
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
                dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
            }

            return result;
        }

        public static bool CanSave<TDbContext>(this TDbContext dbContext, out int changesCount, out List<EntityEntry> entries) where TDbContext : DbContext
        {
            entries = null;
            changesCount = 0;
            var allEntries = dbContext.ChangeTracker.Entries().ToList();
            if (!allEntries.Any())
                return false;

            entries = allEntries
                .Where(x => x.State != EntityState.Detached && x.State != EntityState.Unchanged)
                .ToList();
            changesCount = entries?.Count ?? 0;
            return changesCount != 0;
        }

        public static bool TryValidate<T>(this List<T> responses, out ValidatableResultCollection errors) where T : IResponseBaseDatabase
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