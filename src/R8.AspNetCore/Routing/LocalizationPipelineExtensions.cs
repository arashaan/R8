using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;

using R8.AspNetCore.Localization;

using System;
using System.Globalization;
using System.Linq;

namespace R8.AspNetCore.Routing
{
    public static class LocalizationPipelineExtensions
    {
        /// <summary>
        /// Configures ' LocalizationPipeline ' class to be used in globalization. It's better to google about LocalizationPipeline.
        /// </summary>
        /// <param name="app">An <see cref="IApplicationBuilder"/> instance.</param>
        /// <param name="supportedCultures">An array that representing list of supported cultures in platform.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <remarks>Just declare a class as 'LocalizationPipeline' and put following code: <c>public void Configure(IApplicationBuilder app) => app.ConfigureLocalizationPipeline();</c></remarks>
        public static void ConfigureLocalizationPipeline(this IApplicationBuilder app, params string[] supportedCultures)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (supportedCultures == null)
                throw new ArgumentNullException(nameof(supportedCultures));

            var options = new RequestLocalizationOptions()
                .ConfigureRequestLocalization(supportedCultures);

            options.RequestCultureProviders = new IRequestCultureProvider[] { options.GetProvider() };
            app.UseRequestLocalization(options);
        }

        /// <summary>
        /// Returns a new <see cref="RouteDataRequestCultureProvider"/> instance by given options.
        /// </summary>
        /// <param name="options">An <see cref="RequestLocalizationOptions"/> instance that includes localization options.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="RouteDataRequestCultureProvider"/> instance.</returns>
        public static RouteDataRequestCultureProvider GetProvider(this RequestLocalizationOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return new RouteDataRequestCultureProvider
            {
                RouteDataStringKey = Constraints.LanguageKey,
                UIRouteDataStringKey = Constraints.LanguageKey,
                Options = options
            };
        }

        /// <summary>
        /// Returns an configured instance of <see cref="RequestLocalizationOptions"/>.
        /// </summary>
        /// <param name="options">An <see cref="RequestLocalizationOptions"/> instance that includes localization options.</param>
        /// <param name="supportedCultures">An array that representing list of supported cultures in platform.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="RequestLocalizationOptions"/> instance.</returns>
        public static RequestLocalizationOptions ConfigureRequestLocalization(this RequestLocalizationOptions options, params string[] supportedCultures)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (supportedCultures == null)
                throw new ArgumentNullException(nameof(supportedCultures));

            var cultures = supportedCultures
                .Select(culture => new CultureInfo(culture))
                .ToList();

            var defaultCulture = supportedCultures[0];
            options.DefaultRequestCulture = new RequestCulture(defaultCulture, defaultCulture);
            options.SupportedCultures = cultures;
            options.SupportedUICultures = cultures;
            return options;
        }
    }
}