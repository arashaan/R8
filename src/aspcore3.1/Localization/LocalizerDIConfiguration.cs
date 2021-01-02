using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;

using R8.Lib.Localization;
using R8.Lib.MethodReturn;

namespace R8.AspNetCore.Localization
{
    public static class LocalizerDiConfiguration
    {
        public static async Task InitializeLocalizerAsync(this ILocalizer localizer)
        {
            var provider = localizer.Configuration.Provider;
            if (provider is LocalizerCustomProvider customProvider)
            {
                if (customProvider.DictionaryAsync != null)
                {
                    await localizer.RefreshAsync();
                }
                else
                {
                    if (customProvider.Dictionary != null)
                    {
                        localizer.Refresh();
                    }
                    else
                    {
                        throw new NullReferenceException($"Either {nameof(LocalizerCustomProvider.Dictionary)} or {nameof(LocalizerCustomProvider.DictionaryAsync)} must be filled.");
                    }
                }
            }
            else
            {
                await localizer.RefreshAsync();
            }
        }

        public static void InitializeLocalizer(this ILocalizer localizer)
        {
            var provider = localizer.Configuration.Provider;
            if (provider is LocalizerCustomProvider customProvider)
            {
                if (customProvider.DictionaryAsync != null)
                {
                    localizer.RefreshAsync().GetAwaiter().GetResult();
                }
                else
                {
                    if (customProvider.Dictionary != null)
                    {
                        localizer.Refresh();
                    }
                    else
                    {
                        throw new NullReferenceException($"Either {nameof(LocalizerCustomProvider.Dictionary)} or {nameof(LocalizerCustomProvider.DictionaryAsync)} must be filled.");
                    }
                }
            }
            else
            {
                localizer.Refresh();
            }
        }

        public static void UseResponse(this IServiceProvider serviceProvider)
        {
            ResponseConnection.Services = serviceProvider;
        }

        public static IApplicationBuilder UseResponse(this IApplicationBuilder app)
        {
            ResponseConnection.Services = app.ApplicationServices;
            return app;
        }

        public static IApplicationBuilder UseLocalizer(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (!(context.RequestServices.GetService(typeof(ILocalizer)) is ILocalizer localizer))
                    throw new NullReferenceException($"{nameof(ILocalizer)} must be registered as a service.");

                await localizer.InitializeLocalizerAsync();
                await next();
            });

            return app;
        }
    }
}