using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace R8.AspNetCore.Localization
{
    public static class Extensions
    {
        public static CultureInfo GetCulture(this HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var features = context.Features ?? throw new ArgumentNullException("context.Features");
            var requestCulture = features.Get<IRequestCultureFeature>() ?? throw new ArgumentNullException("features.Get<IRequestCultureFeature>()");
            return requestCulture.RequestCulture.Culture;
        }

        public static string GetCultureName(this HttpContext context)
        {
            return context.GetCulture().Name;
        }

        /// <summary>
        /// Returns base url in Development
        /// </summary>
        /// <param name="context"></param>
        public static string GetTwoLetterCulture(this HttpContext context)
        {
            return context.GetCulture().TwoLetterISOLanguageName;
        }

        /// <summary>
        /// Returns base url in Development
        /// </summary>
        /// <param name="context"></param>
        public static bool IsCultureRightToLeft(this HttpContext context)
        {
            return context.GetCulture().TextInfo.IsRightToLeft;
        }
    }
}