using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

namespace R8.AspNetCore.WebTest.Services.Globalization
{
    public static class Cultures
    {
        public static readonly string[] Supported = { "tr", "en", "fa" };
        public static readonly string Default = Supported[0];

        public static RequestLocalizationOptions ConfigureRequestLocalization(this RequestLocalizationOptions options)
        {
            options ??= new RequestLocalizationOptions();
            var supportedCultures = Supported
                 .Select(culture => new CultureInfo(culture))
                 .ToList();

            options.DefaultRequestCulture = new RequestCulture(Default, Default);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            return options;
        }
    }
}