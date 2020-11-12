using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Infrastructure;

using R8.EntityFrameworkCore;
using R8.Lib;

using System.Diagnostics;
using System.Net;

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

            var httpContextAccessor = dbContext.GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;
            var ipAddress = httpContext?.GetIPAddress() ?? IPAddress.None;
            var userId = httpContext.GetAuthenticatedUser()?.Id;
            var userAgent = httpContext?.Request?.Headers["User-Agent"];
            var frame = new StackTrace().GetFrame(1);

            entry.GenerateAudit(AuditFlags.UnDeleted, userId, ipAddress, userAgent, frame);
            return true;
        }

        public static bool ToggleHiding<TDbContext, TSource>(this TDbContext dbContext, TSource entity) where TDbContext : DbContextBase where TSource : IEntityBase
        {
            var flag = entity.IsDeleted
                ? AuditFlags.UnDeleted
                : AuditFlags.Deleted;
            entity.IsDeleted = !entity.IsDeleted;
            var entry = dbContext.Update(entity);

            var httpContextAccessor = dbContext.GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;
            var ipAddress = httpContext?.GetIPAddress() ?? IPAddress.None;
            var userId = httpContext.GetAuthenticatedUser()?.Id;
            var userAgent = httpContext?.Request?.Headers["User-Agent"];
            var frame = new StackTrace().GetFrame(1);

            entry.GenerateAudit(flag, userId, ipAddress, userAgent, frame);

            return true;
        }

        public static bool Update<TDbContext, TSource>(this TDbContext dbContext, TSource entity, out ValidatableResultCollection errors) where TDbContext : DbContextBase where TSource : IEntityBase
        {
            var isValid = DbContextBase.TryValidate(entity, out errors);
            if (!isValid)
                return false;

            var entry = dbContext.Update(entity);

            var httpContextAccessor = dbContext.GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;
            var ipAddress = httpContext?.GetIPAddress() ?? IPAddress.None;
            var userId = httpContext.GetAuthenticatedUser()?.Id;
            var userAgent = httpContext?.Request?.Headers["User-Agent"];
            var frame = new StackTrace().GetFrame(1);

            entry.GenerateAudit(AuditFlags.Changed, userId, ipAddress, userAgent, frame);

            return true;
        }

        public static bool Hide<TDbContext, TSource>(this TDbContext dbContext, TSource entity) where TDbContext : DbContextBase where TSource : IEntityBase
        {
            if (entity.IsDeleted)
                return false;

            entity.IsDeleted = true;
            var entry = dbContext.Update(entity);

            var httpContextAccessor = dbContext.GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;
            var ipAddress = httpContext?.GetIPAddress() ?? IPAddress.None;
            var userId = httpContext.GetAuthenticatedUser()?.Id;
            var userAgent = httpContext?.Request?.Headers["User-Agent"];
            var frame = new StackTrace().GetFrame(1);

            entry.GenerateAudit(AuditFlags.Deleted, userId, ipAddress, userAgent, frame);
            return true;
        }

        public static bool Add<TDbContext, TSource>(this TDbContext dbContext, TSource entity, out ValidatableResultCollection errors) where TDbContext : DbContextBase where TSource : IEntityBase
        {
            var isValid = DbContextBase.TryValidate(entity, out errors);
            if (!isValid)
                return false;

            var entry = dbContext.Add(entity);

            var httpContextAccessor = dbContext.GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;
            var ipAddress = httpContext?.GetIPAddress() ?? IPAddress.None;
            var userId = httpContext.GetAuthenticatedUser()?.Id;
            var userAgent = httpContext?.Request?.Headers["User-Agent"];
            var frame = new StackTrace().GetFrame(1);

            entry.GenerateAudit(AuditFlags.Created, userId, ipAddress, userAgent, frame);
            return true;
        }
    }
}