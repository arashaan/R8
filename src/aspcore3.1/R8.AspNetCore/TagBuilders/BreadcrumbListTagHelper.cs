using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R8.AspNetCore.TagBuilders
{
    //public class BreadcrumbContext
    //{
    //    public List<IHtmlContent> Items { get; set; } = new List<IHtmlContent>();
    //}

    [HtmlTargetElement("breadcrumb-list", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class BreadcrumbListTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        internal const string BreadcrumbListContextName = "BreadcrumbList";

        public BreadcrumbListTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            _httpContextAccessor.HttpContext.Items[BreadcrumbListContextName] = true;
            var content = await output.GetChildContentAsync();
            var tagBuilderCollection = content.GetTagBuilders();

            var position = 1;
            var result = new List<TagBuilderWithUnderlying>();
            if (tagBuilderCollection.Nodes?.Any() == true)
            {
                for (var index = 1; index <= tagBuilderCollection.Nodes.Count; index++)
                {
                    var tagBuilder = tagBuilderCollection.Nodes[index - 1] as TagBuilderWithUnderlying;
                    if (!tagBuilder.Attributes.Any(x => x.Key.Equals("itemtype", StringComparison.InvariantCultureIgnoreCase) && x.Value.Equals("http://schema.org/ListItem", StringComparison.InvariantCultureIgnoreCase)))
                        continue;

                    var anchorTag = (TagBuilderWithUnderlying)tagBuilder.Nodes.Nodes.Find(x =>
                        x.TagName.Equals("a", StringComparison.InvariantCultureIgnoreCase));

                    var positionTag = (TagBuilderWithUnderlying)tagBuilder.Nodes.Nodes.Find(x =>
                       x.Attributes.Any(c =>
                           c.Key.Equals("itemprop", StringComparison.InvariantCultureIgnoreCase) &&
                           c.Value.Equals("position", StringComparison.InvariantCultureIgnoreCase)));

                    var spanTag = (TagBuilderWithUnderlying)tagBuilder.Nodes.Nodes.Find(x =>
                        x.Attributes.Any(c =>
                            c.Key.Equals("itemprop", StringComparison.InvariantCultureIgnoreCase) &&
                            c.Value.Equals("name", StringComparison.InvariantCultureIgnoreCase)));

                    if (index < tagBuilderCollection.Nodes.Count)
                    {
                        if (anchorTag == null)
                        {
                            tagBuilder.Attributes.Remove("aria-current");
                            tagBuilder.Attributes.Remove("itemscope");
                            tagBuilder.Attributes.Remove("itemprop");
                            tagBuilder.Attributes.Remove("itemtype");

                            tagBuilder.InnerHtml.Clear();
                            if (spanTag != null)
                                tagBuilder.InnerHtml.AppendHtml(spanTag);
                        }
                        else
                        {
                            tagBuilder.InnerHtml.Clear();
                            tagBuilder.InnerHtml.AppendHtml(anchorTag);
                            if (positionTag != null)
                            {
                                positionTag.Attributes["content"] = position.ToString();
                                tagBuilder.InnerHtml.AppendHtml(positionTag);
                                position++;
                            }
                        }
                    }
                    else
                    {
                        tagBuilder.InnerHtml.Clear();
                        if (anchorTag != null)
                            tagBuilder.InnerHtml.AppendHtml(anchorTag);
                        else
                            if (spanTag != null)
                            tagBuilder.InnerHtml.AppendHtml(spanTag);

                        if (positionTag != null)
                        {
                            positionTag.Attributes["content"] = position.ToString();
                            tagBuilder.InnerHtml.AppendHtml(positionTag);
                            position++;
                        }
                    }

                    result.Add(tagBuilder);
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