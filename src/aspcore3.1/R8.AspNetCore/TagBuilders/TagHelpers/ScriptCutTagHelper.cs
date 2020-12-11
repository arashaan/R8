using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R8.AspNetCore.TagBuilders.TagHelpers
{
    [HtmlTargetElement("script", Attributes = "asp-cut-key")]
    public class ScriptCutTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-cut-key")]
        public string CutKey { get; set; }

        [HtmlAttributeNotBound, ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var deferedScripts = new List<TagHelperCutPasteStorage>();

            if (ViewContext.HttpContext.Items.ContainsKey(TagHelperCutPasteStorage.ItemsStorageKey))
            {
                deferedScripts = ViewContext.HttpContext.Items[TagHelperCutPasteStorage.ItemsStorageKey] as List<TagHelperCutPasteStorage>;
                if (deferedScripts == null)
                    throw new ApplicationException("Duplicate Items key : " + TagHelperCutPasteStorage.ItemsStorageKey);
            }
            else
            {
                ViewContext.HttpContext.Items.Add(TagHelperCutPasteStorage.ItemsStorageKey, deferedScripts);
            }

            var result = await output.GetChildContentAsync();
            deferedScripts.Add(new TagHelperCutPasteStorage
            {
                CutPasteKey = this.CutKey,
                TagHelperContent = result,
                Attributes = context.AllAttributes.Where(x => x.Name != "asp-cut-key").ToList()
            });

            output.Content.Clear();
            output.SuppressOutput();
        }
    }
}