using Microsoft.AspNetCore.Html;

using R8.Lib.Localization;

using System;
using System.Linq;
using System.Linq.Expressions;

namespace R8.Lib.AspNetCore.TagHelpers
{
    public static class ILocalizerExtensions
    {
        public static IHtmlContent Html(this ILocalizer localizer, Expression<Func<string>> key, params Func<string, IHtmlContent>[] tags)
        {
            var result = localizer.Html(key, true, tags);
            return result;
        }

        public static IHtmlContent Format<T>(this ILocalizer localizer, Expression<Func<string>> key, params T[] args)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var myKey = Localizer.GetKey(key);
            var localized = localizer[myKey].ToString();
            var newArgs = args.Select(x => x.ToString()).ToArray();

            var final = new HtmlString(string.Format(localized, newArgs));
            return final;
        }

        public static IHtmlContent Format(this ILocalizer localizer, Expression<Func<string>> key, params Func<string, IHtmlContent>[] tags)
        {
            if (tags == null)
                throw new ArgumentNullException(nameof(tags));

            var tagBuilders = tags?.Select(x => x.GetTagBuilder().GetString()).ToArray();
            var result = localizer.Format(key, tagBuilders);
            return result;
        }

        public static IHtmlContent Html(this ILocalizer localizer, Expression<Func<string>> key, bool normalize, params Func<string, IHtmlContent>[] tags)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (tags == null)
                throw new ArgumentNullException(nameof(tags));

            var myKey = Localizer.GetKey(key);
            var localized = localizer[myKey].ToString();
            var html = localized.ReplaceHtml(tags);
            return html;
        }
    }
}