using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R8.AspNetCore.TagBuilders
{
    [HtmlTargetElement("span")]
    [HtmlTargetElement("button")]
    [HtmlTargetElement("option")]
    [HtmlTargetElement("select")]
    [HtmlTargetElement("input")]
    public class MissingDisabledTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-disabled")]
        public bool Disabled { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Disabled)
            {
                output.Attributes.Add("disabled", "");
            }
            base.Process(context, output);
        }
    }

    [HtmlTargetElement("span")]
    public class MissingFeaturesSpanTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-validation-for-name")]
        public string Name { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                output.Attributes.Add("data-valmsg-for", Name);
                output.Attributes.Add("data-valmsg-replace", "true");
            }
        }
    }

    [HtmlTargetElement("input")]
    public class MissingFeaturesInputTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        public MissingFeaturesInputTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        [HtmlAttributeNotBound, ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("asp-attributes")]
        public Dictionary<string, object> Attributes { get; set; }

        [HtmlAttributeName("asp-readonly")]
        public bool Readonly { get; set; }

        [HtmlAttributeName("asp-name")]
        public string Name { get; set; }

        [HtmlAttributeName("asp-checked")]
        public bool Checked { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Checked)
                output.Attributes.Add("checked", null);

            if (Readonly)
                output.Attributes.Add("readonly", "");

            if (Attributes?.Any() == true)
                foreach (var (key, value) in Attributes)
                    output.Attributes.Add(key, value);

            (_htmlHelper as IViewContextAware).Contextualize(ViewContext);
            if (!string.IsNullOrEmpty(Name))
            {
                output.Attributes.Add("name", Name);
                output.Attributes.Add("Id", _htmlHelper.GenerateIdFromName(Name));
            }
        }
    }

    [HtmlTargetElement("textarea")]
    public class MissingFeaturesTextAreaTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        public MissingFeaturesTextAreaTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        [HtmlAttributeNotBound, ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("asp-attributes")]
        public Dictionary<string, object> Attributes { get; set; }

        [HtmlAttributeName("asp-name")]
        public string Name { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Attributes?.Any() == true)
                foreach (var (key, value) in Attributes)
                    output.Attributes.Add(key, value);

            (_htmlHelper as IViewContextAware).Contextualize(ViewContext);
            if (!string.IsNullOrEmpty(Name))
            {
                output.Attributes.Add("name", Name);
                output.Attributes.Add("Id", _htmlHelper.GenerateIdFromName(Name));
            }
        }
    }

    [HtmlTargetElement("select")]
    public class MissingFeaturesSelectTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        public MissingFeaturesSelectTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        [HtmlAttributeNotBound, ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("asp-attributes")]
        public Dictionary<string, object> Attributes { get; set; }

        [HtmlAttributeName("asp-name")]
        public string Name { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Attributes?.Any() == true)
                foreach (var (key, value) in Attributes)
                    output.Attributes.Add(key, value);

            (_htmlHelper as IViewContextAware).Contextualize(ViewContext);
            if (!string.IsNullOrEmpty(Name))
            {
                output.Attributes.Add("name", Name);
                output.Attributes.Add("Id", _htmlHelper.GenerateIdFromName(Name));
            }
        }
    }

    [HtmlTargetElement("label")]
    public class MissingFeaturesLabelTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        public MissingFeaturesLabelTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        [HtmlAttributeNotBound, ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("asp-for-name")]
        public string ForName { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            (_htmlHelper as IViewContextAware).Contextualize(ViewContext);
            if (!string.IsNullOrEmpty(ForName))
                output.Attributes.Add("for", _htmlHelper.GenerateIdFromName(ForName));
        }
    }

    [HtmlTargetElement("option")]
    public class MissingFeaturesOptionTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-selected")]
        public bool Selected { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Selected)
                output.Attributes.Add("selected", null);
        }
    }
}