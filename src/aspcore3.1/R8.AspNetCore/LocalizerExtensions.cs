using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Microsoft.AspNetCore.Html;

using R8.AspNetCore.TagBuilders;
using R8.Lib.Localization;

namespace R8.AspNetCore
{
    public static class LocalizerExtensions
    {
        /// <summary>
        /// Returns a <see cref="IHtmlContent"/> object that representing formatted input.
        /// </summary>
        /// <param name="localizer">An instance of <see cref="ILocalizer"/>.</param>
        /// <param name="key">Name of specific key in <see cref="ILocalizer.Dictionary"/> that containing a localized text.</param>
        /// <param name="tags">A collection of html tags that should be replaced in given text.</param>
        /// <returns>A <see cref="IHtmlContent"/> object that representing a formatted text with given tags.</returns>
        public static IHtmlContent Format(this ILocalizer localizer, string key, params Func<string, IHtmlContent>[] tags)
        {
            if (localizer == null)
                throw new ArgumentNullException(nameof(localizer));
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return localizer.Format<Func<string, IHtmlContent>>(key, tags);
        }

        /// <summary>
        /// Returns a <see cref="IHtmlContent"/> object that representing formatted input.
        /// </summary>
        /// <param name="localizer">An instance of <see cref="ILocalizer"/>.</param>
        /// <param name="key">Name of specific key in <see cref="ILocalizer.Dictionary"/> that containing a localized text.</param>
        /// <param name="args">A collection of arguments that should be replaced in given text.</param>
        /// <returns>A <see cref="IHtmlContent"/> object that representing a formatted text with given tags.</returns>
        public static IHtmlContent Format<T>(this ILocalizer localizer, string key, params T[] args)
        {
            if (localizer == null)
                throw new ArgumentNullException(nameof(localizer));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (args == null || !args.Any())
                throw new ArgumentNullException(nameof(key));

            var tagBuilders = new List<string>();
            foreach (var arg in args)
            {
                if (arg is Func<string, IHtmlContent> tag)
                {
                    var htmlTag = tag.Invoke("").GetString();
                    tagBuilders.Add(htmlTag);
                }
                else
                {
                    tagBuilders.Add(arg.ToString());
                }
            }

            var localized = localizer[key];

            if (tagBuilders.Count == 0)
                return new HtmlString(localized);

            var formattedString = string.Format(localized, tagBuilders.ToArray());
            return new HtmlString(formattedString);
        }

        /// <summary>
        /// Returns a <see cref="IHtmlContent"/> object that representing replaced input tags with given tags.
        /// </summary>
        /// <param name="localizer">An instance of <see cref="ILocalizer"/>.</param>
        /// <param name="key">Name of specific key in <see cref="ILocalizer.Dictionary"/> that containing a localized text.</param>
        /// <param name="tags">A collection of html tags that should be replaced in given html string.</param>
        /// <returns>A <see cref="IHtmlContent"/> object that representing a formatted html with given tags.</returns>
        public static IHtmlContent Html(this ILocalizer localizer, string key, params Func<string, IHtmlContent>[] tags)
        {
            if (localizer == null)
                throw new ArgumentNullException(nameof(localizer));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (tags?.Any() != true)
                throw new ArgumentNullException(nameof(tags));

            var localized = localizer[key, CultureInfo.CurrentCulture];
            return localized.ReplaceHtml(tags);
        }
    }
}