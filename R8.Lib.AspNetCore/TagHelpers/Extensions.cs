using HtmlAgilityPack;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace R8.Lib.AspNetCore.TagHelpers
{
    public static class Extensions
    {
        public static ModelExpression GetPropertyModelExpression(this ModelExpression model, string propertyName)
        {
            return new ModelExpression($"{model.Name}.{propertyName}", model.ModelExplorer.GetExplorerForProperty(propertyName));
        }

        public static string GetUiName(this ModelExpression model, JsonValidatable jsonModel)
        {
            var text = $"{model.Name}.{jsonModel.Name.ToUnescaped(true)}";
            if (!string.IsNullOrEmpty(jsonModel.Id))
                text += $"${jsonModel.Id}";

            return text;
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

        public static TagBuilder GetTagBuilder(this IHtmlContent tag)
        {
            var contentToStr = tag.GetString();
            var decoded = HttpUtility.HtmlDecode(contentToStr);
            var result = GetTagBuilder(decoded);
            return result;
        }

        public static TagBuilder GetTagBuilder(this Func<string, IHtmlContent> tag)
        {
            var contentToStr = tag.Invoke("").GetString();
            var decoded = HttpUtility.HtmlDecode(contentToStr);
            var result = GetTagBuilder(decoded);
            return result;
        }

        public static TagBuilder GetTagBuilder(string html)
        {
            var node = HtmlNode.CreateNode(html);
            var tagBuilder = new TagBuilder(node.Name);
            tagBuilder.MergeAttributes(node.Attributes.ToDictionary(x => x.Name, x => x.Value));
            tagBuilder.InnerHtml.SetHtmlContent(node.InnerHtml);
            return tagBuilder;
        }

        public static string GetString(this IHtmlContent content)
        {
            using var writer = new Utf8StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        private static (TagHelperContext, TagHelperOutput) InitCore<THelper>(this THelper tagHelper) where THelper : TagHelper
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
                  () => new DefaultTagHelperContent()
                ));

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

            // var renderedContent = output.RenderContent();
            // if (renderedContent == null)
            //     throw new NullReferenceException(nameof(renderedContent));
            //
            // var content = renderedContent.GetContent();
            // if (!string.IsNullOrEmpty(content))
            //     createdTag.InnerHtml.SetContent(content);

            return createdTag;
        }

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

                    var temp = htmlDecodedText.TryReplaceCore(tag, tagDicName, out var tempHtmlDecodedText);
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

                var temp = htmlDecodedText.TryReplaceCore(tag, i.ToString(), out var tempHtmlDecodedText);
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

        public static (TagHelperContext, TagHelperOutput) Init<THelper>(this THelper tagHelper) where THelper : TagHelper
        {
            var (context, output) = tagHelper.InitCore();
            tagHelper.Process(context, output);
            return new ValueTuple<TagHelperContext, TagHelperOutput>(context, output);
        }

        public static async Task<(TagHelperContext, TagHelperOutput)> InitAsync<THelper>(this THelper tagHelper) where THelper : TagHelper
        {
            var (context, output) = tagHelper.InitCore();
            await tagHelper.ProcessAsync(context, output);
            return new ValueTuple<TagHelperContext, TagHelperOutput>(context, output);
        }

        public static TagBuilder Render<THelper>(this THelper tagHelper) where THelper : TagHelper
        {
            var (context, output) = tagHelper.Init();
            return output.RenderCore();
        }

        public static async Task<TagBuilder> RenderAsync<THelper>(this THelper tagHelper) where THelper : TagHelper
        {
            var (context, output) = await tagHelper.InitAsync().ConfigureAwait(false);
            return output.RenderCore();
        }
    }
}