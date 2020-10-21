using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R8.Lib.AspNetCore.TagHelpers
{
    [HtmlTargetElement("label", Attributes = "asp-for")]
    public class R8LabelTagHelper : LabelTagHelper
    {
        public R8LabelTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var required = For.Metadata.IsRequired;
            if (required)
                output.Attributes.Add("required", "required");
        }
    }
}