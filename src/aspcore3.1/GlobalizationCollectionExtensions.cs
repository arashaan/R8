using Microsoft.AspNetCore.Html;

using R8.Lib.Localization;

using System.Globalization;
using System.Web;

namespace R8.AspNetCore
{
    public static class GlobalizationCollectionExtensions
    {
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
    }
}