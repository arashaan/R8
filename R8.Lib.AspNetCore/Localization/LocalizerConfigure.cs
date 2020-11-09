using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using R8.Lib.Localization;

using System;
using System.Linq;

namespace R8.Lib.AspNetCore.Localization
{
    public static class LocalizerConfigure
    {
        /// <summary>
        /// Registers <see cref="ILocalizer"/> as a service.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddLocalizer(this IServiceCollection services, Action<LocalizerConfiguration> config)
        {
            services.AddSingleton<ILocalizer>(serviceProvider =>
            {
                using var scope = serviceProvider.CreateScope();

                var requestLocalization = scope.ServiceProvider.GetService<IOptions<RequestLocalizationOptions>>();
                if (requestLocalization == null)
                    throw new NullReferenceException($"Can't find {nameof(RequestLocalizationOptions)} related service");

                var configuration = new LocalizerConfiguration();
                config(configuration);

                return new Localizer(configuration)
                {
                    SupportedCultures = requestLocalization.Value.SupportedCultures.ToList(),
                    DefaultCulture = requestLocalization.Value.DefaultRequestCulture.Culture
                };
            });
            return services;
        }

        public static IApplicationBuilder UseLocalizer(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var serviceProvider = context.RequestServices;
                var localizer = serviceProvider.GetService<ILocalizer>();

                await localizer.RefreshAsync();

                await next();
            });

            return app;
        }
    }
}