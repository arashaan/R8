using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R8.AspNetCore.TagBuilders.TagHelpers
{
    [HtmlTargetElement("script", Attributes = "asp-paste-key")]
    public class ScriptPasteTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-paste-key")]
        public string DeferDestinationId { get; set; }

        [HtmlAttributeNotBound, ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeNotBound]
        private IHtmlGenerator Generator { get; set; }

        public ScriptPasteTagHelper(IHtmlGenerator generator)
        {
            this.Generator = generator;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await base.ProcessAsync(context, output);

            if (!this.ViewContext.HttpContext.Items.ContainsKey(TagHelperCutPasteStorage.ItemsStorageKey))
            {
                output.SuppressOutput();
                return;
            }

            var storage = ViewContext.HttpContext.Items[TagHelperCutPasteStorage.ItemsStorageKey];
            if (!(storage is List<TagHelperCutPasteStorage> cutKeys))
                throw new ApplicationException($"Conversion failed for item flags {storage.GetType()} to flags" + typeof(Dictionary<string, TagHelperCutPasteStorage>));

            if (cutKeys.Count == 0)
            {
                output.SuppressOutput();
                return;
            }

            var componentsWithStorageKey = cutKeys.Where(x => x.CutPasteKey == DeferDestinationId).ToList();
            if (componentsWithStorageKey.Count == 0)
            {
                output.SuppressOutput();
                return;
            }

            var firstScript = componentsWithStorageKey.First();
            output.Content.SetHtmlContent(firstScript.TagHelperContent.GetContent());

            foreach (var attr in firstScript.Attributes)
                output.Attributes.Add(attr);

            if (componentsWithStorageKey.Count <= 0)
            {
                output.SuppressOutput();
                return;
            }

            for (var i = 1; i < componentsWithStorageKey.Count; i++)
            {
                var script = componentsWithStorageKey[i];
                var builder = new TagBuilder("script");

                var content = script.TagHelperContent.GetContent().Trim();
                if (string.IsNullOrEmpty(content))
                    continue;

                builder.MergeAttributes(script.Attributes.ToDictionary(x => x.Name, x => x.Value));
                builder.InnerHtml.AppendHtml(script.TagHelperContent.GetContent());
                output.PostElement.AppendHtml(builder);
            }
        }
    }
}