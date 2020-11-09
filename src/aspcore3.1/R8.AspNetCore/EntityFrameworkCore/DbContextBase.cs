using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

using R8.EntityFrameworkCore;
using R8.Lib.MethodReturn;

using System;
using System.Net;
using System.Runtime.CompilerServices;

namespace R8.AspNetCore.EntityFrameworkCore
{
    /// <summary>
    /// An pre-filled derived class of <see cref="DbContext"/>
    /// </summary>
    public partial class DbContextBase : R8.EntityFrameworkCore.DbContextBase
    {
        public IHttpContextAccessor HttpContextAccessor { get; set; }

        protected DbContextBase() : this(new DbContextOptions<R8.EntityFrameworkCore.DbContextBase>())
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

        public Response<TSource> TryUpdate<TSource>(TSource entity, [CallerMemberName] string caller = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0) where TSource : EntityBase
        {
            if (HttpContextAccessor == null)
                throw new NullReferenceException(nameof(HttpContextAccessor));

            var httpContext = HttpContextAccessor.HttpContext;
            var userId = httpContext.GetCurrentUser()?.GuidId;
            return base.TryUpdate(entity, userId, caller, callerPath, callerLine);
        }

        public Response<TSource> TryAdd<TSource>(TSource entity, string caller = null, string callerPath = null,
            int callerLine = 0) where TSource : EntityBase
        {
            if (HttpContextAccessor == null)
                throw new NullReferenceException(nameof(HttpContextAccessor));

            var httpContext = HttpContextAccessor.HttpContext;
            var userId = httpContext.GetCurrentUser()?.GuidId;
            return base.TryAdd(entity, userId, caller, callerPath, callerLine);
        }

        public Response<TSource> TryUnHide<TSource>(TSource entity, string caller = null, string callerPath = null,
            int callerLine = 0) where TSource : EntityBase
        {
            if (HttpContextAccessor == null)
                throw new NullReferenceException(nameof(HttpContextAccessor));

            var httpContext = HttpContextAccessor.HttpContext;
            var userId = httpContext.GetCurrentUser()?.GuidId;
            return base.TryUnHide(entity, userId, caller, callerPath, callerLine);
        }

        public Response<TSource> TryHideUnHide<TSource>(TSource entity, string caller = null, string callerPath = null,
            int callerLine = 0) where TSource : EntityBase
        {
            if (HttpContextAccessor == null)
                throw new NullReferenceException(nameof(HttpContextAccessor));

            var httpContext = HttpContextAccessor.HttpContext;
            var userId = httpContext.GetCurrentUser()?.GuidId;
            return base.TryHideUnHide(entity, userId, caller, callerPath, callerLine);
        }

        public Response<TSource> TryHide<TSource>(TSource entity, string caller = null, string callerPath = null,
            int callerLine = 0) where TSource : EntityBase
        {
            if (HttpContextAccessor == null)
                throw new NullReferenceException(nameof(HttpContextAccessor));

            var httpContext = HttpContextAccessor.HttpContext;
            var userId = httpContext.GetCurrentUser()?.GuidId;
            return base.TryHide(entity, userId, caller, callerPath, callerLine);
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
            FindChanges(entry, out var oldValues, out var newValues);
            var audit = new Audit(userId, ipAddress, userAgent, flag, entityBase.Id, caller, $"{callerPath}::{callerLine}", oldValues, newValues);
            entityBase.Audits ??= new AuditCollection();
            entityBase.Audits.Add(audit);
        }
    }
}