using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using R8.AspNetCore.Attributes;

namespace R8.AspNetCore.TagBuilders.TagHelpers
{
    [HtmlTargetElement("input", Attributes = "asp-for")]
    public class ExperimentalInputTagHelper : InputTagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        public ExperimentalInputTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper) : base(generator)
        {
            _htmlHelper = htmlHelper;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var validation = For
                .Metadata
                .ValidatorMetadata
                .Where(x => x.GetType() == typeof(ValueValidationAttribute))
                .Select(x => x as ValueValidationAttribute)
                .FirstOrDefault();

            this.For.Metadata.ApplyNewBindProperty(ref output);
            if (validation == null)
                return;

            //if (!output.IsContentModified)
            //{
            //    output.Attributes.Add("data-toggle", "tooltip");
            //    output.Attributes.Add("data-placement", "bottom");
            //    output.Attributes.Add("title", validation.FormatErrorMessage(null));
            //    output.Attributes.Add("placeholder", validation.FormatErrorMessage(null));
            //}
        }
    }
}