using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace R8.EntityFrameworkCore.Audits
{
    /// <summary>
    /// An Audit interceptor to store changed information of entities.
    /// </summary>
    public class AuditProviderInterceptor : SaveChangesInterceptor
    {
        private readonly IAuditDataProvider _provider;
        private readonly AuditProviderConfiguration _config;

        /// <summary>
        /// An Audit interceptor to store changed information of entities.
        /// </summary>
        public AuditProviderInterceptor(IAuditDataProvider provider, AuditProviderConfiguration config)
        {
            _provider = provider;
            _config = config;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var entries = eventData.Context.ChangeTracker.Entries();
            if (entries == null || !entries.Any())
                return await base.SavingChangesAsync(eventData, result, cancellationToken);

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Deleted && _config.PermanentDelete)
                    continue;

                var propertyEntries = GetPropertyEntries(entityEntry);
                if (propertyEntries.Count == 0)
                    continue;

                var entity = entityEntry.Entity;

                if (entity is Audit && entityEntry.State == EntityState.Deleted && !_config.PermanentDelete)
                    entityEntry.State = EntityState.Unchanged;

                if (!(entity is IEntityBaseAudit entityAudit))
                    continue;

                var entityId = ResolveID(entity, out var idProp);
                var entityDelete = ResolveDELETE(entity, out var deleteProp);

                var audit = new Audit
                {
                    RowId = entityId,
                    DateTime = DateTime.UtcNow
                };

                switch (entityEntry.State)
                {
                    case EntityState.Deleted:
                    case EntityState.Modified:
                        {
                            audit.Changes = new AuditChangesCollection(GetChanges(GetPropertyEntries(entityEntry)));
                            if (entityEntry.State == EntityState.Deleted)
                            {
                                await entityEntry.ReloadAsync(cancellationToken);
                                entityEntry.DetectChanges();
                                audit.Flag = AuditFlags.Deleted;
                                entityEntry.State = EntityState.Modified;
                                if (deleteProp != null)
                                {
                                    if (entityDelete != null && entityDelete.Value)
                                        continue;

                                    deleteProp.SetValue(entity, true);
                                    audit.Flag = AuditFlags.Deleted;
                                    audit.Additional = await _provider.OnRemoveAsync(entityEntry);
                                }
                            }
                            else
                            {
                                var deleteInChanges = audit.Changes.FirstOrDefault(x => x.Key.Equals(_config.DeleteColumn));
                                if (deleteInChanges != null)
                                {
                                    if (deleteInChanges.OldValue?.ToString()?.Equals(false.ToString(),
                                            StringComparison.InvariantCultureIgnoreCase) == true &&
                                        deleteInChanges.NewValue?.ToString()?.Equals(true.ToString(),
                                            StringComparison.InvariantCultureIgnoreCase) == true)
                                    {
                                        audit.Flag = AuditFlags.Deleted;
                                        audit.Additional = await _provider.OnRemoveAsync(entityEntry);
                                    }

                                    if (deleteInChanges.OldValue?.ToString()?.Equals(true.ToString(),
                                            StringComparison.InvariantCultureIgnoreCase) == true &&
                                        deleteInChanges.NewValue?.ToString()?.Equals(false.ToString(),
                                            StringComparison.InvariantCultureIgnoreCase) == true)
                                    {
                                        audit.Flag = AuditFlags.UnDeleted;
                                        audit.Additional = await _provider.OnUnRemoveAsync(entityEntry);
                                    }
                                }
                                else
                                {
                                    audit.Flag = AuditFlags.Changed;
                                    audit.Additional = await _provider.OnUpdateAsync(entityEntry);
                                }
                            }

                            audit.Changes = new AuditChangesCollection(GetChanges(GetPropertyEntries(entityEntry)));

                            if (audit.Changes?.Any() == true)
                            {
                                foreach (var auditChange in audit.Changes)
                                {
                                    var memberEntry =
                                        entityEntry.Members.FirstOrDefault(x => x.Metadata.Name == auditChange.Key);
                                    if (!(memberEntry?.Metadata is Property entry))
                                        continue;

                                    var foreignKey = entry.ForeignKeys?.FirstOrDefault();
                                    if (foreignKey == null)
                                        continue;

                                    var navigationEntityName = foreignKey.DependentToPrincipal.ClrType;
                                    auditChange.NavigationalEntity = $"{navigationEntityName.Namespace}.{navigationEntityName.Name}";
                                }
                            }
                            if (deleteProp != null)
                            {
                                if (audit.Flag == AuditFlags.Deleted || audit.Flag == AuditFlags.UnDeleted)
                                {
                                    var delete = audit.Changes[_config.DeleteColumn];
                                    if (delete != null) audit.Changes.Remove(delete);
                                }
                            }
                            break;
                        }

                    case EntityState.Added:
                        {
                            audit.Flag = AuditFlags.Created;
                            audit.Additional = await _provider.OnAddAsync(entityEntry);
                            break;
                        }

                    case EntityState.Detached:
                    case EntityState.Unchanged:
                    default:
                        continue;
                }

                entityAudit.Audits = new AuditCollection(entityAudit.Audits.Prepend(audit));
                //if (entityAudit.Audits.Created == null)
                //    throw new DBConcurrencyException($"Unable to store audits for entity that not being added yet.");
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private bool? ResolveDELETE(object entity, out PropertyInfo property)
        {
            var entityType = entity.GetType();
            property = null;

            if (string.IsNullOrEmpty(_config.DeleteColumn))
                throw new NullReferenceException($"{nameof(_config.DeleteColumn)} must be defined, when {nameof(_config.PermanentDelete)} equals to false.");

            property = entityType.GetProperty(_config.DeleteColumn);
            if (property == null)
                throw new NullReferenceException($"Unable to find {nameof(_config.DeleteColumn)} property.");

            if (property.GetValue(entity) is bool delete)
                return delete;

            property = null;
            throw new TypeAccessException($"{nameof(_config.DeleteColumn)} property type must be boolean.");
        }

        private Guid ResolveID(object entity, out PropertyInfo property)
        {
            if (string.IsNullOrEmpty(_config.IdColumn))
                throw new NullReferenceException($"{nameof(_config.IdColumn)} must be defined.");

            var entityType = entity.GetType();
            property = entityType.GetProperty(_config.IdColumn);
            if (property == null)
                throw new NullReferenceException("Unable to find Id column in entity.");

            if (!(property.GetValue(entity) is Guid entityId))
            {
                property = null;
                throw new Exception($"Based on current strategy, Entity's ID must be a type of {typeof(Guid)}.");
            }

            if (entityId != Guid.Empty)
                return entityId;

            property = null;
            throw new NullReferenceException("Unable to find Id value in entity.");
        }

        private List<PropertyEntry> GetPropertyEntries(EntityEntry entityEntry)
        {
            var restrictedColumns = new List<string>();
            if (_config.UntrackableColumns?.Any() == true)
                restrictedColumns.AddRange(_config.UntrackableColumns);

            if (!restrictedColumns.Contains(_config.IdColumn))
                restrictedColumns.Add(_config.IdColumn);

            if (!restrictedColumns.Contains(nameof(IEntityBaseAudit.Audits)))
                restrictedColumns.Add(nameof(IEntityBaseAudit.Audits));

            var propertyEntries = entityEntry.Members
                .Where(x => !restrictedColumns.Contains(x.Metadata.Name) && x is PropertyEntry)
                .Cast<PropertyEntry>()
                .ToList();
            return propertyEntries;
        }

        private static IEnumerable<AuditChange> GetChanges(IEnumerable<PropertyEntry> propertyEntries)
        {
            var changes = (from propertyEntry in propertyEntries
                           let propertyName = propertyEntry.Metadata.Name
                           let newValue = propertyEntry.CurrentValue
                           let oldValue = propertyEntry.OriginalValue
                           where newValue != null || oldValue != null
                           where newValue?.Equals(oldValue) != true
                           select new AuditChange(propertyName, oldValue, newValue)).ToList();
            return changes;
        }
    }
}