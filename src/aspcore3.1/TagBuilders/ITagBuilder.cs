using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace R8.AspNetCore.TagBuilders
{
    public interface ITagBuilder
    {
        ITagBuilderCollection Nodes { get; }
        AttributeDictionary Attributes { get; }
        bool HasInnerHtml { get; }
        IHtmlContentBuilder InnerHtml { get; }
        string TagName { get; }
        TagRenderMode TagRenderMode { get; }

        string GetString();
    }
}