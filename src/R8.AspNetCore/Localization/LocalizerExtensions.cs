using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

using R8.AspNetCore.HttpContextExtensions;
using R8.AspNetCore.TagBuilders;
using R8.Lib;
using R8.Lib.Localization;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace R8.AspNetCore.Localization
{
    public static class LocalizerExtensions
    {
        public static string GetGlobalizedUrl(this ILocalizer localizer, HttpContext httpContext, string url, CultureInfo culture, bool fullUrl = true)
        {
            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch
            {
                uri = new Uri(httpContext.GetBaseUrl() + url.Substring(1));
            }

            var finalUrl = uri.AbsolutePath;
            var urlCulture = string.Empty;
            foreach (var supportedCulture in localizer.SupportedCultures.Where(supportedCulture =>
                finalUrl.StartsWith($"/{supportedCulture.Name}/")))
            {
                urlCulture = supportedCulture.Name;
            }

            var sb = new StringBuilder(finalUrl);
            if (!string.IsNullOrEmpty(urlCulture))
                sb.Remove(0, finalUrl.FindByIndex("/", 2));

            if (culture.Name != localizer.DefaultCulture.Name)
                sb.Insert(0, $"/{culture.Name}");

            finalUrl = $"{sb}{uri.Query}";

            return fullUrl
                ? httpContext.GetBaseUrl() + finalUrl.Substring(1)
                : finalUrl;
        }

        public static CultureInfo GetCulture(this HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var features = context.Features ?? throw new ArgumentNullException(nameof(context.Features));
            var requestCulture = features.Get<IRequestCultureFeature>() ?? throw new ArgumentNullException("features.Get<IRequestCultureFeature>()");
            return requestCulture.RequestCulture.Culture;
        }

        /// <summary>
        /// Represents a culture translation in type of <see cref="HtmlString"/>.
        /// </summary>
        /// <param name="container">A <see cref="LocalizerContainer"/> object that representing translations for specific key.</param>
        /// <param name="culture">A <see cref="CultureInfo"/> object that representing specific culture.</param>
        /// <returns>An <see cref="HtmlString"/> object.</returns>
        /// <remarks>If <c>culture</c> be null then it mean <c>CultureInfo.CurrentCulture.</c></remarks>
        public static HtmlString GetHtmlString(this LocalizerContainer container, CultureInfo culture = null)
        {
            var text = container.Get(culture ?? CultureInfo.CurrentCulture, false);
            return new HtmlString(HttpUtility.HtmlDecode(text));
        }

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