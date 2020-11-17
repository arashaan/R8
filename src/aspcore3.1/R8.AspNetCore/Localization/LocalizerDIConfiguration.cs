using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using R8.Lib.Localization;

using System;

namespace R8.AspNetCore.Localization
{
    public static class LocalizerDiConfiguration
    {
        //public static IApplicationBuilder UseLocalizer(this IApplicationBuilder app, Func<IServiceProvider, ILocalizerProvider> provider)
        //{
        //    app.Use(async (context, next) =>
        //    {
        //        var serviceProvider = context.RequestServices;
        //        var localizer = serviceProvider.GetService<ILocalizer>();
        //        if (localizer == null)
        //            throw new NullReferenceException($"{nameof(ILocalizer)} must be registered as a service.");

        //        if (localizer.GetProvider() is LocalizerJsonProvider)
        //            throw new ArgumentException($"Implementing {nameof(ILocalizerProvider)} for JSON and CUSTOM is different. Implementing configuration for JSON Provider {nameof(LocalizerConfigure.AddLocalizer)} shou");
        //        var invokedProvider = provider.Invoke(serviceProvider);
        //        await localizer.StartAsync(invokedProvider);
        //        await next();
        //    });

        //    return app;
        //}

        public static IApplicationBuilder UseLocalizer(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var serviceProvider = context.RequestServices;
                var localizer = serviceProvider.GetService<ILocalizer>();
                if (localizer == null)
                    throw new NullReferenceException($"{nameof(ILocalizer)} must be registered as a service.");

                await localizer.RefreshAsync();
                await next();
            });

            return app;
        }
    }
}