using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using R8.AspNetCore.Routing;
using R8.Lib.Localization;

namespace R8.AspNetCore.TagBuilders
{
    [HtmlTargetElement("status", TagStructure = TagStructure.WithoutEndTag)]
    public class StatusTagHelper : TagHelper
    {
        private readonly ILocalizer _localizer;

        public StatusTagHelper(ILocalizer localizer)
        {
            _localizer = localizer;
        }

        [ViewContext, HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!(ViewContext.ViewData.Model is PageModel page))
            {
                output.SuppressOutput();
                return;
            }

            if (page.Status == null)
            {
                output.SuppressOutput();
                return;
            }

            var flag = page.Status.Value;
            var status = _localizer[flag.ToString()];
            var message = WebUtility.UrlDecode(page.Message);
            // if (string.IsNullOrEmpty(content))
            // {
            //   output.SuppressOutput();
            //   return;
            // }

            var span = new TagBuilder("span");
            span.InnerHtml.AppendHtml(status);

            if (!string.IsNullOrEmpty(message))
            {
                var exceptions = message.Split(ValidatableResultCollectionExtensions.ExceptionDivider).ToList();
                if (exceptions?.Any() == true)
                {
                    foreach (var exception in exceptions)
                    {
                        var exceptionTag = new TagBuilder("span");

                        var arr = exception.Split(ValidatableResultCollectionExtensions.ExceptionKeyValueDivider);
                        if (arr == null || arr.Length != 2)
                            continue;

                        var exKey = WebUtility.UrlDecode(arr[0]);

                        var badgeTag = new TagBuilder("span");
                        badgeTag.AddCssClass("badge");
                        badgeTag.AddCssClass("badge-dark");
                        badgeTag.InnerHtml.Append(exKey);
                        exceptionTag.InnerHtml.AppendHtml(badgeTag);

                        var exValue = arr[1].Split(ValidatableResultCollectionExtensions.ExceptionErrorsDivider);
                        var exValues = string.Join(" — ", exValue);
                        exceptionTag.InnerHtml.AppendHtml(" ").AppendHtml(exValues);

                        span.InnerHtml.AppendHtml(exceptionTag);
                    }
                }
            }

            var closeButton = new TagBuilder("a");
            closeButton.Attributes.Add("href", "#");
            closeButton.Attributes.Add("onclick", "$('#pageSTATUS').remove()");
            closeButton.AddCssClass("close");
            // closeButton.Attributes.Add("style", "float: left");
            closeButton.InnerHtml.AppendHtml("&times;");

            output.AddClass("status-message", HtmlEncoder.Default);
            // output.AddClass("text-right", HtmlEncoder.Default);
            output.Attributes.Add("id", "pageSTATUS");
            // if (string.IsNullOrEmpty(message))
            // output.AddClass("hidden", HtmlEncoder.Default);
            // else
            if (!string.IsNullOrEmpty(status))
                output.Content.AppendHtml(span).AppendHtml(closeButton);

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";
        }
    }
}