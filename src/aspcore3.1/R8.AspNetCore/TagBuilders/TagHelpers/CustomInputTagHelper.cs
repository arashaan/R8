using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R8.AspNetCore.TagBuilders.TagHelpers
{
    [HtmlTargetElement("input")]
    public class CustomInputTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        public CustomInputTagHelper(IHtmlHelper htmlHelper)
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

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Disabled)
                output.Attributes.Add("disabled", "");

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