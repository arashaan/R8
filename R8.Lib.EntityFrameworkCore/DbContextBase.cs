using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using Newtonsoft.Json;

using R8.Lib.Enums;
using R8.Lib.MethodReturn;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace R8.Lib.EntityFrameworkCore
{
    /// <summary>
    /// An pre-filled derived class of <see cref="DbContext"/>
    /// </summary>
    public abstract class DbContextBase : DbContext
    {
        public string ConnectionString { get; set; }

        protected DbContextBase() : this(new DbContextOptions<DbContextBase>())
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected DbContextBase(DbContextOptions options) : base(options)
        {
        }

        // public static JsonSerializerSettings AuditSerializerSettings
        // {
        //     get
        //     {
        //         var jsonSettings = JsonExtensions.CustomJsonSerializerSettings.Settings;
        //         jsonSettings.Formatting = Formatting.None;
        //         jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        //         return jsonSettings;
        //     }
        // }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            if (Database.IsSqlServer())
                modelBuilder.AddCustomDbFunctions();

            base.OnModelCreating(modelBuilder);
        }

        private void DetachCore(EntityEntry entry)
        {
            Entry(entry.Entity).State = entry.State switch
            {
                EntityState.Added => EntityState.Detached,
                EntityState.Modified => EntityState.Unchanged,
                EntityState.Deleted => EntityState.Unchanged,
                _ => Entry(entry.Entity).State
            };
        }

        public virtual Response<TSource> TryAdd<TSource>(TSource entity, Guid? userId, [CallerMemberName] string caller = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0) where TSource : EntityBase
        {
            var isValid = TryValidate(entity, out var errors);
            if (!isValid)
                return new Response<TSource>(Flags.ModelIsNotValid, entity, errors);

            var model = base.Add(entity);
            GenerateAudit(model, userId, caller, callerPath, callerLine);

            return new Response<TSource>(Flags.Success, model.Entity);
        }

        public Flags TryUnHideBase<TSource>(TSource entity)
            where TSource : EntityBase
        {
            if (!entity.IsDeleted)
                return Flags.NotDeleted;

            entity.IsDeleted = false;
            base.Update(entity);
            return Flags.Success;
        }

        public virtual Response<TSource> TryUnHide<TSource>(TSource entity, Guid? userId, [CallerMemberName] string caller = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0)
          where TSource : EntityBase
        {
            var hide = TryUnHideBase(entity);
            if (!entity.IsDeleted)
                return new Response<TSource>(Flags.NotDeleted, entity);

            entity.IsDeleted = false;
            var entry = base.Update(entity);
            GenerateAudit(entry, userId, caller, callerPath, callerLine);

            return new Response<TSource>(Flags.Success, entry.Entity);
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

        public static object? GetIResponseUnderlyingEntity(IResponse childResponse)
        {
            if (childResponse.GetType() == typeof(Response<>))
            {
                var childEntityProp = childResponse.GetType().GetProperty(nameof(Response<EntityBase>.Result));
                return childEntityProp == null
                    ? null
                    : childEntityProp.GetValue(childResponse);
            }
            else
            {
                return null;
            }
        }

        public virtual Response<TSource> TryUpdate<TSource>(TSource entity, Guid? userId, [CallerMemberName] string caller = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0) where TSource : EntityBase
        {
            var isValid = TryValidate(entity, out var errors);
            if (!isValid)
                return new Response<TSource>(Flags.ModelIsNotValid, entity, errors);

            var entry = base.Update(entity);
            GenerateAudit(entry, userId, caller, callerPath, callerLine);

            var result = new Response<TSource>(Flags.Success, entry.Entity);
            return result;
        }

        public virtual Response<TSource> TryHideUnHide<TSource>(TSource entity, Guid? userId, [CallerMemberName] string caller = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0) where TSource : EntityBase
        {
            entity.IsDeleted = !entity.IsDeleted;
            var entry = base.Update(entity);
            GenerateAudit(entry, userId, caller, callerPath, callerLine);

            return new Response<TSource>(Flags.Success, entity);
        }

        public virtual Response<TSource> TryHide<TSource>(TSource entity, Guid? userId, [CallerMemberName] string caller = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0) where TSource : EntityBase
        {
            if (entity.IsDeleted)
                return new Response<TSource>(Flags.AlreadyDeleted, entity);

            entity.IsDeleted = true;
            var entry = base.Update(entity);
            GenerateAudit(entry, userId, caller, callerPath, callerLine);

            return new Response<TSource>(Flags.Success, entry.Entity);
        }

        public new async Task<DatabaseSaveState> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var hasSavability = CheckSavability(out var changesCount);
            if (!hasSavability)
                return DatabaseSaveState.NoNeedToSave;

            base.ChangeTracker.DetectChanges();
            DatabaseSaveState result;
            try
            {
                base.ChangeTracker.AutoDetectChangesEnabled = false;
                var changesInDatabase = await base.SaveChangesAsync(cancellationToken);
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

        private bool CheckSavability(out int changes)
        {
            var entityEntries = base.ChangeTracker.Entries().ToList();
            var changesCount = entityEntries.Where(x => x.State != EntityState.Detached && x.State != EntityState.Unchanged).ToList();
            changes = changesCount.Count;
            return changesCount.Count != 0;
        }

        public new DatabaseSaveState SaveChanges()
        {
            var hasSavability = CheckSavability(out var changesCount);
            if (!hasSavability)
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

        public virtual void DeletePermanently<TSource>(TSource entity) where TSource : class
        {
            Entry(entity).State = EntityState.Deleted;
        }

        private void GenerateAudit(EntityEntry entry, Guid? userId, string caller, string callerPath, int callerLine)
        {
            AuditFlags flag;
            switch (entry.State)
            {
                case EntityState.Deleted:
                    flag = AuditFlags.Deleted;
                    break;

                case EntityState.Modified:
                    flag = AuditFlags.Changed;
                    break;

                case EntityState.Added:
                    flag = AuditFlags.Created;
                    break;

                case EntityState.Detached:
                case EntityState.Unchanged:
                default:
                    return;
            }

            if (!(entry.Entity is EntityBase entityBase))
                return;

            FindChanges(entry, out var oldValues, out var newValues);
            var audit = new Audit(userId, flag, entityBase.Id, caller, $"{callerPath}::{callerLine}", oldValues, newValues);
            entityBase.Audits ??= new AuditCollection();
            entityBase.Audits.Add(audit);
        }

        public static void FindChanges(EntityEntry entry, out Dictionary<string, object> oldValues, out Dictionary<string, object> newValues)
        {
            oldValues = new Dictionary<string, object>();
            newValues = new Dictionary<string, object>();
            if (entry.State != EntityState.Modified && entry.State != EntityState.Added)
                return;

            var propertyEntries = entry.Members
                .Where(x => x.Metadata.Name != nameof(EntityBase.Id)
                            && x.Metadata.Name != nameof(EntityBase.Audits)
                            && x.Metadata.Name != nameof(EntityBase.RowVersion)
                            && x is PropertyEntry)
                .Cast<PropertyEntry>()
                .ToList();
            if (!propertyEntries.Any())
                return;

            foreach (var propertyEntry in propertyEntries)
            {
                var propertyName = propertyEntry.Metadata.Name;
                var newValue = propertyEntry.CurrentValue;
                var oldValue = propertyEntry.OriginalValue;

                if (newValue == null && oldValue == null)
                    continue;

                if (newValue?.Equals(oldValue) == true)
                    continue;

                oldValues.Add(propertyName, oldValue);
                newValues.Add(propertyName, newValue);
            }
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
                // nameof(entity.SafetyHash)
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