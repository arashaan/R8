using System.Diagnostics;
using System.Net;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

using R8.EntityFrameworkCore;
using R8.Lib;

namespace R8.AspNetCore.EntityFrameworkCore
{
    public static class DbContextBaseExtensions
    {
        public static bool UnHide<TDbContext, TSource>(this TDbContext dbContext, TSource entity) where TDbContext : DbContextBase where TSource : IEntityBase
        {
            if (!entity.IsDeleted)
                return false;

            entity.IsDeleted = false;
            var entry = dbContext.Update(entity);
            var frame = new StackTrace().GetFrame(1);
            dbContext.GenerateAudit(entry, AuditFlags.UnDeleted);
            return true;
        }

        public static bool ToggleHiding<TDbContext, TSource>(this TDbContext dbContext, TSource entity) where TDbContext : DbContextBase where TSource : IEntityBase
        {
            var flag = entity.IsDeleted
                ? AuditFlags.UnDeleted
                : AuditFlags.Deleted;
            entity.IsDeleted = !entity.IsDeleted;
            var entry = dbContext.Update(entity);
            var frame = new StackTrace().GetFrame(1);
            dbContext.GenerateAudit(entry, flag);
            return true;
        }

        private static void GenerateAudit<TDbContext>(this TDbContext dbContext, EntityEntry entry, AuditFlags flag) where TDbContext : DbContextBase
        {
            var httpContextAccessor = dbContext.GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;
            var remoteIpAddress = httpContext?.Connection.RemoteIpAddress ?? IPAddress.None;
            var localIpAddress = httpContext?.Connection.LocalIpAddress ?? IPAddress.None;
            var userId = httpContext.GetAuthenticatedUser()?.Id;
            var userAgent = httpContext?.Request?.Headers["User-Agent"];

            entry.GenerateAudit(flag, userId, remoteIpAddress, localIpAddress, userAgent);
        }

        public static bool Update<TDbContext, TSource>(this TDbContext dbContext, TSource entity, out ValidatableResultCollection errors) where TDbContext : DbContextBase where TSource : IEntityBase
        {
            var isValid = DbContextBase.TryValidate(entity, out errors);
            if (!isValid)
                return false;

            var entry = dbContext.Update(entity);
            var frame = new StackTrace().GetFrame(1);
            dbContext.GenerateAudit(entry, AuditFlags.Changed);

            return true;
        }

        public static bool Hide<TDbContext, TSource>(this TDbContext dbContext, TSource entity) where TDbContext : DbContextBase where TSource : IEntityBase
        {
            if (entity.IsDeleted)
                return false;

            entity.IsDeleted = true;
            var entry = dbContext.Update(entity);
            var frame = new StackTrace().GetFrame(1);
            dbContext.GenerateAudit(entry, AuditFlags.Deleted);
            return true;
        }

        public static bool Add<TDbContext, TSource>(this TDbContext dbContext, TSource entity, out ValidatableResultCollection errors) where TDbContext : DbContextBase where TSource : IEntityBase
        {
            var isValid = DbContextBase.TryValidate(entity, out errors);
            if (!isValid)
                return false;

            var entry = dbContext.Add(entity);
            var frame = new StackTrace().GetFrame(1);
            dbContext.GenerateAudit(entry, AuditFlags.Created);
            return true;
        }
    }
}