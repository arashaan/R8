using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using R8.Lib.AspNetCore.Attributes;
using R8.Lib.Localization;

using System;
using System.Linq;
using System.Reflection;

namespace R8.Lib.AspNetCore.TagBuilders
{
    [HtmlTargetElement("select", Attributes = "asp-for")]
    public class R8SelectTagHelper : SelectTagHelper
    {
        private readonly ILocalizer _localizer;

        public R8SelectTagHelper(IHtmlGenerator generator, ILocalizer localizer) : base(generator)
        {
            _localizer = localizer;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var metadata = this.For.Metadata;
            var propName = metadata.PropertyName;

            var containerType = metadata.ContainerType;
            var propInfo = containerType
              .GetPublicProperties().Find(x => x.Name.Equals(propName, StringComparison.CurrentCulture));

            metadata.ApplyNewBindProperty(ref output);
            var filterAttribute = propInfo.GetCustomAttribute<FilterAttribute>();
            if (filterAttribute == null)
                return;

            var tempItems = filterAttribute.GetSelectListItems(_localizer);
            if (tempItems?.Any() != true)
                return;

            Items = tempItems.Select(x => new SelectListItem
            {
                Text = x.Text,
                Value = x.Value,
                Selected = Items?.FirstOrDefault(c => c.Value == x.Value)?.Selected ?? false
            }).ToList();

            output.Content.Clear();
            output.PostContent.Clear();
            output.PreContent.Clear();
            output.PostElement.Clear();
            output.PreElement.Clear();

            var (_, tagHelperOutput) = new SelectTagHelper(Generator)
            {
                For = For,
                ViewContext = ViewContext,
                Name = Name,
                Items = Items
            }.Init();

            output.Content.AppendHtml(tagHelperOutput.Content.GetContent());
            output.PostContent.AppendHtml(tagHelperOutput.PostContent.GetContent());
            output.PreContent.AppendHtml(tagHelperOutput.PreContent.GetContent());
            output.PostElement.AppendHtml(tagHelperOutput.PostElement.GetContent());
            output.PreElement.AppendHtml(tagHelperOutput.PreElement.GetContent());
        }
    }
}