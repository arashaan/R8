using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R8.AspNetCore.TagBuilders.TagHelpers
{
    [HtmlTargetElement("button", Attributes = "asp-disabled")]
    public class CustomButtonTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-disabled")]
        public bool Disabled { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Disabled)
                output.Attributes.Add("disabled", "");

            base.Process(context, output);
        }
    }
}