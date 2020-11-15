using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R8.AspNetCore.TagBuilders
{
    [HtmlTargetElement("label", Attributes = "asp-for")]
    public class CustomLabelTagHelper : LabelTagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        [HtmlAttributeName("asp-for-name")]
        public string ForName { get; set; }

        public CustomLabelTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper) : base(generator)
        {
            _htmlHelper = htmlHelper;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var required = For.Metadata.IsRequired;
            if (required)
                output.Attributes.Add("required", "required");

            (_htmlHelper as IViewContextAware).Contextualize(ViewContext);
            if (!string.IsNullOrEmpty(ForName))
                output.Attributes.Add("for", _htmlHelper.GenerateIdFromName(ForName));
        }
    }
}