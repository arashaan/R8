using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

using HtmlAgilityPack;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using R8.Lib;

namespace R8.AspNetCore.TagBuilders
{
    public static class Extensions
    {
        public static ModelExpression GetPropertyModelExpression(this ModelExpression model, string propertyName)
        {
            return new ModelExpression($"{model.Name}.{propertyName}", model.ModelExplorer.GetExplorerForProperty(propertyName));
        }

        public static void ApplyNewBindProperty(this ModelMetadata metadata, ref TagHelperOutput output)
        {
            var propName = metadata.PropertyName;

            var containerType = metadata.ContainerType;
            var propInfo = containerType
                .GetPublicProperties().Find(x => x.Name.Equals(propName, StringComparison.CurrentCulture));

            var bindPropertyAttribute = propInfo?.GetCustomAttribute<BindPropertyAttribute>();
            if (bindPropertyAttribute == null)
                return;

            if (!string.IsNullOrEmpty(bindPropertyAttribute.Name))
                output.Attributes.Insert(0, new TagHelperAttribute("name", bindPropertyAttribute.Name));
        }

        /// <summary>
        /// Returns a generated instance of <see cref="ITagBuilder"/> by given html string.
        /// </summary>
        /// <param name="tag">An specific html string that representing an html tag.</param>
        /// <returns>An <see cref="ITagBuilder"/> object.</returns>
        public static ITagBuilder GetTagBuilder(this IHtmlContent tag)
        {
            var contentToStr = tag.GetString();
            var decoded = HttpUtility.HtmlDecode(contentToStr);
            var result = GetTagBuilder(decoded);
            return result;
        }

        /// <summary>
        /// Returns a collection of generated instance of <see cref="ITagBuilder"/> by given html string.
        /// </summary>
        /// <param name="tag">An specific html string that representing an list of html tags.</param>
        /// <returns>An <see cref="ITagBuilderCollection"/> object.</returns>
        public static ITagBuilderCollection GetTagBuilders(this IHtmlContent tag)
        {
            var contentToStr = tag.GetString();
            var decoded = HttpUtility.HtmlDecode(contentToStr);
            var result = GetTagBuilders(decoded);
            return result;
        }

        /// <summary>
        /// Returns a generated instance of <see cref="ITagBuilder"/> by given html string.
        /// </summary>
        /// <param name="tag">An specific html string that representing an html tag.</param>
        /// <returns>An <see cref="ITagBuilder"/> object.</returns>
        public static ITagBuilder GetTagBuilder(this Func<string, IHtmlContent> tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            var contentToStr = tag.Invoke("").GetString();
            var decoded = HttpUtility.HtmlDecode(contentToStr);
            var result = GetTagBuilder(decoded);
            return result;
        }

        /// <summary>
        /// Returns a collection of generated instance of <see cref="ITagBuilder"/> by given html string.
        /// </summary>
        /// <param name="html">An specific html string that representing an list of html tags.</param>
        /// <returns>An <see cref="ITagBuilderCollection"/> object.</returns>
        public static ITagBuilderCollection GetTagBuilders(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var result = new TagBuilderCollection();
            if (doc.DocumentNode?.ChildNodes == null || !doc.DocumentNode.ChildNodes.Any() ||
                doc.DocumentNode.ChildNodes.All(x => x.NodeType != HtmlNodeType.Element))
                return result;

            foreach (var node in doc.DocumentNode.ChildNodes)
            {
                if (node.NodeType != HtmlNodeType.Element)
                    continue;

                var tagBuilder = new TagBuilderWithUnderlying(node.Name);
                tagBuilder.MergeAttributes(node.Attributes.ToDictionary(x => x.Name, x => x.Value));
                tagBuilder.InnerHtml.AppendHtml(node.InnerHtml);

                if (tagBuilder.HasInnerHtml)
                    tagBuilder.Nodes = GetTagBuilders(tagBuilder.InnerHtml);

                result.Nodes.Add(tagBuilder);
            }

            return result;
        }

        /// <summary>
        /// Returns an <see cref="ITagBuilder"/> object from given html tag.
        /// </summary>
        /// <param name="html">A <see cref="string"/> value that representing an html tag.</param>
        /// <returns>A <see cref="ITagBuilder"/> object.</returns>
        public static ITagBuilder GetTagBuilder(string html)
        {
            if (html == null)
                return null;

            var node = GetTagBuilders(html);
            var nodes = node?.Nodes;
            var firstNode = nodes?.FirstOrDefault();
            return firstNode;
        }

        /// <summary>
        /// Returns a rendered html string of given <see cref="IHtmlContent"/> value.
        /// </summary>
        /// <param name="content">A <see cref="IHtmlContent"/> value that representing html string.</param>
        /// <returns>A <see cref="string"/> value.</returns>
        public static string GetString(this IHtmlContent content)
        {
            if (content == null)
                return null;

            using var writer = new Utf8StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        private static (TagHelperContext, TagHelperOutput) InitCore<THelper>(this THelper tagHelper, string unencodedContent = null) where THelper : TagHelper
        {
            var tagType = tagHelper.GetType();
            if (tagType == null)
                throw new NullReferenceException(nameof(tagType));

            var targetAttrs = tagType.GetCustomAttributes<HtmlTargetElementAttribute>().ToList();
            if (targetAttrs?.Any() != true)
                throw new NullReferenceException(nameof(targetAttrs));

            var tagName = targetAttrs[0].Tag;
            var attributes = tagHelper.GetType()
              .GetProperties(BindingFlags.Public | BindingFlags.Instance)
              .Select(x => new TagHelperAttribute(x.Name, x.GetValue(tagHelper)))
              .ToList();

            var context = new TagHelperContext(new TagHelperAttributeList(attributes), new Dictionary<object, object>(), Guid.NewGuid().ToString());
            var output = new TagHelperOutput(
              tagName,
              new TagHelperAttributeList(),
              (useCachedResult, encoder) =>
                Task.Factory.StartNew<TagHelperContent>(
                  () =>
                  {
                      var instance = new DefaultTagHelperContent();

                      if (!string.IsNullOrEmpty(unencodedContent))
                          instance.SetContent(unencodedContent);

                      return instance;
                  }));

            return new ValueTuple<TagHelperContext, TagHelperOutput>(context, output);
        }

        private static TagHelperContent RenderContent(this TagHelperOutput output)
        {
            var content = output.Content.GetContent();
            var postContent = output.PostContent.GetContent();
            var preContent = output.PreContent.GetContent();

            return null;
        }

        private static TagBuilder RenderCore(this TagHelperOutput output)
        {
            var createdTag = new TagBuilder(output.TagName);
            createdTag.MergeAttributes(output.Attributes.ToDictionary(c => c.Name, c => c.Value));
            return createdTag;
        }

        /// <summary>
        /// Replaces an html string contents with given dictionary of tags.
        /// </summary>
        /// <param name="htmlDecodedText">A <see cref="string"/> value that representing non-encoded html string.</param>
        /// <param name="tags">A <see cref="Dictionary{TKey,TValue}"/> that representing tags to be replaces with given <see cref="htmlDecodedText"/>.</param>
        /// <returns>A <see cref="HtmlString"/> object.</returns>
        public static HtmlString ReplaceHtmlByTagName(this string htmlDecodedText, Dictionary<string, Func<string, IHtmlContent>> tags)
        {
            if (string.IsNullOrEmpty(htmlDecodedText))
                return new HtmlString(null);

            if (tags?.Any() != true)
                return new HtmlString(htmlDecodedText);

            htmlDecodedText = htmlDecodedText
                .Replace("\r\n", "")
                .Replace("</ ", "</");

            foreach (var tagGroup in tags.Select(x => new { Id = x.Key, Tag = x.Value.GetTagBuilder() }))
            {
                try
                {
                    var tagDicName = tagGroup.Id;
                    var tag = tagGroup.Tag;

                    var temp = htmlDecodedText.TryReplaceCore((TagBuilder)tag, tagDicName, out var tempHtmlDecodedText);
                    if (!temp)
                        continue;

                    htmlDecodedText = tempHtmlDecodedText;
                }
                catch
                {
                }
            }

            return new HtmlString(htmlDecodedText);
        }

        private static bool TryReplaceCore(this string htmlDecodedText, TagBuilder tag, string tagName, out string editedHtmlText)
        {
            editedHtmlText = htmlDecodedText;
            var tagStart = $"<{tagName}>";
            var tagEnd = $"</{tagName}>";
            var attributes = string.Join(" ", tag.Attributes.Select(x => $"{x.Key}='{x.Value}'"));

            var innerHtmlStartIndex = htmlDecodedText.IndexOf(tagStart) + tagStart.Length;
            var innerHtmlEndIndex = htmlDecodedText.IndexOf(tagEnd, innerHtmlStartIndex);

            if (innerHtmlStartIndex == -1 || innerHtmlEndIndex == -1)
                return false;

            var innerHtml = htmlDecodedText[innerHtmlStartIndex..innerHtmlEndIndex];

            var stringBuilder = new StringBuilder(htmlDecodedText);
            var finalInnerHtml = tag.HasInnerHtml
                ? tag.InnerHtml.GetString().Trim()
                : innerHtml?.Trim();

            if (!string.IsNullOrEmpty(finalInnerHtml))
            {
                if (innerHtmlStartIndex >= 0 && !string.IsNullOrEmpty(innerHtml))
                    stringBuilder.Remove(innerHtmlStartIndex, innerHtml.Length);

                stringBuilder.Insert(innerHtmlStartIndex, finalInnerHtml);
                htmlDecodedText = stringBuilder.ToString();
            }

            htmlDecodedText = htmlDecodedText.Replace(tagStart, $"<{tag.TagName} {attributes}>");
            htmlDecodedText = htmlDecodedText.Replace(tagEnd, $"</{tag.TagName}>");
            editedHtmlText = htmlDecodedText;
            return true;
        }

        public static HtmlString ReplaceHtml(this string htmlDecodedText, params Func<string, IHtmlContent>[] tags)
        {
            if (string.IsNullOrEmpty(htmlDecodedText))
                return new HtmlString(null);

            if (tags?.Any() != true)
                return new HtmlString(htmlDecodedText);

            htmlDecodedText = htmlDecodedText
                .Replace("\r\n", "")
                .Replace("</ ", "</");

            for (var i = 0; i < tags.Length; i++)
            {
                var tag = tags[i].GetTagBuilder();

                var temp = htmlDecodedText.TryReplaceCore((TagBuilder)tag, i.ToString(), out var tempHtmlDecodedText);
                if (!temp)
                    continue;

                htmlDecodedText = tempHtmlDecodedText;
            }

            return new HtmlString(htmlDecodedText);
        }

        public static HtmlString Replace(this IHtmlHelper _, string htmlDecodedText, params Func<string, IHtmlContent>[] tags)
        {
            return ReplaceHtml(htmlDecodedText, tags);
        }

        public static HtmlString ReplaceByTagName(this IHtmlHelper _, string htmlDecodedText, Dictionary<string, Func<string, IHtmlContent>> tags)
        {
            return ReplaceHtmlByTagName(htmlDecodedText, tags);
        }

        internal static (TagHelperContext, TagHelperOutput) Init<THelper>(this THelper tagHelper, string unencodedContent = null) where THelper : TagHelper
        {
            var (context, output) = tagHelper.InitCore(unencodedContent);
            tagHelper.Process(context, output);
            return new ValueTuple<TagHelperContext, TagHelperOutput>(context, output);
        }

        internal static async Task<(TagHelperContext, TagHelperOutput)> InitAsync<THelper>(this THelper tagHelper, string unencodedContent = null) where THelper : TagHelper
        {
            var (context, output) = tagHelper.InitCore(unencodedContent);
            await tagHelper.ProcessAsync(context, output);
            return new ValueTuple<TagHelperContext, TagHelperOutput>(context, output);
        }

        public static TagBuilder GetTagBuilder<THelper>(this THelper tagHelper, string unencodedContent = null) where THelper : TagHelper
        {
            var output = tagHelper.GetOutput(unencodedContent);
            return output.RenderCore();
        }

        public static TagHelperOutput GetOutput<THelper>(this THelper tagHelper, string unencodedContent = null) where THelper : TagHelper
        {
            var (context, output) = tagHelper.Init(unencodedContent);
            return output;
        }

        public static async Task<TagHelperOutput> GetOutputAsync<THelper>(this THelper tagHelper, string unencodedContent = null) where THelper : TagHelper
        {
            var (context, output) = await tagHelper.InitAsync(unencodedContent).ConfigureAwait(false);
            return output;
        }

        public static async Task<TagBuilder> GetTagBuilderAsync<THelper>(this THelper tagHelper, string unencodedContent = null) where THelper : TagHelper
        {
            var output = await tagHelper.GetOutputAsync(unencodedContent).ConfigureAwait(false);
            return output.RenderCore();
        }
    }
}