using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using R8.EntityFrameworkCore;
using R8.Lib.Enums;

using System;
using System.Linq;
using System.Linq.Expressions;

namespace R8.AspNetCore.EntityFrameworkCore
{
    public static class QueryWebExtensions
    {
        // public static IQueryable<TSource> BelongToUser<TSource, TUser>(this IQueryable<TSource> query, TUser user, Expression<Func<TSource, bool>> predicate) where TSource : EntityBase where TUser : AuthenticatedUser
        // {
        //     if (user == null || (user.Role != Roles.Admin && user.Role != Roles.Operator))
        //         query = query.Where(predicate);
        //
        //     return query;
        // }
        //
        // public static IQueryable<TSource> InitializeQuery<TDbContext, TSource>(this TDbContext context, Expression<Func<TDbContext, DbSet<TSource>>> dbSet, AuthenticatedUser user, bool ignoreFilterForAdmins = true) where TDbContext : DbContextBase where TSource : EntityBase
        // {
        //     var _dbSet = dbSet.Compile().Invoke(context);
        //     var result = ignoreFilterForAdmins && (user.Role == Roles.Admin || user.Role == Roles.Operator)
        //         ? _dbSet.IgnoreQueryFilters()
        //         : _dbSet.AsQueryable();
        //     return result;
        // }
        //
        // public static IQueryable<TSource> InitializeQuery<TDbContext, TSource>(this TDbContext context, Expression<Func<TDbContext, DbSet<TSource>>> dbSet, bool ignoreFilterForAdmins = true) where TDbContext : DbContextBase where TSource : EntityBase
        // {
        //     var httpContextAccessor = context.GetService<IHttpContextAccessor>();
        //     if (httpContextAccessor == null)
        //         throw new NullReferenceException(nameof(HttpContextAccessor));
        //
        //     var currentUser = context.GetService<IHttpContextAccessor>().HttpContext.GetAuthenticatedUser();
        //     if (currentUser != null && (currentUser.Role == Roles.Admin || currentUser.Role == Roles.Operator))
        //     {
        //         return dbSet.Compile().Invoke(context)
        //             .IgnoreQueryFilters()
        //             .AsQueryable();
        //     }
        //
        //     return dbSet.Compile().Invoke(context).AsQueryable();
        // }
        //
        // public static IQueryable<TSource> InitializeQuery<TDbContext, TSource>(this TDbContext context, Expression<Func<TDbContext, DbSet<TSource>>> dbSet, out AuthenticatedUser user, bool ignoreFilterForAdmins = true) where TDbContext : DbContextBase where TSource : EntityBase
        // {
        //     user = context.GetService<IHttpContextAccessor>().HttpContext.GetAuthenticatedUser();
        //
        //     var result = context.InitializeQuery(dbSet, user);
        //     return result;
        // }
    }
}