using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using R8.Lib;
using R8.Lib.Enums;
using R8.Lib.MethodReturn;

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

        public bool Add<TSource>(TSource entity, Guid userId, out ValidatableResultCollection errors) where TSource : IEntityBase
        {
            var isValid = TryValidate(entity, out errors);
            if (!isValid)
                return false;

            var entry = base.Add(entity);
            var frame = new StackTrace().GetFrame(1);
            GenerateAudit(entry, AuditFlags.Created, userId, frame);
            return true;
        }

        private static void GenerateAudit(EntityEntry entry, AuditFlags flag, Guid userId, StackFrame frame)
        {
            var remoteIpAddress = HttpExtensions.GetIPAddress();
            var localIpAddress = HttpExtensions.GetLocalIPAddress();
            entry.GenerateAudit(flag, userId, remoteIpAddress, localIpAddress, null, frame);
        }

        public bool UnHide<TSource>(TSource entity, Guid userId) where TSource : IEntityBase
        {
            if (!entity.IsDeleted)
                return false;

            entity.IsDeleted = false;
            var entry = base.Update(entity);
            var frame = new StackTrace().GetFrame(1);
            GenerateAudit(entry, AuditFlags.UnDeleted, userId, frame);
            return true;
        }

        public virtual bool TryValidate(IResponseTrack response, out ValidatableResultCollection errors)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            errors = new ValidatableResultCollection();
            switch (response)
            {
                case ResponseCollection responseGroup when responseGroup.Results?.Any() != true:
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
                case IResponseBaseDatabase responseDatabase:
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

        private static object? GetIResponseUnderlyingEntity(IResponseBase childResponseBase)
        {
            if (childResponseBase.GetType() != typeof(ResponseBase<>))
                return null;

            var childEntityProp = childResponseBase.GetType().GetProperty(nameof(ResponseBase<IEntityBase>.Result));
            return childEntityProp?.GetValue(childResponseBase);
        }

        public bool Update<TSource>(TSource entity, Guid userId, out ValidatableResultCollection errors) where TSource : IEntityBase
        {
            var isValid = TryValidate(entity, out errors);
            if (!isValid)
                return false;

            var entry = base.Update(entity);
            var frame = new StackTrace().GetFrame(1);
            GenerateAudit(entry, AuditFlags.Changed, userId, frame);
            return true;
        }

        public bool ToggleHiding<TSource>(TSource entity, Guid userId) where TSource : IEntityBase
        {
            var flag = entity.IsDeleted
                ? AuditFlags.UnDeleted
                : AuditFlags.Deleted;
            entity.IsDeleted = !entity.IsDeleted;
            var entry = base.Update(entity);
            var frame = new StackTrace().GetFrame(1);
            GenerateAudit(entry, flag, userId, frame);
            return true;
        }

        public bool Hide<TSource>(TSource entity, Guid userId) where TSource : IEntityBase
        {
            if (entity.IsDeleted)
                return false;

            entity.IsDeleted = true;
            var entry = base.Update(entity);
            var frame = new StackTrace().GetFrame(1);
            GenerateAudit(entry, AuditFlags.Deleted, userId, frame);
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
                nameof(IEntityBase.Id),
                nameof(IEntityBase.Audits),
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