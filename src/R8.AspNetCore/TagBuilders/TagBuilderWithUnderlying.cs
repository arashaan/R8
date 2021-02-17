using Microsoft.AspNetCore.Mvc.Rendering;

namespace R8.AspNetCore.TagBuilders
{
    internal class TagBuilderWithUnderlying : TagBuilder, ITagBuilder
    {
        public TagBuilderWithUnderlying(string tagName) : base(tagName)
        {
        }

        public ITagBuilderCollection Nodes { get; set; } = new TagBuilderCollection();

        public string GetString()
        {
            return ((TagBuilder)this).GetString();
        }

        public TagBuilder ToTagBuilder()
        {
            return this;
        }
    }
}