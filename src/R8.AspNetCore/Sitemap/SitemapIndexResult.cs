using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

using Microsoft.AspNetCore.Hosting;
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
    public class SitemapIndexResult : ActionResult
    {
        private string _nameSpace;

        /// <summary>
        /// Returns an <see cref="ActionResult"/> for presenting an sitemap indexer for other sitemaps.
        /// </summary>
        /// <param name="nameSpace">A namespace to scan nested types.</param>
        public SitemapIndexResult(string nameSpace)
        {
        }

        /// <summary>
        /// Returns an <see cref="ActionResult"/> for presenting an sitemap indexer for other sitemaps.
        /// </summary>
        /// <remarks>In this ctor, automatically scans for types under current namespace.</remarks>
        public SitemapIndexResult()
        {
        }

        public static DateTimeOffset GetFileModification(IWebHostEnvironment _environment, string absolutePath)
        {
            var fileProvider = _environment.ContentRootFileProvider;
            var internalPath = $"Pages/{absolutePath[1..]}.cshtml";
            var razorDate = fileProvider.GetFileInfo(internalPath).LastModified;
            var razorModelDate = fileProvider.GetFileInfo($"{internalPath}.cs").LastModified;

            return razorModelDate >= razorDate
                ? razorModelDate
                : razorDate;
        }

        /// <summary>
        /// returns a collection of <see cref="PageModel" /> under given namespace with respect to <see cref="SitemapIndexAttribute"/> attribute.
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
                            && x.GetCustomAttribute<SitemapSettingsAttribute>() == null
                            && x.GetCustomAttribute<SitemapIndexAttribute>() != null);
            return pageModels;
        }

        private static readonly XAttribute XmlNamespace = new XAttribute("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

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
                    throw new Exception($"Cannot find any page model with {typeof(SitemapIndexAttribute)} attribute.");

                var baseUrl = httpContext.GetBaseUrl();
                var siteMapIndex = new XElement("sitemapindex", XmlNamespace);
                foreach (var (pageType, indexNode) in nodes)
                {
                    if (string.IsNullOrEmpty(indexNode.Url))
                        throw new NullReferenceException($"{nameof(indexNode.Url)} needed");

                    var element = new XElement("sitemap");
                    var url = baseUrl[..^1];
                    if (indexNode.Url.StartsWith("/"))
                        url += indexNode.Url;

                    element.SetElementValue("loc", HttpUtility.UrlDecode(url));
                    element.SetElementValue("lastmod", indexNode.LastModificationDate.ToString("yyyy-MM-ddThh:mm:ss") + "+00:00");

                    siteMapIndex.Add(element);
                }
                xml.Add(siteMapIndex);

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
            var pageTypes = new List<Type>();
            if (!string.IsNullOrEmpty(_nameSpace))
                return GetResultByPageTypes(context, pageTypes);

            if (!(context.ActionDescriptor is CompiledPageActionDescriptor pageActionDescriptor))
                throw new ArgumentOutOfRangeException($"Currently any type but {typeof(PageModel)} not supported.");

            _nameSpace = pageActionDescriptor.PageTypeInfo.Namespace;
            pageTypes = ScanPageTypesUnderNamespace(_nameSpace).ToList();

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