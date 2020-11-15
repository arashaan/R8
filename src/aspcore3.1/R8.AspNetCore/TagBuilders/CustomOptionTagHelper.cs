using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R8.AspNetCore.TagBuilders
{
    [HtmlTargetElement("option")]
    public class CustomOptionTagHelper : OptionTagHelper
    {
        [HtmlAttributeName("asp-selected")]
        public bool Selected { get; set; }

        [HtmlAttributeName("asp-disabled")]
        public bool Disabled { get; set; }

        public CustomOptionTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Disabled)
                output.Attributes.Add("disabled", "");

            if (Selected)
                output.Attributes.Add("selected", null);
        }
    }
}