using System.Collections.Generic;

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R8.AspNetCore.TagBuilders
{
    internal class TagHelperCutPasteStorage
    {
        public const string ItemsStorageKey = "a2b459c4-3c62-4a90-977a-5999eb5978c5";

        // CutPasteKey identifies the appartenence of the cuted part to the paste section
        public string CutPasteKey { get; set; }

        // Cut script TagContent
        public TagHelperContent TagHelperContent { get; set; }

        // Attributes belonging to the cut script
        public List<TagHelperAttribute> Attributes { get; set; }
    }
}