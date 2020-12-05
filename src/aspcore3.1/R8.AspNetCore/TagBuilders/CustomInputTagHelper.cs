using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using R8.AspNetCore.Attributes;

namespace R8.AspNetCore.TagBuilders
{
    [HtmlTargetElement("input", Attributes = "asp-for")]
    public class CustomInputTagHelper : InputTagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        public CustomInputTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper) : base(generator)
        {
            _htmlHelper = htmlHelper;
        }

        [HtmlAttributeName("asp-disabled")]
        public bool Disabled { get; set; }

        [HtmlAttributeName("asp-attributes")]
        public Dictionary<string, object> Attributes { get; set; }

        [HtmlAttributeName("asp-readonly")]
        public bool Readonly { get; set; }

        [HtmlAttributeName("asp-checked")]
        public bool Checked { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (Disabled)
                output.Attributes.Add("disabled", "");

            var validation = For
                .Metadata
                .ValidatorMetadata
                .Where(x => x.GetType() == typeof(ValueValidationAttribute))
                .Select(x => x as ValueValidationAttribute)
                .FirstOrDefault();

            this.For.Metadata.ApplyNewBindProperty(ref output);
            if (validation == null)
                return;

            if (!output.IsContentModified)
            {
                output.Attributes.Add("data-toggle", "tooltip");
                output.Attributes.Add("data-placement", "bottom");
                output.Attributes.Add("title", validation.FormatErrorMessage(null));
                output.Attributes.Add("placeholder", validation.FormatErrorMessage(null));
            }

            if (Checked)
                output.Attributes.Add("checked", null);

            if (Readonly)
                output.Attributes.Add("readonly", "");

            if (Attributes?.Any() == true)
                foreach (var (key, value) in Attributes)
                    output.Attributes.Add(key, value);
        }
    }
}