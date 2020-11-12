using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using R8.Lib;
using R8.Lib.Paginator;

namespace R8.EntityFrameworkCore
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Returns DbContext from given <see cref="IQueryable"/>.
        /// </summary>
        /// <typeparam name="TDbContext">the type of DbContext.</typeparam>
        /// <param name="query">A <see cref="IQueryable{T}"/> source.</param>
        /// <returns>A <see cref="DbContextBase"/> object for given type.</returns>
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
        //    currentUser ??= httpContext.GetAuthenticatedUser();
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

        //public static string DateTimeId => new Audit()
        //        .GetPublicProperties()
        //        .Find(x => x.Name == nameof(Audit.DateTime))
        //        .GetJsonProperty();

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a <see cref="IAudit"/> creation <see cref="DateTime"/>.
        /// </summary>
        /// <typeparam name="TSource">A generic type of <see cref="EntityBase"/>.</typeparam>
        /// <param name="query">A <see cref="IQueryable{T}"/> that representing source query.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="IOrderedQueryable{TElement}"/> whose elements are sorted in descending order according to a <see cref="IAudit"/> creation <see cref="DateTime"/>.</returns>
        public static IQueryable<TSource> OrderByCreationDateTime<TSource>(this IQueryable<TSource> query) where TSource : IEntityBase
        {
            if (query == null) 
                throw new ArgumentNullException(nameof(query));

            return query.OrderBy(x => x.Audits.First().DateTime);
        }

        //public static IOrderedEnumerable<TSource> OrderByCreationDateTime<TSource>(this ICollection<TSource> sources) where TSource : EntityBase
        //{
        //    var source = sources
        //        .OrderBy(x => x.Audits.FirstOrDefault(v => v.Flag == AuditFlags.Created).DateTime);

        //    return source;
        //}

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a <see cref="IAudit"/> creation <see cref="DateTime"/>.
        /// </summary>
        /// <typeparam name="TSource">A generic type of <see cref="EntityBase"/>.</typeparam>
        /// <param name="sources">A collection of <see cref="TSource"/>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted in ascending order according to a <see cref="IAudit"/> creation <see cref="DateTime"/>.</returns>
        public static IOrderedEnumerable<TSource> OrderByCreationDateTime<TSource>(this IEnumerable<TSource> sources) where TSource : IEntityBase
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));

            return sources.OrderBy(x => x.Audits.First().DateTime);
        }

        //
        // public static string DapperGetColumns(string tableName, string idColumn, params string[] columns)
        // {
        //     if (tableName == null) throw new ArgumentNullException(nameof(tableName));
        //     if (idColumn == null) throw new ArgumentNullException(nameof(idColumn));
        //
        //     // AS {nameof(EntityBase.IdString)}
        //     var text = $"[{tableName}].[{idColumn}] ";
        //     if (columns?.Any() != true)
        //         return text;
        //
        //     text += ", ";
        //     text += string.Join(", ", columns.Select(column => $"[{tableName}].[{column}]"));
        //
        //     return text;
        // }

        /// <summary>
        /// Returns a simplified SQL like query.
        /// </summary>
        /// <param name="searchString">A <see cref="string"/> value.</param>
        /// <returns>A SQL Raw query as <see cref="string"/>.</returns>
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

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a <see cref="IAudit"/> creation <see cref="DateTime"/>.
        /// </summary>
        /// <typeparam name="TSource">A generic type of <see cref="EntityBase"/>.</typeparam>
        /// <param name="sources">A collection of <see cref="TSource"/>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted in descending order according to a <see cref="IAudit"/> creation <see cref="DateTime"/>.</returns>
        public static IOrderedEnumerable<TSource> OrderDescendingByCreationDateTime<TSource>(this IEnumerable<TSource> sources) where TSource : IEntityBase
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));

            return sources.OrderByDescending(x => x.Audits.First().DateTime);
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a <see cref="IAudit"/> creation <see cref="DateTime"/>.
        /// </summary>
        /// <typeparam name="TSource">A generic type of <see cref="EntityBase"/>.</typeparam>
        /// <param name="query">A <see cref="IQueryable{T}"/> that representing source query.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="IOrderedQueryable{TElement}"/> whose elements are sorted in descending order according to a <see cref="IAudit"/> creation <see cref="DateTime"/>.</returns>
        public static IOrderedQueryable<TSource> OrderDescendingByCreationDateTime<TSource>(this IQueryable<TSource> query) where TSource : IEntityBase
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return query.OrderByDescending(x => x.Audits.First().DateTime);
        }

        ///// <summary>
        ///// Sorts the elements of a sequence in descending order according to a <see cref="IAudit"/> creation <see cref="DateTime"/>.
        ///// </summary>
        ///// <typeparam name="TSource">A type of <see cref="EntityBase"/>.</typeparam>
        ///// <param name="sources">A collection of <see cref="TSource"/>.</param>
        ///// <exception cref="ArgumentNullException"></exception>
        ///// <returns>An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted in descending order according to <see cref="Audit"/></returns>
        //public static IOrderedEnumerable<TSource> OrderDescendingByCreationDateTime<TSource>(this IEnumerable<TSource> sources) where TSource : EntityBase
        //{
        //    if (sources == null)
        //        throw new ArgumentNullException(nameof(sources));

        //    var source = sources
        //        .OrderByDescending(x => x.Audits.FirstOrDefault(v => v.Flag == AuditFlags.Created)?.DateTime);
        //    return source;
        //}

        /// <summary>
        /// Paginates results according to given page number and page size.
        /// </summary>
        /// <typeparam name="TSource">A generic type of <see cref="EntityBase"/>.</typeparam>
        /// <param name="query">A <see cref="IQueryable{T}"/> that representing source query.</param>
        /// <param name="page">A <see cref="int"/> value that representing specific page number.</param>
        /// <param name="paginate">A <see cref="bool"/> value that asking for paginating data.</param>
        /// <param name="loadData">A <see cref="bool"/> value that asking for loading of related data.</param>
        /// <param name="cacheData">A <see cref="bool"/> value that asking for caching of data.</param>
        /// <param name="pageSize">A <see cref="int"/> value that representing number of items in each page.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="Pagination{TModel}"/> instance that representing query results divided in each pages.</returns>
        private static async Task<Pagination<TSource>> PaginateAsync<TSource>(this IQueryable<TSource> query, int page, bool paginate, bool loadData, bool cacheData, int pageSize = 10)
          where TSource : class, IEntityBase
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            page = page <= 0 ? 1 : page;

            var output = new Pagination<TSource>();
            page = page <= 1 ? 1 : page;

            query = query.OrderDescendingByCreationDateTime();
            var rowQue = query;
            if (cacheData)
                rowQue = query.Cacheable();

            var rowCount = await rowQue
                .CountAsync()
                .ConfigureAwait(false);
            if (rowCount <= 0)
                return output;

            var pageCount = ((double)rowCount / pageSize).RoundToUp();
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
                    .ToListAsync()
                    .ConfigureAwait(false);
            }

            var pages = ((double)rowCount / pageSize).RoundToUp();
            return new Pagination<TSource>(models, page, pages, rowCount);
        }

        /// <summary>
        /// Paginates results according to given page number and page size.
        /// </summary>
        /// <typeparam name="TSource">A generic type of <see cref="EntityBase"/>.</typeparam>
        /// <typeparam name="TSearch">A generic type of <see cref="IBaseSearch"/></typeparam>
        /// <typeparam name="TResult">A generic type for result.</typeparam>
        /// <param name="query">A <see cref="IQueryable{T}"/> that representing source query.</param>
        /// <param name="selector">An <see cref="Expression{TResult}"/> that projecting results.</param>
        /// <param name="searchModel">An object of type <see cref="IBaseSearch"/> that representing page number and page size.</param>
        /// <param name="sortByCreation">A <see cref="bool"/> value that asking for sorting data by creation date time.</param>
        /// <param name="cacheData">A <see cref="bool"/> value that asking for caching of data.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="Pagination{TModel}"/> instance that representing query results divided in each pages.</returns>
        public static async Task<Pagination<TResult>> PaginateAsync<TSource, TSearch, TResult>(this IQueryable<TSource> query, Expression<Func<TSource, TResult>> selector, TSearch searchModel, bool sortByCreation = true, bool cacheData = true)
            where TSource : IEntityBase where TSearch : IBaseSearch where TResult : class
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var output = new Pagination<TResult>();

            var page = searchModel?.PageNo ?? 1;
            var pageSize = searchModel?.PageSize ?? 10;

            if (sortByCreation)
                query = query.OrderDescendingByCreationDateTime();

            var rowQue = query;
            if (cacheData)
                rowQue = query.Cacheable();

            var rowCount = await rowQue
                .CountAsync()
                .ConfigureAwait(false);
            if (rowCount <= 0)
                return output;

            var pageCount = ((double)rowCount / pageSize).RoundToUp();
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
                .ToListAsync()
                .ConfigureAwait(false);

            var pages = ((double)rowCount / pageSize).RoundToUp();
            output = new Pagination<TResult>(models, page, pages, rowCount);
            return output;
        }

        /// <summary>
        /// Paginates results according to given page number and page size.
        /// </summary>
        /// <typeparam name="TSource">A generic type of <see cref="EntityBase"/>.</typeparam>
        /// <typeparam name="TSearch">A generic type of <see cref="IBaseSearch"/></typeparam>
        /// <param name="query">A <see cref="IQueryable{T}"/> that representing source query.</param>
        /// <param name="searchModel">An object of type <see cref="IBaseSearch"/> that representing page number and page size.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="Pagination{TModel}"/> instance that representing query results divided in each pages.</returns>
        public static Task<Pagination<TSource>> PaginateAsync<TSource, TSearch>(this IQueryable<TSource> query, TSearch searchModel)
            where TSource : class, IEntityBase where TSearch : IBaseSearch
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var page = searchModel?.PageNo ?? 1;
            var pageSize = searchModel?.PageSize ?? 10;

            return query.PaginateAsync(page, true, true, false, pageSize);
        }

        /// <summary>
        /// Paginates results according to given page number and page size.
        /// </summary>
        /// <typeparam name="TSource">A generic type of <see cref="EntityBase"/>.</typeparam>
        /// <typeparam name="TSearch">A generic type of <see cref="IBaseSearch"/></typeparam>
        /// <param name="query">A <see cref="IQueryable{T}"/> that representing source query.</param>
        /// <param name="searchModel">An object of type <see cref="IBaseSearch"/> that representing page number and page size.</param>
        /// <param name="paginate">A <see cref="bool"/> value that asking for paginating data.</param>
        /// <param name="loadData">A <see cref="bool"/> value that asking for loading of related data.</param>
        /// <param name="cacheData">A <see cref="bool"/> value that asking for caching of data.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="Pagination{TModel}"/> instance that representing query results divided in each pages.</returns>
        public static Task<Pagination<TSource>> PaginateAsync<TSource, TSearch>(this IQueryable<TSource> query,
            TSearch searchModel, bool paginate, bool loadData, bool cacheData)
            where TSource : class, IEntityBase where TSearch : IBaseSearch
        {
            var page = searchModel?.PageNo ?? 1;
            var pageSize = searchModel?.PageSize ?? 10;

            return query.PaginateAsync(page, paginate, loadData, cacheData, pageSize);
        }
    }
}