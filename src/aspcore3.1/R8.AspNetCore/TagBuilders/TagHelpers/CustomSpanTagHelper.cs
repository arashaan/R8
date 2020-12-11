using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R8.AspNetCore.TagBuilders.TagHelpers
{
    [HtmlTargetElement("span")]
    public class CustomSpanTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-validation-for-name")]
        public string Name { get; set; }

        [HtmlAttributeName("asp-disabled")]
        public bool Disabled { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Disabled)
                output.Attributes.Add("disabled", "");

            if (!string.IsNullOrEmpty(Name))
            {
                output.Attributes.Add("data-valmsg-for", Name);
                output.Attributes.Add("data-valmsg-replace", "true");
            }
        }
    }
}