using Microsoft.AspNetCore.Html;

using System.Globalization;
using System.Web;

namespace R8.Lib.AspNetCore.Base
{
    public static class GlobalizationCollectionExtensions
    {
        public static HtmlString GetHtmlString(this GlobalizationCollectionJson collection, CultureInfo culture = null)
        {
            var text = collection.GetLocale(culture ?? CultureInfo.CurrentCulture, false);
            var html = new HtmlString(HttpUtility.HtmlDecode(text));
            return html;
        }
    }
}