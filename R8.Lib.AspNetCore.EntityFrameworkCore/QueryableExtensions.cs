using EFCoreSecondLevelCacheInterceptor;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

using R8.Lib.AspNetCore.Base;
using R8.Lib.Enums;
using R8.Lib.Paginator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace R8.Lib.AspNetCore.EntityFrameworkCore
{
    public static class QueryableExtensions
    {
        public static IQueryable<TSource> BelongToUser<TSource, TUser>(this IQueryable<TSource> query, TUser user, Expression<Func<TSource, bool>> predicate) where TSource : EntityBase where TUser : CurrentUser
        {
            if (user == null || (user.Role != Roles.Admin && user.Role != Roles.Operator))
                query = query.Where(predicate);

            return query;
        }

        public static TDbContext GetDbContext<TDbContext>(this IQueryable query) where TDbContext : DbContextBase
        {
            const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            var queryCompiler = typeof(EntityQueryProvider).GetField("_queryCompiler", bindingFlags).GetValue(query.Provider);
            var queryContextFactory = queryCompiler.GetType().GetField("_queryContextFactory", bindingFlags).GetValue(queryCompiler);

            var dependencies = typeof(RelationalQueryContextFactory).GetProperty("Dependencies", bindingFlags).GetValue(queryContextFactory);
            var queryContextDependencies = typeof(TDbContext).Assembly.GetType(typeof(QueryContextDependencies).FullName);
            var stateManagerProperty = queryContextDependencies.GetProperty("StateManager", bindingFlags | BindingFlags.Public).GetValue(dependencies);
            var stateManager = (IStateManager)stateManagerProperty;

            if (stateManager != null)
                return (TDbContext)stateManager.Context;

            throw new NullReferenceException($"Unable to get '{typeof(TDbContext).Name}' from query");
        }

        //private static IQueryable<TSource> ApplyAdminSearchConditions<TSource, TSearch>(this IQueryable<TSource> query, HttpContext httpContext, TSearch searchModel, CurrentUser currentUser = null) where TSource : EntityBase where TSearch : BaseSearchModel
        //{
        //    currentUser ??= httpContext.GetCurrentUser();
        //    if (searchModel == null)
        //        return query;

        //    Audit baser;
        //    var dateTimeProperty = new Audit().GetPublicProperties().FirstOrDefault(x => x.Name == nameof(baser.DateTime)).GetJsonProperty();
        //    var userIdProperty = new Audit().GetPublicProperties().FirstOrDefault(x => x.Name == nameof(baser.UserId)).GetJsonProperty();

        //    if (!string.IsNullOrEmpty(searchModel.CreationDateFrom))
        //    {
        //        // var dateTime = ((DateTime)searchModel.CreationDateFrom).ToString("yyyy/MM/dd", new CultureInfo("en-US"));
        //        query = query.Where(x => EF.Functions.DateDiff("DAY", searchModel.CreationDateFrom,
        //          EF.Functions.JsonValue(x.AuditsJson, "$[0]." + dateTimeProperty)) <= 0);
        //    }

        //    if (!string.IsNullOrEmpty(searchModel.CreationDateTo))
        //    {
        //        // var dateTime = ((DateTime)searchModel.CreationDateTo).ToString("yyyy/MM/dd", new CultureInfo("en-US"));
        //        query = query.Where(x => EF.Functions.DateDiff("DAY", searchModel.CreationDateTo,
        //          EF.Functions.JsonValue(x.AuditsJson, "$[0]." + dateTimeProperty)) >= 0);
        //    }

        //    if (currentUser.Role != Roles.Admin)
        //        return query;

        //    if (string.IsNullOrEmpty(searchModel.CreatorId))
        //        return query;

        //    query = query.Where(entity => EF.Functions.JsonValue(entity.AuditsJson, "$[0]." + userIdProperty) == searchModel.CreatorId);

        //    return query;
        //}

        public static IQueryable<TSource> InitializeQuery<TDbContext, TSource>(this TDbContext context, Expression<Func<TDbContext, DbSet<TSource>>> dbSet, CurrentUser user, bool ignoreFilterForAdmins = true) where TDbContext : DbContextBase where TSource : EntityBase
        {
            var _dbSet = dbSet.Compile().Invoke(context);
            var result = ignoreFilterForAdmins && (user.Role == Roles.Admin || user.Role == Roles.Operator)
                ? _dbSet.IgnoreQueryFilters()
                : _dbSet.AsQueryable();
            return result;
        }

        //public static string DateTimeId => new Audit()
        //        .GetPublicProperties()
        //        .Find(x => x.Name == nameof(Audit.DateTime))
        //        .GetJsonProperty();

        public static IQueryable<TSource> OrderByCreationDateTime<TSource>(this IQueryable<TSource> query) where TSource : EntityBase
        {
            return query.OrderBy(x => x.Audits.FirstOrDefault().DateTime);
        }

        //public static IOrderedEnumerable<TSource> OrderByCreationDateTime<TSource>(this ICollection<TSource> sources) where TSource : EntityBase
        //{
        //    var source = sources
        //        .OrderBy(x => x.Audits.FirstOrDefault(v => v.Flag == AuditFlags.Created).DateTime);

        //    return source;
        //}

        public static IOrderedEnumerable<TSource> OrderByCreationDateTime<TSource>(this IEnumerable<TSource> sources) where TSource : EntityBase
        {
            var source = sources
                .OrderBy(x => x.Audits.FirstOrDefault().DateTime);

            return source;
        }

        public static string DapperGetColumns(string tableName, string idColumn, params string[] columns)
        {
            if (tableName == null) throw new ArgumentNullException(nameof(tableName));
            if (idColumn == null) throw new ArgumentNullException(nameof(idColumn));

            // AS {nameof(EntityBase.IdString)}
            var text = $"[{tableName}].[{idColumn}] ";
            if (columns?.Any() != true)
                return text;

            text += ", ";
            text += string.Join(", ", columns.Select(column => $"[{tableName}].[{column}]"));

            return text;
        }

        public static string Like(this string searchString)
        {
            return $"%{string.Join("%", searchString.Split(' ').ToArray())}%";
        }

        //public static IOrderedEnumerable<TSource> OrderDescendingByCreationDateTime<TSource>(this ICollection<TSource> sources) where TSource : EntityBase
        //{
        //    var source = sources
        //        .OrderByDescending(x => x.Audits.FirstOrDefault(v => v.Flag == AuditFlags.Created).DateTime);

        //    return source;
        //}
        public static IEnumerable<TSource> OrderDescendingByCreationDateTimeEF<TSource>(this IEnumerable<TSource> entities) where TSource : EntityBase
        {
            var source = from que in entities
                         orderby que.Audits.First().DateTime descending
                         //orderby EF.Functions.JsonValue(que.AuditsJson, "$[0]." + DateTimeId) descending
                         select que;
            return source;
        }

        public static IQueryable<TSource> OrderDescendingByCreationDateTime<TSource>(this IQueryable<TSource> query) where TSource : EntityBase
        {
            return query.OrderByDescending(x => x.Audits.FirstOrDefault().DateTime);
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to <see cref="Audit"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="sources"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted in descending order according to <see cref="Audit"/></returns>
        public static IOrderedEnumerable<TSource> OrderDescendingByCreationDateTime<TSource>(this IEnumerable<TSource> sources) where TSource : EntityBase
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));

            var source = sources
                .OrderByDescending(x => x.Audits.FirstOrDefault(v => v.Flag == AuditFlags.Created)?.DateTime);
            return source;
        }

        public static IQueryable<TSource> InitializeQuery<TDbContext, TSource>(this TDbContext context, Expression<Func<TDbContext, DbSet<TSource>>> dbSet, bool ignoreFilterForAdmins = true) where TDbContext : DbContextBase where TSource : EntityBase
        {
            var currentUser = context.HttpContextAccessor.HttpContext.GetCurrentUser();
            if (currentUser != null && (currentUser.Role == Roles.Admin || currentUser.Role == Roles.Operator))
            {
                return dbSet.Compile().Invoke(context)
                    .IgnoreQueryFilters()
                    .AsQueryable();
            }

            return dbSet.Compile().Invoke(context).AsQueryable();
        }

        public static IQueryable<TSource> InitializeQuery<TDbContext, TSource>(this TDbContext context, Expression<Func<TDbContext, DbSet<TSource>>> dbSet, out CurrentUser user, bool ignoreFilterForAdmins = true) where TDbContext : DbContextBase where TSource : EntityBase
        {
            user = context.HttpContextAccessor.HttpContext.GetCurrentUser();

            var result = context.InitializeQuery(dbSet, user);
            return result;
        }

        public static Task<Pagination<TSource>> PaginateAsync<TSource, TSearch>(this IQueryable<TSource> query, TSearch searchModel, HttpContext httpContext, bool paginate, bool loadData, bool cacheData, CancellationToken cancellationToken = default)
            where TSource : EntityBase where TSearch : IBaseSearch
        {
            //query = query.ApplyAdminSearchConditions(httpContext, searchModel);

            var output = query.PaginateAsync(searchModel, paginate, loadData, cacheData, cancellationToken);
            return output;
        }

        private static async Task<Pagination<TSource>> PaginateAsync<TSource>(this IQueryable<TSource> query, int page, bool paginate, bool loadData, bool cacheData, int pageSize = 10, CancellationToken cancellationToken = default)
          where TSource : EntityBase
        {
            page = page <= 0 ? 1 : page;

            var output = new Pagination<TSource>();
            page = page <= 1 ? 1 : page;

            query = query.OrderDescendingByCreationDateTime();
            var rowQue = query;
            if (cacheData)
                rowQue = query.Cacheable();

            var rowCount = await rowQue
                .CountAsync(cancellationToken);
            if (rowCount <= 0)
                return output;

            var pageCount = Numbers.RoundToUp((double)rowCount / pageSize);
            if (page > pageCount)
                page = 1;

            var models = new List<TSource>();
            if (loadData)
            {
                var pagingQuery = query;
                if (paginate)
                {
                    pagingQuery = page > 1
                        ? query.Skip(pageSize * (page - 1))
                        : query;
                    pagingQuery = pagingQuery.Take(pageSize);
                }

                var dataQue = pagingQuery;
                if (cacheData)
                    dataQue = dataQue.Cacheable();

                models = await dataQue
                    .ToListAsync(cancellationToken);
            }

            var pages = Numbers.RoundToUp((double)rowCount / pageSize);
            output = new Pagination<TSource>(models, page, pages, rowCount);
            return output;
        }

        public static async Task<Pagination<TResult>> PaginateAsync<TSource, TSearch, TResult>(this IQueryable<TSource> query, Expression<Func<TSource, TResult>> selector, TSearch searchModel, bool sortByCreation = true, bool cacheData = true)
            where TSource : EntityBase where TSearch : IBaseSearch where TResult : class
        {
            var output = new Pagination<TResult>();

            var page = searchModel?.PageNo ?? 1;
            var pageSize = searchModel?.PageSize ?? 10;

            if (sortByCreation)
                query = query.OrderDescendingByCreationDateTime();

            var rowQue = query;
            if (cacheData)
                rowQue = query.Cacheable();

            var rowCount = await rowQue
                .CountAsync();
            if (rowCount <= 0)
                return output;

            var pageCount = Numbers.RoundToUp((double)rowCount / pageSize);
            if (page > pageCount)
                page = 1;

            var pagingQuery = page > 1
                    ? query.Skip(pageSize * (page - 1))
                    : query;
            pagingQuery = pagingQuery.Take(pageSize);

            var dataQue = pagingQuery;
            if (cacheData)
                dataQue = dataQue.Cacheable();

            var models = await dataQue
                .Select(selector)
                .ToListAsync();

            var pages = Numbers.RoundToUp((double)rowCount / pageSize);
            output = new Pagination<TResult>(models, page, pages, rowCount);
            return output;
        }

        public static Task<Pagination<TSource>> PaginateAsync<TSource, TSearch>(this IQueryable<TSource> query, TSearch searchModel)
            where TSource : EntityBase where TSearch : IBaseSearch
        {
            var page = searchModel?.PageNo ?? 1;
            var pageSize = searchModel?.PageSize ?? 10;

            var output = query.PaginateAsync(page, true, true, false, pageSize);
            return output;
        }

        public static Task<Pagination<TSource>> PaginateAsync<TSource, TSearch>(this IQueryable<TSource> query, TSearch searchModel, bool paginate, bool loadData, bool cacheData, CancellationToken cancellationToken = default)
          where TSource : EntityBase where TSearch : IBaseSearch
        {
            var page = searchModel?.PageNo ?? 1;
            var pageSize = searchModel?.PageSize ?? 10;

            var output = query.PaginateAsync(page, paginate, loadData, cacheData, pageSize, cancellationToken);
            return output;
        }
    }
}