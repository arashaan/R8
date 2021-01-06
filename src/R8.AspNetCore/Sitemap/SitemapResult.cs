using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using R8.AspNetCore.HttpContextExtensions;
using R8.AspNetCore.Routing;
using R8.AspNetCore.Sitemap.Models;
using R8.Lib;

namespace R8.AspNetCore.Sitemap
{
    /// <summary>
    /// Returns an <see cref="ActionResult"/> for presenting sitemap.xml
    /// </summary>
    public class SitemapResult : ActionResult
    {
        private readonly List<SitemapNode> _nodes;
        private string _nameSpace;

        /// <summary>
        /// Returns an <see cref="ActionResult"/> for presenting sitemap.xml
        /// </summary>
        /// <remarks>In this ctor, automatically scans for types under current namespace.</remarks>
        public SitemapResult()
        {
        }

        /// <summary>
        /// Returns an <see cref="ActionResult"/> for presenting sitemap.xml
        /// </summary>
        /// <param name="nameSpace">A namespace to scan nested types.</param>
        public SitemapResult(string nameSpace)
        {
            _nameSpace = nameSpace;
        }

        /// <summary>
        /// Returns an <see cref="ActionResult"/> for presenting sitemap.xml
        /// </summary>
        /// <param name="nodes">a collection of <see cref="ISitemapModel"/> models that can be either <see cref="SitemapIndexNode"/> or <see cref="SitemapNode"/>.</param>
        public SitemapResult(List<SitemapNode> nodes)
        {
            _nodes = nodes;
        }

        private static readonly XAttribute XmlNamespace = new XAttribute("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

        /// <summary>
        /// returns a collection of <see cref="PageModel" /> under given namespace with respect to <see cref="SitemapSettingsAttribute"/> attribute.
        /// </summary>
        /// <param name="nameSpace">A <see cref="string"/> value that representing an specific namespace to scan nested page types.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> object.</returns>
        public static IEnumerable<Type> ScanPageTypesUnderNamespace(string nameSpace)
        {
            var pageModels = Assembly
                .GetEntryAssembly()
                .GetTypes()
                .Where(x => !string.IsNullOrEmpty(x.Namespace)
                            && x.IsClass
                            && x.Namespace.StartsWith(nameSpace)
                            && x.IsSubclassOf(typeof(PageModel))
                            && x.GetCustomAttribute<SitemapSettingsAttribute>() != null
                            && x.GetCustomAttribute<SitemapIndexAttribute>() == null);
            return pageModels;
        }

        internal const SitemapChangeFrequency FrequencyDefault = SitemapChangeFrequency.NoNeed;
        internal const int PriorityDefault = 99999;

        private static ContentResult GetResultByPageTypes(ActionContext context, IReadOnlyCollection<Type> pageTypes)
        {
            var httpContext = context.HttpContext;
            var xml = new XDocument
            {
                Declaration = new XDeclaration("1.0", Encoding.UTF8.EncodingName, string.Empty)
            };

            var result = new ContentResult
            {
                ContentType = "application/xml",
                StatusCode = StatusCodes.Status200OK
            };
            if (pageTypes?.Any() == true)
            {
                var urlFactory = new UrlHelperFactory();
                var urlHelper = urlFactory.GetUrlHelper(context);
                var nodes = new Dictionary<Type, SitemapNode>();
                foreach (var pageType in pageTypes)
                {
                    var sitemapSettings = pageType.GetCustomAttribute<SitemapSettingsAttribute>();
                    if (sitemapSettings == null)
                        continue;

                    var relativePath = pageType.GetPagePath();
                    var pageUrl = urlHelper.Page(relativePath);
                    var model = new SitemapNode
                    {
                        Url = pageUrl,
                        LastModificationDate = DateTime.UtcNow
                    };

                    nodes.Add(pageType, model);
                }

                if (nodes.Count == 0)
                    throw new Exception($"Cannot find any page model with {typeof(SitemapSettingsAttribute)} attribute.");

                var baseUrl = httpContext.GetBaseUrl();
                var urlSet = new XElement("urlset", XmlNamespace);
                foreach (var (pageType, node) in nodes)
                {
                    var sitemapSettings = pageType.GetCustomAttribute<SitemapSettingsAttribute>();

                    if (string.IsNullOrEmpty(node.Url))
                        throw new NullReferenceException($"{nameof(node.Url)} needed");

                    var element = new XElement("url");
                    var url = baseUrl[..^1];
                    if (node.Url.StartsWith("/"))
                    {
                        url += node.Url;
                        if (node.Url.Equals("/"))
                            url += "index";
                    }

                    // var uri = new Uri(url);
                    // element.SetElementValue("loc", HttpUtility.UrlDecode(uri.AbsolutePath));
                    element.SetElementValue("loc", HttpUtility.UrlDecode(url));
                    element.SetElementValue("lastmod", node.LastModificationDate.ToString("yyyy-MM-ddThh:mm:ss") + "+00:00");

                    if (sitemapSettings.Priority != PriorityDefault)
                        element.SetElementValue("priority", sitemapSettings.Priority);

                    if (sitemapSettings.ChangeFrequency != FrequencyDefault)
                        element.SetElementValue("changefreq", sitemapSettings.ChangeFrequency.ToString().ToLower());

                    urlSet.Add(element);
                }
                xml.Add(urlSet);

                using var stringWriter = new Utf8StringWriter();
                using var xmlTextWriter = new XmlTextWriter(stringWriter);
                xml.WriteTo(xmlTextWriter);
                result.Content = stringWriter.ToString();
            }
            else
            {
                result.Content = string.Empty;
            }
            return result;
        }

        private static ContentResult GetResultBySitemapNodes(ActionContext context, IReadOnlyCollection<SitemapNode> nodes)
        {
            var httpContext = context.HttpContext;
            var xml = new XDocument
            {
                Declaration = new XDeclaration("1.0", Encoding.UTF8.EncodingName, string.Empty)
            };

            var result = new ContentResult
            {
                ContentType = "application/xml",
                StatusCode = StatusCodes.Status200OK
            };
            if (nodes?.Any() == true)
            {
                if (nodes.Any(x => x.LastModificationDate.Kind != DateTimeKind.Utc))
                    throw new Exception($"{nameof(ISitemapModel.LastModificationDate)} must be {nameof(DateTimeKind.Utc)}");

                var baseUrl = httpContext.GetBaseUrl();
                var urlSet = new XElement("urlset", XmlNamespace);
                foreach (var node in nodes)
                {
                    if (string.IsNullOrEmpty(node.Url))
                        throw new NullReferenceException($"{nameof(node.Url)} needed");

                    var element = new XElement("url");
                    var url = baseUrl[..^1];
                    if (node.Url.StartsWith("/"))
                    {
                        url += node.Url;
                        if (node.Url.Equals("/"))
                            url += "Index";
                    }

                    // var uri = new Uri(url);
                    element.SetElementValue("loc", HttpUtility.UrlDecode(url));
                    // element.SetElementValue("loc", HttpUtility.UrlDecode(uri.AbsolutePath));
                    element.SetElementValue("lastmod", node.LastModificationDate.ToString("yyyy-MM-ddThh:mm:ss") + "+00:00");

                    if (node.Priority != null && node.Priority.Value != PriorityDefault)
                        element.SetElementValue("priority", node.Priority);

                    if (node.ChangeFrequency != null && node.ChangeFrequency.Value != FrequencyDefault)
                        element.SetElementValue("changefreq", node.ChangeFrequency.ToString().ToLower());

                    urlSet.Add(element);
                }
                xml.Add(urlSet);

                using var stringWriter = new Utf8StringWriter();
                using var xmlTextWriter = new XmlTextWriter(stringWriter);
                xml.WriteTo(xmlTextWriter);
                result.Content = stringWriter.ToString();
            }
            else
            {
                result.Content = string.Empty;
            }
            return result;
        }

        private ContentResult GetResult(ActionContext context)
        {
            if (_nodes?.Any() == true)
                return GetResultBySitemapNodes(context, _nodes);

            if (string.IsNullOrEmpty(_nameSpace))
            {
                if (!(context.ActionDescriptor is CompiledPageActionDescriptor pageActionDescriptor))
                    throw new ArgumentOutOfRangeException($"Currently any type but {typeof(PageModel)} not supported.");

                _nameSpace = pageActionDescriptor.PageTypeInfo.Namespace;
            }

            var pageTypes = ScanPageTypesUnderNamespace(_nameSpace).ToList();
            return GetResultByPageTypes(context, pageTypes);
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            var result = GetResult(context);
            return result.ExecuteResultAsync(context);
        }

        public override void ExecuteResult(ActionContext context)
        {
            var result = GetResult(context);
            result.ExecuteResult(context);
        }
    }
}