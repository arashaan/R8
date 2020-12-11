using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R8.AspNetCore.TagBuilders.TagHelpers
{
    [HtmlTargetElement("option")]
    public class CustomOptionTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-selected")]
        public bool Selected { get; set; }

        [HtmlAttributeName("asp-disabled")]
        public bool Disabled { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Disabled)
                output.Attributes.Add("disabled", "");

            if (Selected)
                output.Attributes.Add("selected", "");
        }
    }
}