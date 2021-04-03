using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R8.AspNetCore.TagBuilders.TagHelpers
{
    [HtmlTargetElement("breadcrumb-container", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class BreadcrumbContainerTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        internal const string BreadcrumbListContextName = "BreadcrumbList";

        public BreadcrumbContainerTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeNotBound, ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            _httpContextAccessor.HttpContext.Items[BreadcrumbListContextName] = true;
            var content = await output.GetChildContentAsync();
            var ol = content.GetTagBuilder();
            var olNodes = ol.GetInnerHtmlNodes();

            var position = 1;
            var result = new List<TagBuilder>();
            if (olNodes?.Any() == true)
            {
                for (var index = 1; index <= olNodes.Count; index++)
                {
                    var li = olNodes[index - 1];
                    if (!li.Attributes.Any(x =>
                        x.Key.Equals("itemtype", StringComparison.InvariantCultureIgnoreCase) &&
                        x.Value.Equals("http://schema.org/ListItem", StringComparison.InvariantCultureIgnoreCase)))
                        continue;

                    var nodes = li.GetInnerHtmlNodes();
                    var a = nodes.Find(x =>
                        x.TagName.Equals("a", StringComparison.InvariantCultureIgnoreCase));

                    var metaPosition = nodes.Find(x =>
                       x.Attributes.Any(c =>
                           c.Key.Equals("itemprop", StringComparison.InvariantCultureIgnoreCase) &&
                           c.Value.Equals("position", StringComparison.InvariantCultureIgnoreCase)));

                    var span = nodes.Find(x =>
                        x.Attributes.Any(c =>
                            c.Key.Equals("itemprop", StringComparison.InvariantCultureIgnoreCase) &&
                            c.Value.Equals("name", StringComparison.InvariantCultureIgnoreCase)));

                    var classList = li.Attributes.ContainsKey("class")
                        ? li.Attributes["class"]?.Split(" ").ToList()
                        : new List<string>();

                    var positionIncrement = false;
                    if (index < olNodes.Count)
                    {
                        // position < last

                        if (classList?.Any(x => x.Equals("active")) == true)
                            li.Attributes.Remove("class");

                        if (a != null)
                        {
                            li.InnerHtml.Clear();
                            li.InnerHtml.AppendHtml(a);

                            if (metaPosition != null)
                                positionIncrement = true;
                        }
                        else
                        {
                            li.Attributes.Remove("aria-current");
                            li.Attributes.Remove("itemscope");
                            li.Attributes.Remove("itemprop");
                            li.Attributes.Remove("itemtype");

                            li.InnerHtml.Clear();
                            if (span != null)
                                li.InnerHtml.AppendHtml(span.InnerHtml);
                        }
                    }
                    else
                    {
                        // position = last
                        if (a != null)
                        {
                            var currentPage = ViewContext.HttpContext.Request.GetEncodedPathAndQuery();
                            if (a.Attributes["href"].Equals(currentPage, StringComparison.InvariantCulture))
                            {
                                li.AddCssClass("active");
                                li.InnerHtml.Clear();
                                li.InnerHtml.AppendHtml(a.InnerHtml);
                            }
                            else
                            {
                                li.InnerHtml.Clear();
                                li.InnerHtml.AppendHtml(a);
                            }
                        }
                        else if (span != null)
                        {
                            li.AddCssClass("active");
                            li.InnerHtml.Clear();
                            li.InnerHtml.AppendHtml(span);
                        }

                        if (metaPosition != null)
                            positionIncrement = true;
                    }

                    if (positionIncrement)
                    {
                        metaPosition.Attributes["content"] = position.ToString();
                        li.InnerHtml.AppendHtml(metaPosition);
                        position++;
                    }
                    result.Add(li);
                }
            }

            output.Content.Clear();
            if (result.Any())
            {
                foreach (var tagBuilder in result)
                    output.Content.AppendHtml(tagBuilder);

                output.TagMode = TagMode.StartTagAndEndTag;
                output.TagName = "ol";
                output.Attributes.Add("itemscope", "");
                output.Attributes.Add("itemtype", "http://schema.org/BreadcrumbList");
            }
            else
            {
                output.SuppressOutput();
            }
        }
    }
}