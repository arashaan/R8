using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;

namespace R8.AspNetCore.TagBuilders
{
    internal class TagBuilderCollection : ITagBuilderCollection
    {
        public List<ITagBuilder> Nodes { get; set; } = new List<ITagBuilder>();
        public IHtmlContentBuilder InnerHtml => new HtmlContentBuilder(Nodes.Cast<object>().ToList());

        public bool HasInnerHtml => !string.IsNullOrEmpty(InnerHtml.GetString());

        public override string ToString()
        {
            return InnerHtml.GetString();
        }
    }
}