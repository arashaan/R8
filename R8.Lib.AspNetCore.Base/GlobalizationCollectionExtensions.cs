using Microsoft.AspNetCore.Html;

using R8.Lib.Localization;

using System.Globalization;
using System.Web;

namespace R8.Lib.AspNetCore.Base
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