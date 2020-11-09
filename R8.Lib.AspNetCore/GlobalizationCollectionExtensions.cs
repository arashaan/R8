using System.Globalization;
using System.Web;
using Microsoft.AspNetCore.Html;
using R8.Lib.Localization;

namespace R8.Lib.AspNetCore
{
    public static class GlobalizationCollectionExtensions
    {
        public static HtmlString GetHtmlString(this LocalizerContainer collection, CultureInfo culture = null)
        {
            var text = collection.Get(culture ?? CultureInfo.CurrentCulture, false);
            var html = new HtmlString(HttpUtility.HtmlDecode(text));
            return html;
        }
    }
}