using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

using Newtonsoft.Json;

using R8.Lib.AspNetCore.Base;
using R8.Lib.Enums;
using R8.Lib.MethodReturn;

using System;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace R8.Lib.AspNetCore.EntityFrameworkCore
{
    /// <summary>
    /// An pre-filled derived class of <see cref="DbContext"/>
    /// </summary>
    public abstract class DbContextBase : DbContext
    {
        public IHttpContextAccessor HttpContextAccessor { get; set; }
        public string ConnectionString { get; set; }

        protected DbContextBase() : this(new DbContextOptions<DbContextBase>())
        {
        }

        protected DbContextBase(DbContextOptions options) : base(options)
        {
            this.HttpContextAccessor = this.GetService<IHttpContextAccessor>();
        }

        protected DbContextBase(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            this.HttpContextAccessor = httpContextAccessor;
        }

        public static JsonSerializerSettings AuditSerializerSettings
        {
            get
            {
                var jsonSettings = JsonSettingsExtensions.JsonNetSettings;
                jsonSettings.Formatting = Formatting.None;
                jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                return jsonSettings;
            }
        }

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

        public Response<TSource> TryAdd<TSource>(TSource entity, [CallerMemberName] string caller = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0) where TSource : EntityBase
        {
            var isValid = TryValidate(entity, out var errors);
            if (!isValid)
                return new Response<TSource>(Flags.ModelIsNotValid, entity, errors);

            var model = base.Add(entity);
            GenerateAudit(model, caller, callerPath, callerLine);

            var result = new Response<TSource>(Flags.Success, model.Entity);
            return result;
        }

        public Response<TEntity> TryUnHide<TEntity>(TEntity entity, [CallerMemberName] string caller = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0)
          where TEntity : EntityBase
        {
            if (!entity.IsDeleted)
                return new Response<TEntity>(Flags.NotDeleted, entity);

            entity.IsDeleted = false;
            var entry = base.Update(entity);
            GenerateAudit(entry, caller, callerPath, callerLine);

            var result = new Response<TEntity>(Flags.Success, entry.Entity);
            return result;
        }

        public bool TryValidate(IResponseBase response, out ValidatableResultCollection errors)
        {
            // TODO should be IResponseTrack
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

        public Response<TSource> TryUpdate<TSource>(TSource entity, [CallerMemberName] string caller = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0) where TSource : EntityBase
        {
            if (HttpContextAccessor == null)
                throw new NullReferenceException(nameof(HttpContextAccessor));

            var isValid = TryValidate(entity, out var errors);
            if (!isValid)
                return new Response<TSource>(Flags.ModelIsNotValid, entity, errors);

            var entry = base.Update(entity);
            GenerateAudit(entry, caller, callerPath, callerLine);

            var result = new Response<TSource>(Flags.Success, entry.Entity);
            return result;
        }

        public Response<TEntity> TryHideUnHide<TEntity>(TEntity entity, [CallerMemberName] string caller = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0) where TEntity : EntityBase

        {
            entity.IsDeleted = !entity.IsDeleted;
            var entry = base.Update(entity);
            GenerateAudit(entry, caller, callerPath, callerLine);

            return new Response<TEntity>(Flags.Success, entity);
        }

        public Response<TEntity> TryHide<TEntity>(TEntity entity, [CallerMemberName] string caller = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0) where TEntity : EntityBase
        {
            if (entity.IsDeleted)
                return new Response<TEntity>(Flags.AlreadyDeleted, entity);

            entity.IsDeleted = true;
            var entry = base.Update(entity);
            GenerateAudit(entry, caller, callerPath, callerLine);

            return new Response<TEntity>(Flags.Success, entry.Entity);
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

        public void DeletePermanently<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Deleted;
        }

        private void GenerateAudit(EntityEntry entry, string caller, string callerPath, int callerLine)
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

            if (HttpContextAccessor == null)
                throw new NullReferenceException($"'{nameof(HttpContextAccessor)}' should not be null in this level");

            var httpContext = HttpContextAccessor.HttpContext;
            var ipAddress = httpContext?.GetIpAddress() ?? IPAddress.None;
            var userId = httpContext.GetCurrentUser()?.GuidId;
            var userAgent = httpContext?.Request?.Headers["User-Agent"];
            var audit = new Audit(userId, ipAddress, userAgent, entry, flag, entityBase.Id, caller, $"{callerPath}::{callerLine}");
            entityBase.Audits ??= new AuditCollection();
            entityBase.Audits.Add(audit);
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