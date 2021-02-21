using System;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using R8.Lib.MethodReturn;

namespace R8.Lib.Localization
{
    public static class LocalizerConfigure
    {
        /// <summary>
        /// Registers <see cref="ILocalizer"/> as a service.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddLocalizer(this IServiceCollection services, Action<IServiceProvider, LocalizerConfiguration> config)
        {
            // services.AddMemoryCache();
            services.AddSingleton<ILocalizer>(serviceProvider =>
            {
                var memoryCache = serviceProvider.GetService<IMemoryCache>();
                var configuration = new LocalizerConfiguration();
                config(serviceProvider, configuration);
                var instance = new Localizer(configuration, memoryCache);
                return instance;
            });

            // services.AddTransient<IResponseStatus>(serviceProvider =>
            // {
            //     using var scope = serviceProvider.CreateScope();
            //
            //     var localizer = scope.ServiceProvider.GetService<ILocalizer>();
            //     if (localizer == null)
            //         throw new NullReferenceException($"Can't find {nameof(ILocalizer)} related service");
            //
            //     var obj = new ResponseStatus();
            //     obj.SetLocalizer(localizer);
            //     return obj;
            // });
            return services;
        }
    }
}