using System;

using Microsoft.AspNetCore.Builder;

using R8.Lib.Localization;

namespace R8.AspNetCore.Localization
{
    public static class LocalizerDiConfiguration
    {
        public static IApplicationBuilder UseLocalizer(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (!(context.RequestServices.GetService(typeof(ILocalizer)) is ILocalizer localizer))
                    throw new NullReferenceException($"{nameof(ILocalizer)} must be registered as a service.");

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

                await next();
            });

            return app;
        }
    }
}