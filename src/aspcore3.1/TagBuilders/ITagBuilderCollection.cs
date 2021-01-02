using System.Collections.Generic;

using Microsoft.AspNetCore.Html;

namespace R8.AspNetCore.TagBuilders
{
    public interface ITagBuilderCollection
    {
        List<ITagBuilder> Nodes { get; }
        IHtmlContentBuilder InnerHtml { get; }
        bool HasInnerHtml { get; }
    }
}