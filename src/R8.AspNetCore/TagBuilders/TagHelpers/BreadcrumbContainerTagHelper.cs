using Microsoft.AspNetCore.Http;
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

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            _httpContextAccessor.HttpContext.Items[BreadcrumbListContextName] = true;
            var content = await output.GetChildContentAsync();
            var ol = content.GetTagBuilders();

            var position = 1;
            var result = new List<TagBuilderWithUnderlying>();
            if (ol.Nodes?.Any() == true)
            {
                for (var index = 1; index <= ol.Nodes.Count; index++)
                {
                    var li = ol.Nodes[index - 1] as TagBuilderWithUnderlying;
                    if (!li.Attributes.Any(x =>
                        x.Key.Equals("itemtype", StringComparison.InvariantCultureIgnoreCase) &&
                        x.Value.Equals("http://schema.org/ListItem", StringComparison.InvariantCultureIgnoreCase)))
                        continue;

                    var a = (TagBuilderWithUnderlying)li.Nodes.Nodes.Find(x =>
                        x.TagName.Equals("a", StringComparison.InvariantCultureIgnoreCase));

                    var metaPosition = (TagBuilderWithUnderlying)li.Nodes.Nodes.Find(x =>
                       x.Attributes.Any(c =>
                           c.Key.Equals("itemprop", StringComparison.InvariantCultureIgnoreCase) &&
                           c.Value.Equals("position", StringComparison.InvariantCultureIgnoreCase)));

                    var span = (TagBuilderWithUnderlying)li.Nodes.Nodes.Find(x =>
                        x.Attributes.Any(c =>
                            c.Key.Equals("itemprop", StringComparison.InvariantCultureIgnoreCase) &&
                            c.Value.Equals("name", StringComparison.InvariantCultureIgnoreCase)));

                    if (index < ol.Nodes.Count)
                    {
                        // position < last
                        if (li.Attributes.ContainsKey("class"))
                        {
                            var classAttr = li.Attributes["class"];
                            if (!string.IsNullOrEmpty(classAttr))
                            {
                                var classes = classAttr.Split(" ").ToList();
                                if (classes.Any())
                                {
                                    if (classes.Contains("active"))
                                    {
                                        classes.Remove("active");
                                        li.Attributes.Remove("class");

                                        if (classes?.Any() == true)
                                            foreach (var @class in classes)
                                                li.AddCssClass(@class);
                                    }
                                }
                            }
                        }

                        if (a == null)
                        {
                            li.Attributes.Remove("aria-current");
                            li.Attributes.Remove("itemscope");
                            li.Attributes.Remove("itemprop");
                            li.Attributes.Remove("itemtype");

                            li.InnerHtml.Clear();
                            if (span != null)
                                li.InnerHtml.AppendHtml(span.InnerHtml);
                        }
                        else
                        {
                            li.InnerHtml.Clear();
                            li.InnerHtml.AppendHtml(a);
                            if (metaPosition != null)
                            {
                                metaPosition.Attributes["content"] = position.ToString();
                                li.InnerHtml.AppendHtml(metaPosition);
                                position++;
                            }
                        }
                    }
                    else
                    {
                        // position = last
                        if (li.Attributes.ContainsKey("class"))
                        {
                            var classAttr = li.Attributes["class"];
                            if (!string.IsNullOrEmpty(classAttr))
                            {
                                var classes = classAttr.Split(" ").ToList();
                                if (classes.Any())
                                {
                                    if (!classes.Contains("active"))
                                        li.AddCssClass("active");
                                }
                                else
                                {
                                    li.AddCssClass("active");
                                }
                            }
                            else
                            {
                                li.AddCssClass("active");
                            }
                        }
                        else
                        {
                            li.AddCssClass("active");
                        }

                        li.InnerHtml.Clear();
                        if (a != null)
                            li.InnerHtml.AppendHtml(a.InnerHtml);
                        else if (span != null)
                            li.InnerHtml.AppendHtml(span);

                        if (metaPosition != null)
                        {
                            metaPosition.Attributes["content"] = position.ToString();
                            li.InnerHtml.AppendHtml(metaPosition);
                            position++;
                        }
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