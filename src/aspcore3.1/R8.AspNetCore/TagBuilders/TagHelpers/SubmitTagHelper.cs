using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using R8.AspNetCore.Enums;

namespace R8.AspNetCore.TagBuilders.TagHelpers
{
    [HtmlTargetElement("submit", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class SubmitTagHelper : TagHelper
    {
        [HtmlAttributeName("type")]
        public BsButtonTypes Style { get; set; } = BsButtonTypes.Success;

        public string Class { get; set; }

        [HtmlAttributeName("asp-disabled")]
        public bool Disabled { get; set; }

        public ButtonTypes Button { get; set; } = ButtonTypes.Button;

        [HtmlAttributeName("href")]
        public string Href { get; set; }

        public bool FullWidth { get; set; }
        public bool Small { get; set; } = true;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = (await output.GetChildContentAsync().ConfigureAwait(false)).GetContent(HtmlEncoder.Default);

            output.AddClass("btn", HtmlEncoder.Default);
            output.AddClass($"btn-{Style.ToString().ToLower()}", HtmlEncoder.Default);

            if (Disabled)
                output.Attributes.Add("disabled", "");

            if (Small)
                output.AddClass("btn-sm", HtmlEncoder.Default);

            if (FullWidth)
                output.AddClass("btn-block", HtmlEncoder.Default);

            if (!string.IsNullOrEmpty(Class))
            {
                var classArr = Class.Split(" ");
                if (classArr?.Any() == true)
                {
                    foreach (var @class in classArr)
                        output.AddClass(@class, HtmlEncoder.Default);
                }
            }

            switch (Button)
            {
                case ButtonTypes.Button:
                    output.TagName = "button";
                    output.Attributes.Add("type", "submit");
                    output.TagMode = TagMode.StartTagAndEndTag;
                    output.Content.AppendHtml(content);
                    break;

                case ButtonTypes.Input:
                    output.TagName = "input";
                    output.Attributes.Add("type", "submit");
                    output.TagMode = TagMode.SelfClosing;
                    output.Attributes.Add("value", content);
                    break;

                case ButtonTypes.Anchor:
                default:
                    output.TagName = "a";
                    output.TagMode = TagMode.StartTagAndEndTag;
                    output.Content.AppendHtml(content);
                    output.Attributes.Add("href", Href);
                    break;
            }
        }
    }
}