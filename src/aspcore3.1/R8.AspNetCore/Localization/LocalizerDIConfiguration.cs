using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using R8.Lib.Localization;

using System;

namespace R8.AspNetCore.Localization
{
    public static class LocalizerDiConfiguration
    {
        public static IApplicationBuilder UseLocalizer(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var serviceProvider = context.RequestServices;
                var localizer = serviceProvider.GetService<ILocalizer>();
                if (localizer == null)
                    throw new NullReferenceException($"{nameof(ILocalizer)} must be registered as a service.");

                await localizer.InitializeAsync();
                await next();
            });

            return app;
        }
    }
}