using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R8.AspNetCore.TagBuilders.TagHelpers
{
    [HtmlTargetElement("title-bar", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class TitleBarTagHelper : TagHelper
    {
        public string Class { get; set; }

        [HtmlAttributeName("asp-for")]
        public ModelExpression AspFor { get; set; }

        public string For { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            string subject;
            bool isLabel;
            string name;
            if (AspFor != null)
            {
                name = AspFor.Name.Replace(".", "_");
                subject = AspFor.Metadata.DisplayName;
                isLabel = true;
            }
            else
            {
                subject = (await output.GetChildContentAsync()).GetContent();
                isLabel = !string.IsNullOrEmpty(For);
                name = For;
            }

            if (!string.IsNullOrEmpty(subject))
            {
                var tag = new TagBuilder(isLabel ? "label" : "span");
                tag.InnerHtml.AppendHtml(subject);
                if (isLabel)
                    tag.Attributes.Add("for", name);

                var hr = new TagBuilder("hr");

                output.TagName = "div";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.AppendHtml(hr).AppendHtml(tag).AppendHtml(hr);

                if (!string.IsNullOrEmpty(Class))
                    foreach (var cls in Class.Split(' '))
                        output.AddClass(cls, HtmlEncoder.Default);

                output.AddClass("title-bar", HtmlEncoder.Default);
            }
            else
            {
                output.TagName = "hr";
                output.TagMode = TagMode.StartTagAndEndTag;
            }
        }
    }
}