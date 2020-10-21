using Microsoft.AspNetCore.Http;

using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace R8.Lib.AspNetCore.Base
{
    public static class Extensions
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
                sb.Remove(0, finalUrl.IndexOfNth("/", 2));

            if (culture.Name != localizer.DefaultCulture.Name)
                sb.Insert(0, $"/{culture.Name}");

            finalUrl = $"{sb}{uri.Query}";

            return fullUrl
                ? httpContext.GetBaseUrl() + finalUrl.Substring(1)
                : finalUrl;
        }
    }
}