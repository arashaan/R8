using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System;
using System.Linq;
using R8.Lib.Localization;

namespace R8.Lib.AspNetCore.Localizer
{
    public static class LocalizerConfigure
    {
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

                var instance = new Localization.Localizer(configuration)
                {
                    SupportedCultures = requestLocalization.Value.SupportedCultures.ToList(),
                    DefaultCulture = requestLocalization.Value.DefaultRequestCulture.Culture
                };
                return instance;
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