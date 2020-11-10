﻿using Microsoft.EntityFrameworkCore;

using R8.Lib;
using R8.Lib.Enums;
using R8.Lib.MethodReturn;

using System;
using System.Diagnostics;
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
        public string ConnectionString => Database.GetConnectionString();

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

        public bool Add<TSource>(TSource entity, Guid userId, out ValidatableResultCollection errors) where TSource : EntityBase
        {
            var isValid = TryValidate(entity, out errors);
            if (!isValid)
                return false;

            var entry = base.Add(entity);

            var frame = new StackTrace().GetFrame(1);
            entry.GenerateAudit(AuditFlags.Created, userId, null, null, frame);

            return true;
        }

        public bool UnHide<TSource>(TSource entity, Guid userId) where TSource : EntityBase
        {
            if (!entity.IsDeleted)
                return false;

            entity.IsDeleted = false;
            var entry = base.Update(entity);

            var frame = new StackTrace().GetFrame(1);
            entry.GenerateAudit(AuditFlags.UnDeleted, userId, null, null, frame);

            return true;
        }

        public virtual bool TryValidate(IResponseTrack response, out ValidatableResultCollection errors)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            errors = new ValidatableResultCollection();
            switch (response)
            {
                case ResponseCollection responseGroup when responseGroup.Results == null || !responseGroup.Results.Any():
                    return !errors.Any();

                case ResponseCollection responseGroup:
                    {
                        var results = from childResponse in responseGroup.Results
                                      let childEntity = GetIResponseUnderlyingEntity(childResponse.Value)
                                      where childEntity != null
                                      select childEntity;
                        var resultEntities = results.ToList();
                        if (resultEntities?.Any() != true)
                            return !errors.Any();

                        foreach (var resultEntity in resultEntities)
                        {
                            var isChildValid = TryValidate(resultEntity, out var tempChildErrors);
                            if (!isChildValid)
                                errors.AddRange(tempChildErrors);
                        }

                        break;
                    }
                case IResponseDatabase responseDatabase:
                    {
                        var childEntity = GetIResponseUnderlyingEntity(responseDatabase);
                        if (childEntity != null)
                        {
                            var isChildValid = TryValidate(childEntity, out var tempChildErrors);
                            if (!isChildValid)
                                errors.AddRange(tempChildErrors);
                        }
                        else
                        {
                            return !errors.Any();
                        }

                        break;
                    }
                default:
                    throw new Exception("Unable to determine type");
            }

            return !errors.Any();
        }

        private static object? GetIResponseUnderlyingEntity(IResponse childResponse)
        {
            if (childResponse.GetType() != typeof(Response<>))
                return null;

            var childEntityProp = childResponse.GetType().GetProperty(nameof(Response<EntityBase>.Result));
            return childEntityProp?.GetValue(childResponse);
        }

        public bool Update<TSource>(TSource entity, Guid userId, out ValidatableResultCollection errors) where TSource : EntityBase
        {
            var isValid = TryValidate(entity, out errors);
            if (!isValid)
                return false;

            var entry = base.Update(entity);

            var frame = new StackTrace().GetFrame(1);
            entry.GenerateAudit(AuditFlags.Changed, userId, null, null, frame);

            return true;
        }

        public bool ToggleHiding<TSource>(TSource entity, Guid userId) where TSource : EntityBase
        {
            var flag = entity.IsDeleted
                ? AuditFlags.UnDeleted
                : AuditFlags.Deleted;
            entity.IsDeleted = !entity.IsDeleted;
            var entry = base.Update(entity);

            var frame = new StackTrace().GetFrame(1);
            entry.GenerateAudit(flag, userId, null, null, frame);

            return true;
        }

        public bool Hide<TSource>(TSource entity, Guid userId) where TSource : EntityBase
        {
            if (entity.IsDeleted)
                return false;

            entity.IsDeleted = true;
            var entry = base.Update(entity);

            var frame = new StackTrace().GetFrame(1);
            entry.GenerateAudit(AuditFlags.Deleted, userId, null, null, frame);

            return true;
        }

        public new async Task<DatabaseSaveState> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var canSave = CanSave(out var changesCount);
            if (!canSave)
                return DatabaseSaveState.NoNeedToSave;

            base.ChangeTracker.DetectChanges();
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

        private bool CanSave(out int changesCount)
        {
            var entries = base
                .ChangeTracker
                .Entries()?
                .Where(x => x.State != EntityState.Detached && x.State != EntityState.Unchanged)
                .ToList();
            changesCount = entries?.Count ?? 0;
            return changesCount != 0;
        }

        public new DatabaseSaveState SaveChanges()
        {
            var canSave = CanSave(out var changesCount);
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
                Console.WriteLine(ex);
                result = DatabaseSaveState.SaveFailure;
            }
            finally
            {
                base.ChangeTracker.AutoDetectChangesEnabled = true;
            }

            return result;
        }

        public static bool TryValidate<TEntity>(TEntity entity, out ValidatableResultCollection errors)
          where TEntity : EntityBase
        {
            entity.Validate();
            errors = entity.ValidationErrors;

            if (errors?.Any() != true)
                return errors?.Count == 0;

            var ignoredNames = new[]
            {
                nameof(entity.Id),
                nameof(entity.Audits),
            };
            var ignored = errors.Where(x => ignoredNames.Contains(x.Name)).ToList();
            if (ignored?.Any() != true)
                return errors?.Count == 0;

            foreach (var result in ignored)
                errors.Remove(result);

            return errors?.Count == 0;
        }

        public static bool TryValidate(object entity, out ValidatableResultCollection errors)
        {
            errors = new ValidatableResultCollection();
            if (!(entity is EntityBase vEntity))
                return false;

            vEntity.Validate();
            errors = vEntity.ValidationErrors;

            if (errors?.Any() != true)
                return errors?.Count == 0;

            var ignoredNames = new[]
            {
                nameof(EntityBase.Id),
                nameof(EntityBase.Audits),
            };
            var ignored = errors.Where(x => ignoredNames.Contains(x.Name)).ToList();
            if (ignored?.Any() != true)
                return errors?.Count == 0;

            foreach (var result in ignored)
                errors.Remove(result);

            return errors?.Count == 0;
        }
    }
}