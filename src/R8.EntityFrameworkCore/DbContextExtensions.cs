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
        /// <summary>
        /// Applies configuration from all <see cref="IEntityTypeConfiguration{TEntity}" />.
        /// instances that are defined in provided assembly.
        /// </summary>
        /// <typeparam name="TDbContext">A derived type of <see cref="DbContext"/></typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="dbContext"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ScanConfigurations<TDbContext>(this ModelBuilder modelBuilder, TDbContext dbContext) where TDbContext : DbContext
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            modelBuilder.ApplyConfigurationsFromAssembly(dbContext.GetType().Assembly);
        }

        /// <summary>
        /// Sets Auto-Increment for specific Property in given types.
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="property"></param>
        /// <param name="start"></param>
        /// <param name="entityTypes"></param>
        /// <returns></returns>
        public static ModelBuilder HasAutoIncrementColumn(this ModelBuilder modelBuilder, string property, long start, params Type[] entityTypes)
        {
            if (entityTypes == null || !entityTypes.Any())
                return modelBuilder;

            foreach (var entityType in entityTypes)
            {
                var tableName = entityType.Name;
                modelBuilder.HasSequence<long>($"{tableName}_seq", schema: "dbo")
                    .StartsAt(start)
                    .IncrementsBy(1);

                modelBuilder.Entity(entityType)
                    .Property(property)
                    .HasDefaultValueSql($"NEXT VALUE FOR dbo.{tableName}_seq");
            }

            return modelBuilder;
        }

        /// <summary>
        /// Sets <see cref="EntityBase.IsDeleted"/> value to <c>false</c> in <see cref="EntityBase"/> typed class, based on <see cref="EntityBase"/> strategy.
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity"></param>
        /// <param name="userId"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="bool"/> that indicates operations if successfully done.</returns>
        /// <remarks>In <see cref="EntityBase"/> strategy, any row will not be deleted, Only <see cref="EntityBase.IsDeleted"/> will be set to <c>true</c>.</remarks>
        public static bool UnHide<TDbContext, TSource>(this TDbContext dbContext, TSource entity, Guid? userId = null) where TDbContext : DbContext where TSource : IEntityBase
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (!entity.IsDeleted)
                return false;

            entity.IsDeleted = false;
            var entry = dbContext.Update(entity);
            dbContext.GenerateAudit(entry, AuditFlags.UnDeleted, userId);

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool ToggleHiding<TDbContext, TSource>(this TDbContext dbContext, TSource entity, Guid? userId = null) where TDbContext : DbContext where TSource : IEntityBase
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var flag = entity.IsDeleted
                ? AuditFlags.UnDeleted
                : AuditFlags.Deleted;
            entity.IsDeleted = !entity.IsDeleted;
            var entry = dbContext.Update(entity);
            dbContext.GenerateAudit(entry, flag, userId);

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity"></param>
        /// <param name="errors"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static bool Update<TDbContext, TSource>(this TDbContext dbContext, TSource entity, out ValidatableResultCollection errors) where TDbContext : DbContext where TSource : IEntityBase
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var isValid = entity.TryValidate(out errors);
            if (!isValid)
                return false;

            var entry = dbContext.Update(entity);
            dbContext.GenerateAudit(entry, AuditFlags.Changed, null);

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity"></param>
        /// <param name="userId"></param>
        /// <param name="errors"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static bool Update<TDbContext, TSource>(this TDbContext dbContext, TSource entity, Guid? userId, out ValidatableResultCollection errors) where TDbContext : DbContext where TSource : IEntityBase
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var isValid = entity.TryValidate(out errors);
            if (!isValid)
                return false;

            var entry = dbContext.Update(entity);
            dbContext.GenerateAudit(entry, AuditFlags.Changed, userId);

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity"></param>
        /// <param name="userId"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static bool Hide<TDbContext, TSource>(this TDbContext dbContext, TSource entity, Guid? userId = null) where TDbContext : DbContext where TSource : IEntityBase
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            if (entity.IsDeleted)
                return false;

            entity.IsDeleted = true;
            var entry = dbContext.Update(entity);
            dbContext.GenerateAudit(entry, AuditFlags.Deleted, userId);

            return true;
        }

        /// <summary>
        ///     <para>
        ///         Begins tracking the given entity, and any other reachable entities that are
        ///         not already being tracked, in the <see cref="EntityState.Added" /> state such that
        ///         they will be inserted into the database when <see cref="Save()" /> is called.
        ///     </para>
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <typeparam name="TEntity"> The type of the entity. </typeparam>
        /// <param name="dbContext">A <see cref="DbContext"/> instance.</param>
        /// <param name="entity"> The entity to add. </param>
        /// <param name="userId">A <see cref="Guid"/> id that representing maintainer internal id.</param>
        /// <param name="errors">An output <see cref="ValidatableResultCollection"/> object when you get error.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>You'll get <see cref="true"/> when operation got success.</returns>
        public static bool Add<TDbContext, TEntity>(this TDbContext dbContext, TEntity entity, Guid userId, out ValidatableResultCollection errors) where TDbContext : DbContext where TEntity : IEntityBase
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var isValid = entity.TryValidate(out errors);
            if (!isValid)
                return false;

            var entry = dbContext.Add(entity);
            dbContext.GenerateAudit(entry, AuditFlags.Created, userId);

            return true;
        }

        /// <summary>
        ///     <para>
        ///         Begins tracking the given entity, and any other reachable entities that are
        ///         not already being tracked, in the <see cref="EntityState.Added" /> state such that
        ///         they will be inserted into the database when <see cref="Save()" /> is called.
        ///     </para>
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <typeparam name="TEntity"> The type of the entity. </typeparam>
        /// <param name="dbContext">A <see cref="DbContext"/> instance.</param>
        /// <param name="entity"> The entity to add. </param>
        /// <param name="errors">An output <see cref="ValidatableResultCollection"/> object when you get error.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>You'll get <see cref="true"/> when operation got success.</returns>
        public static bool Add<TDbContext, TEntity>(this TDbContext dbContext, TEntity entity, out ValidatableResultCollection errors) where TDbContext : DbContext where TEntity : IEntityBase
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var isValid = entity.TryValidate(out errors);
            if (!isValid)
                return false;

            var entry = dbContext.Add(entity);
            dbContext.GenerateAudit(entry, AuditFlags.Created, null);

            return true;
        }

        /// <summary>
        ///     <para>
        ///         Saves all changes made in this context to the database.
        ///     </para>
        ///     <para>
        ///         This method will automatically call <see cref="ChangeTracker.DetectChanges" /> to discover any
        ///         changes to entity instances before saving to the underlying database. This can be disabled via
        ///         <see cref="ChangeTracker.AutoDetectChangesEnabled" />.
        ///     </para>
        ///     <para>
        ///         Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///         that any asynchronous operations have completed before calling another method on this context.
        ///     </para>
        /// </summary>
        /// <param name="dbContext">A derived type of <see cref="DbContext"/>.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken" /> to observe while waiting for the task to complete. </param>
        /// <returns>
        ///     A task that represents the asynchronous save operation. The task result contains An <see cref="DatabaseSaveStatus"/>.
        /// </returns>
        /// <exception cref="DbUpdateException">
        ///     An error is encountered while saving to the database.
        /// </exception>
        /// <exception cref="DbUpdateConcurrencyException">
        ///     A concurrency violation is encountered while saving to the database.
        ///     A concurrency violation occurs when an unexpected number of rows are affected during save.
        ///     This is usually because the data in the database has been modified since it was loaded into memory.
        /// </exception>
        public static async Task<DatabaseSaveStatus> SaveAsync<TDbContext>(this TDbContext dbContext, CancellationToken cancellationToken = default) where TDbContext : DbContext
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            dbContext.ChangeTracker.DetectChanges();
            var canSave = dbContext.NeedSave(out var changesCount, out var entries);
            if (!canSave)
                return new DatabaseSaveStatus { Save = DatabaseSaveState.NoNeedToSave };

            var result = new DatabaseSaveStatus
            {
                EntityEntries = entries,
                ChangesBeforeSave = changesCount
            };

            try
            {
                dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                var changesInDatabase = await dbContext
                    .SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);
                if (changesInDatabase > 0)
                {
                    result.Save = changesInDatabase == changesCount
                        ? DatabaseSaveState.Saved
                        : DatabaseSaveState.SavedWithErrors;
                    result.ChangesAfterSave = changesInDatabase;
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

            return result;
        }

        /// <summary>
        ///     <para>
        ///         Saves all changes made in this context to the database.
        ///     </para>
        ///     <para>
        ///         This method will automatically call <see cref="ChangeTracker.DetectChanges" /> to discover any
        ///         changes to entity instances before saving to the underlying database. This can be disabled via
        ///         <see cref="ChangeTracker.AutoDetectChangesEnabled" />.
        ///     </para>
        /// </summary>
        /// <param name="dbContext">A derived type of <see cref="DbContext"/>.</param>
        /// <returns>
        ///     An <see cref="DatabaseSaveStatus"/>.
        /// </returns>
        /// <exception cref="DbUpdateException">
        ///     An error is encountered while saving to the database.
        /// </exception>
        /// <exception cref="DbUpdateConcurrencyException">
        ///     A concurrency violation is encountered while saving to the database.
        ///     A concurrency violation occurs when an unexpected number of rows are affected during save.
        ///     This is usually because the data in the database has been modified since it was loaded into memory.
        /// </exception>
        public static DatabaseSaveStatus Save<TDbContext>(this TDbContext dbContext) where TDbContext : DbContext
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            dbContext.ChangeTracker.DetectChanges();
            var canSave = dbContext.NeedSave(out var changesCount, out var entries);
            if (!canSave)
                return new DatabaseSaveStatus { Save = DatabaseSaveState.NoNeedToSave };

            var result = new DatabaseSaveStatus
            {
                EntityEntries = entries,
                ChangesBeforeSave = changesCount
            };

            try
            {
                dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                var changesInDatabase = dbContext.SaveChanges();
                if (changesInDatabase > 0)
                {
                    result.Save = changesInDatabase == changesCount
                        ? DatabaseSaveState.Saved
                        : DatabaseSaveState.SavedWithErrors;
                    result.ChangesAfterSave = changesInDatabase;
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

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="changesCount"></param>
        /// <param name="entries"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static bool NeedSave<TDbContext>(this TDbContext dbContext, out int changesCount, out List<EntityEntry> entries) where TDbContext : DbContext
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

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

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="responses"></param>
        /// <param name="errors"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static bool TryValidate<T>(this List<T> responses, out ValidatableResultCollection errors) where T : IResponseBaseDatabase
        {
            if (responses == null) throw new ArgumentNullException(nameof(responses));

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