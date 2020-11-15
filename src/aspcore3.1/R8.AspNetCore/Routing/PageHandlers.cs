using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using R8.Lib;

namespace R8.AspNetCore.Routing
{
    public static class PageHandlers
    {
        internal static IEnumerable<string> Namespaces(this Type pageType)
        {
            var nameSpace = pageType?.Namespace;
            if (string.IsNullOrEmpty(nameSpace))
                throw new Exception("Namespace must be not null or empty");

            var splitByDots = nameSpace.Split('.');
            return splitByDots;
        }

        public static ActionResult Combine(this RedirectToPageResult targetPageUrl, Dictionary<string, object> with)
        {
            var targetUrlQueryStrings = (targetPageUrl.RouteValues?.Any() == true
                ? $"?{string.Join("&", targetPageUrl.RouteValues.SelectMany(x => $"{x.Key}={x.Value}"))}"
                : "");
            var urlAndQueries = $"{targetPageUrl.PageName}{targetUrlQueryStrings}";
            var parsedUrl = ParseUrl(urlAndQueries);
            var parsedQueryBuilder = parsedUrl.QueryBuilder ?? new QueryBuilder();
            var dic = new ConcurrentDictionary<string, string>(parsedQueryBuilder);

            if (with?.Any() == true)
            {
                foreach (var (query, value) in with)
                {
                    if (value == null || string.IsNullOrEmpty(value.ToString()))
                        continue;

                    dic.AddOrUpdate(query, _ => value.ToString(), (a, b) => value.ToString());
                }
            }

            parsedQueryBuilder = new QueryBuilder(dic);
            var result =
              new RedirectToPageResult(targetPageUrl.PageName, targetPageUrl.PageHandler, parsedQueryBuilder, targetPageUrl.Permanent)
              {
                  Fragment = targetPageUrl.Fragment,
                  Host = targetPageUrl.Host,
                  PreserveMethod = targetPageUrl.PreserveMethod,
                  Protocol = targetPageUrl.Protocol,
                  UrlHelper = targetPageUrl.UrlHelper
              };
            return result;
        }

        public static ParsedUrl Combine(this Uri uri, Dictionary<string, object> with)
        {
            var urlAndQueries = uri.PathAndQuery;
            var (absolutePath, queryBuilder) = ParseUrl(urlAndQueries);
            var parsedQueryBuilder = queryBuilder ?? new QueryBuilder();
            var dic = new ConcurrentDictionary<string, string>(parsedQueryBuilder);

            if (with?.Any() == true)
            {
                foreach (var (query, value) in with)
                {
                    if (value == null || string.IsNullOrEmpty(value.ToString()))
                        continue;

                    dic.AddOrUpdate(query, _ => value.ToString(), (a, b) => value.ToString());
                }
            }

            parsedQueryBuilder = new QueryBuilder(dic);
            var result = new ParsedUrl(absolutePath, parsedQueryBuilder);
            return result;
        }

        public static Dictionary<string, object> ToDictionary(this QueryBuilder queryBuilder)
        {
            var que = queryBuilder?
                .Select(x => new { x.Key, x.Value })?
                .GroupBy(x => x.Key)?
                .Select(x => new { x.Last().Key, x.Last().Value })?
                .ToList();
            return que?.Any() == true
                ? que.ToDictionary(x => x.Key, x => (object)x.Value)
                : default;
        }

        public static Dictionary<string, string> ToDictionary(this QueryString queryString)
        {
            if (queryString == null || !queryString.HasValue)
                return null;

            var dic = ParseQueryString(queryString.Value);
            return dic;
        }

        public static Dictionary<string, string> ParseQueryString(string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
                return null;

            var query = queryString;
            if (query.StartsWith("?"))
                query = query.Substring(1, query.Length - 1);

            var dic = query
                .Split("&")
                .Select(x => x.Split("="))
                .Where(x => x.Length == 2)
                .Select(x => new { Key = x[0], Value = x[1] })
                .ToDictionary(x => x.Key, x => WebUtility.UrlDecode(x.Value));
            return dic;
        }

        public static Uri GetUrlUri(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            const string domainUrl = "http://localhost:8080";

            string finalUrl;
            if (url.StartsWith("/"))
            {
                finalUrl = $"{domainUrl}{url}";
            }
            else
            {
                finalUrl = url.StartsWith("http")
                    ? url
                    : $"{domainUrl}/{url}";
            }

            var uri = new Uri(finalUrl);
            return uri;
        }

        public static Dictionary<string, string> GetQueryStrings(this Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            var queries = uri.Query;
            if (string.IsNullOrEmpty(queries))
                return null;

            var result = ParseQueryString(queries);
            return result;
        }

        internal static ParsedUrl ParseUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(url);

            var uri = GetUrlUri(url);
            // var baseUri = uri.GetComponents(UriComponents.Scheme | UriComponents.Host | UriComponents.Port,
            // UriFormat.UriEscaped);
            var absolutePath = uri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
            absolutePath = $"{(!absolutePath.StartsWith("/") ? "/" : "")}{absolutePath}";
            var queries = uri.GetQueryStrings();

            var result = new ParsedUrl(absolutePath, queries == null ? null : new QueryBuilder(queries));
            return result;
        }

        internal static string GetPath(Type type, PageHandlerConfiguration currentConfig)
        {
            var sectionSeparator = currentConfig.UseBackslash ? @"\" : "/";
            var namespaces = type.Namespaces().ToList();
            var manageNamespace = new List<string>();

            var pagesIndex = namespaces.IndexOf(currentConfig.PagesFolder);
            for (var i = 0; i < namespaces.Count; i++)
            {
                var text = namespaces[i];
                if (currentConfig.StartWithPages && i >= pagesIndex)
                    manageNamespace.Add(text);

                if (!currentConfig.StartWithPages && i > pagesIndex)
                    manageNamespace.Add(text);
            }

            var addressArray = new List<string>();
            if (manageNamespace.Count > 0)
                addressArray.AddRange(manageNamespace);

            addressArray.Add(type.Name.Replace("Model", "", StringComparison.Ordinal));

            if (!currentConfig.EndWithIndex && addressArray.Last().Equals(currentConfig.IndexPage, StringComparison.InvariantCultureIgnoreCase))
                addressArray.RemoveAt(addressArray.LastIndexOf(currentConfig.IndexPage));

            if (!currentConfig.EndWithIndex && addressArray[0] == currentConfig.PagesFolder)
                addressArray.RemoveAt(0);

            var address = $"{sectionSeparator}{string.Join(sectionSeparator, addressArray.ToArray())}";
            return address;
        }

        public static string GetPagePath<TPage>(this ICulturalizedUrlHelper urlHelper) where TPage : PageModelBase
        {
            var currentConfig = new PageHandlerConfiguration();
            var address = GetPath(typeof(TPage), currentConfig);

            var url = urlHelper.Page(address);
            return url;
        }

        public static string GetPagePath(this Type type, Action<PageHandlerConfiguration> config = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if ((type == typeof(PageModelBase)) && !type.IsSubclassOf(typeof(Microsoft.AspNetCore.Mvc.RazorPages.PageModel)))
                throw new Exception(nameof(type));

            var currentConfig = new PageHandlerConfiguration();
            config?.Invoke(currentConfig);

            var address = GetPath(type, currentConfig);
            if (currentConfig.RouteDictionary == null)
                return address;

            try
            {
                var routeDic = currentConfig.RouteDictionary.ToDictionary();
                if (routeDic.Count == 0) return address;

                var (absolutePath, queryBuilder) = ParseUrl(address);
                queryBuilder ??= new QueryBuilder();
                foreach (var (key, value) in routeDic)
                    queryBuilder.Add(key, value.ToString());

                var result = absolutePath + queryBuilder.ToQueryString();
                return result;
            }
            catch
            {
                return address;
            }
        }
    }
}