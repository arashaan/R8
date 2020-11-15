using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R8.AspNetCore.TagBuilders
{
    [HtmlTargetElement("textarea")]
    public class CustomTextAreaTagHelper : TextAreaTagHelper
    {
        //private readonly IHtmlHelper _htmlHelper;

        public CustomTextAreaTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper) : base(generator)
        {
            //_htmlHelper = htmlHelper;
        }

        [HtmlAttributeName("asp-attributes")]
        public Dictionary<string, object> Attributes { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Attributes?.Any() == true)
                foreach (var (key, value) in Attributes)
                    output.Attributes.Add(key, value);

            //(_htmlHelper as IViewContextAware).Contextualize(ViewContext);
            //if (!string.IsNullOrEmpty(Name))
            //{
            //    output.Attributes.Add("name", Name);
            //    output.Attributes.Add("Id", _htmlHelper.GenerateIdFromName(Name));
            //}
        }
    }
}