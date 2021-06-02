using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using System;

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
            services.AddSingleton<ILocalizer>(serviceProvider =>
            {
                var configuration = new LocalizerConfiguration();
                config(serviceProvider, configuration);

                Localizer instance;
                if (configuration.UseMemoryCache)
                {
                    var memoryCache = serviceProvider.GetService<IMemoryCache>();
                    instance = new Localizer(configuration, memoryCache);
                }
                else
                {
                    instance = new Localizer(configuration, null);
                }
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
            //     var obj = new WrapperStatus();
            //     obj.SetLocalizer(localizer);
            //     return obj;
            // });
            return services;
        }
    }
}