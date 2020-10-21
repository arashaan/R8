using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

using System.Globalization;

namespace R8.Lib.AspNetCore.Globalization
{
    public static class Extensions
    {
        public static CultureInfo GetCulture(this HttpContext context)
        {
            var requestCulture = context.Features.Get<IRequestCultureFeature>();
            var culture = requestCulture.RequestCulture.Culture;
            var result = culture;
            return result;
        }

        public static string GetCultureName(this HttpContext context)
        {
            var culture = context.GetCulture();
            var result = culture.Name;
            return result;
        }

        /// <summary>
        /// Returns base url in Development
        /// </summary>
        /// <param name="context"></param>
        public static string GetTwoLetterCulture(this HttpContext context)
        {
            var culture = context.GetCulture();
            var iso = culture.TwoLetterISOLanguageName;
            return iso;
        }

        /// <summary>
        /// Returns base url in Development
        /// </summary>
        /// <param name="context"></param>
        public static bool IsCultureRightToLeft(this HttpContext context)
        {
            var culture = context.GetCulture();
            var rtl = culture.TextInfo.IsRightToLeft;
            return rtl;
        }
    }
}