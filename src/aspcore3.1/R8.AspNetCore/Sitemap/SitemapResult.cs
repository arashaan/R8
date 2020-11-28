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

using R8.AspNetCore.Sitemap.Models;
using R8.Lib;

namespace R8.AspNetCore.Sitemap
{
    public class SitemapResult : ActionResult
    {
        private readonly IEnumerable<ISitemapModel> _nodes;

        public SitemapResult()
        {
        }

        public SitemapResult(IEnumerable<ISitemapModel> nodes)
        {
            _nodes = nodes;
        }

        public override void ExecuteResult(ActionContext context)
        {
            var result = Prepare(context);
            result.ExecuteResult(context);
        }

        private static readonly XAttribute XmlNamespace = new XAttribute("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

        public static XElement CreateIndex(string baseUrl, IEnumerable<SitemapIndexNode> indexNodes)
        {
            if (baseUrl == null) throw new ArgumentNullException(nameof(baseUrl));
            if (indexNodes == null) throw new ArgumentNullException(nameof(indexNodes));

            var siteMapIndex = new XElement("sitemapindex", XmlNamespace);
            foreach (var indexNode in indexNodes)
            {
                if (string.IsNullOrEmpty(indexNode.Url))
                    throw new NullReferenceException($"{nameof(indexNode.Url)} needed");

                var url = new XElement("sitemap");
                var finalUrl = baseUrl;
                if (indexNode.Url.StartsWith("/"))
                    finalUrl += indexNode.Url.Substring(1);

                url.SetElementValue("loc", finalUrl);

                if (indexNode.LastModificationDate != null)
                    url.SetElementValue("lastmod", indexNode.LastModificationDate.Value.ToString("yyyy-MM-dd"));

                siteMapIndex.Add(url);
            }
            return siteMapIndex;
        }

        public static XElement CreateUrlSet(string baseUrl, IEnumerable<SitemapNode> nodes)
        {
            if (baseUrl == null) throw new ArgumentNullException(nameof(baseUrl));
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));

            var urlSet = new XElement("urlSet", XmlNamespace);
            foreach (var node in nodes)
            {
                if (string.IsNullOrEmpty(node.Url))
                    throw new NullReferenceException($"{nameof(node.Url)} needed");

                var element = new XElement("url");
                var url = baseUrl;
                if (node.Url.StartsWith("/"))
                    url += node.Url.Substring(1);

                element.SetElementValue("loc", HttpUtility.UrlDecode(url));

                if (node.LastModificationDate != null)
                    element.SetElementValue("lastmod", node.LastModificationDate.Value.ToString("yyyy-MM-dd"));

                if (node.Priority != null)
                    element.SetElementValue("priority", node.Priority);

                if (node.ChangeFrequency != null)
                    element.SetElementValue("changefreq", node.ChangeFrequency.Value.ToString().ToLower());

                urlSet.Add(element);
            }
            return urlSet;
        }

        public static IEnumerable<Type> ScanPageTypesUnderNamespace(string nameSpace)
        {
            var pageModels = Assembly
                .GetEntryAssembly()
                .GetTypes()
                .Where(x => !string.IsNullOrEmpty(x.Namespace)
                            && x.IsClass
                            && x.Namespace.StartsWith(nameSpace)
                            && x.IsSubclassOf(typeof(PageModel))
                            && x.GetCustomAttribute<SitemapNoIndexAttribute>() == null
                            && x.GetCustomAttribute<SitemapSettingsAttribute>() != null);
            return pageModels;
        }

        private ContentResult Prepare(ActionContext context)
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
            if (_nodes?.Any() == true)
            {
                if (_nodes.Any(x => x.LastModificationDate != null && x.LastModificationDate.Value.Kind != DateTimeKind.Utc))
                    throw new Exception($"{nameof(ISitemapModel.LastModificationDate)} must be {nameof(DateTimeKind.Utc)}");

                var baseUrl = httpContext.GetBaseUrl();
                switch (_nodes)
                {
                    case IEnumerable<SitemapNode> sitemapNodes:
                        {
                            var urlSet = CreateUrlSet(baseUrl, sitemapNodes);
                            xml.Add(urlSet);
                            break;
                        }
                    case IEnumerable<SitemapIndexNode> sitemapIndexNodes:
                        {
                            var indexNode = CreateIndex(baseUrl, sitemapIndexNodes);
                            xml.Add(indexNode);
                            break;
                        }

                    default:
                        throw new ArgumentOutOfRangeException();
                }

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

        public override Task ExecuteResultAsync(ActionContext context)
        {
            var result = Prepare(context);
            return result.ExecuteResultAsync(context);
        }
    }
}