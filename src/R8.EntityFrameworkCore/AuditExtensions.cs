using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace R8.EntityFrameworkCore
{
    public static class AuditExtensions
    {
        /// <summary>
        /// Generates and adds a audit in type of <see cref="IAudit"/> to given entity entry according to request.
        /// </summary>
        /// <param name="entry">A <see cref="EntityEntry"/> object that containing an database entry.</param>
        /// <param name="flag">A <see cref="AuditFlags"/> enumerator constant that representing type of audit.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void GenerateAudit(this EntityEntry entry, AuditFlags flag)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            if (!(entry.Entity is EntityBase entityBase))
                return;

            var audit = new Audit(flag, entityBase.Id);
            entityBase.Audits ??= new AuditCollection();
            entityBase.Audits.Add(audit);
        }

        /// <summary>
        /// Generates and adds a audit in type of <see cref="IAudit"/> to given entity entry according to request.
        /// </summary>
        /// <param name="dbContext">A derived type of <see cref="DbContext"/>.</param>
        /// <param name="entry">A <see cref="EntityEntry"/> object that containing an database entry.</param>
        /// <param name="flag">A <see cref="AuditFlags"/> enumerator constant that representing type of audit.</param>
        /// <param name="userId">A <see cref="Guid"/> id that representing maintainer internal id.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void GenerateAudit<TDbContext>(this TDbContext dbContext, EntityEntry entry, AuditFlags flag, Guid? userId) where TDbContext : DbContext
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            var auditGenerated = false;

            if (dbContext is IAuditGenerator auditContext)
            {
                var auditOptions = auditContext.AuditOptions.Invoke(entry);
                if (userId != null)
                {
                    entry.GenerateAudit(flag, (Guid)userId, auditOptions);
                }
                else
                {
                    entry.GenerateAudit(flag, auditOptions);
                }

                auditGenerated = true;
            }

            if (!auditGenerated)
                entry.GenerateAudit(AuditFlags.Created);
        }

        /// <summary>
        /// Generates and adds a audit in type of <see cref="IAudit"/> to given entity entry according to request.
        /// </summary>
        /// <param name="entry">A <see cref="EntityEntry"/> object that containing an database entry.</param>
        /// <param name="flag">A <see cref="AuditFlags"/> enumerator constant that representing type of audit.</param>
        /// <param name="userId">A <see cref="Guid"/> object that representing which user did changes.</param>
        /// <param name="options">A <see cref="AuditOptions"/> object that should being copied into audits.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void GenerateAudit(this EntityEntry entry, AuditFlags flag, Guid userId, AuditOptions options)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (!(entry.Entity is EntityBase entityBase))
                return;

            entry.GenerateAudit(flag, userId, options.RemoteIpAddress, options.LocalIpAddress, options.UserAgent);
        }

        /// <summary>
        /// Generates and adds a audit in type of <see cref="IAudit"/> to given entity entry according to request.
        /// </summary>
        /// <param name="entry">A <see cref="EntityEntry"/> object that containing an database entry.</param>
        /// <param name="flag">A <see cref="AuditFlags"/> enumerator constant that representing type of audit.</param>
        /// <param name="options">A <see cref="AuditOptions"/> object that should being copied into audits.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void GenerateAudit(this EntityEntry entry, AuditFlags flag, AuditOptions options)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (!(entry.Entity is EntityBase entityBase))
                return;

            entry.GenerateAudit(flag, options.UserId, options.RemoteIpAddress, options.LocalIpAddress, options.UserAgent);
        }

        /// <summary>
        /// Generates and adds a audit in type of <see cref="IAudit"/> to given entity entry according to request.
        /// </summary>
        /// <param name="entry">A <see cref="EntityEntry"/> object that containing an database entry.</param>
        /// <param name="flag">A <see cref="AuditFlags"/> enumerator constant that representing type of audit.</param>
        /// <param name="userId">A <see cref="Guid"/> object that representing which user did changes.</param>
        /// <param name="remoteIpAddress">A <see cref="IPAddress"/> object that representing what is user's remote ip address.</param>
        /// <param name="localIpAddress">A <see cref="IPAddress"/> object that representing what is user's local ip address.</param>
        /// <param name="userAgent">A <see cref="string"/> value that representing user-agent according to request.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void GenerateAudit(this EntityEntry entry, AuditFlags flag, Guid? userId, IPAddress remoteIpAddress, IPAddress localIpAddress, string userAgent)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            if (!(entry.Entity is EntityBase entityBase))
                return;

            var oldValues = new Dictionary<string, object>();
            var newValues = new Dictionary<string, object>();
            if (entry.State != EntityState.Modified && entry.State != EntityState.Added)
                return;

            var propertyEntries = entry.Members
                .Where(x => x.Metadata.Name != nameof(EntityBase.Id)
                            && x.Metadata.Name != nameof(EntityBase.Audits)
                            && x.Metadata.Name != nameof(EntityBase.RowVersion)
                            && x is PropertyEntry)
                .Cast<PropertyEntry>()
                .ToList();
            if (propertyEntries.Count == 0)
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

            var audit = new Audit(userId, remoteIpAddress, localIpAddress, userAgent, flag, entityBase.Id, oldValues: oldValues, newValues: newValues);
            entityBase.Audits ??= new AuditCollection();
            entityBase.Audits.Add(audit);
        }
    }
}