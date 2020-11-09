using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Html;
using R8.Lib.AspNetCore.TagBuilders;
using R8.Lib.Localization;

namespace R8.Lib.AspNetCore
{
    public static class LocalizerExtensions
    {
        public static IHtmlContent Format(this ILocalizer localizer, string key, params Func<string, IHtmlContent>[] tags)
        {
            if (localizer == null)
                throw new ArgumentNullException(nameof(localizer));
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var localized = localizer[key].ToString();
            var final = localizer.Format<Func<string, IHtmlContent>>(key, tags);
            return final;
        }

        public static IHtmlContent Format<T>(this ILocalizer localizer, string key, params T[] args)
        {
            if (localizer == null)
                throw new ArgumentNullException(nameof(localizer));
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var tagBuilders = new List<string>();
            foreach (var arg in args)
            {
                if (arg is Func<string, IHtmlContent> tag)
                {
                    var htmlTag = tag.GetTagBuilder().GetString();
                    tagBuilders.Add(htmlTag);
                }
                else
                {
                    tagBuilders.Add(arg.ToString());
                }
            }

            var localized = localizer[key];
            var formattedString = string.Format(localized, tagBuilders.ToArray());
            var final = new HtmlString(formattedString);
            return final;
        }

        public static IHtmlContent Html(this ILocalizer localizer, string key, params Func<string, IHtmlContent>[] tags)
        {
            if (localizer == null)
                throw new ArgumentNullException(nameof(localizer));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (tags == null || !tags.Any())
                throw new ArgumentNullException(nameof(tags));

            var localized = localizer[key, CultureInfo.CurrentCulture];
            var html = localized.ReplaceHtml(tags);
            return html;
        }
    }
}