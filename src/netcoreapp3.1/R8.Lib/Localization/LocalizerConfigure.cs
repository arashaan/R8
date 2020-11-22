using Microsoft.Extensions.DependencyInjection;

using R8.Lib.MethodReturn;

using System;

namespace R8.Lib.Localization
{
    public static class LocalizerConfigure
    {
        /// <summary>
        /// Registers <see cref="ILocalizer"/> as a service.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddLocalizer<TProvider>(this IServiceCollection services, Func<IServiceProvider, TProvider> options) where TProvider : ILocalizerProvider
        {
            services.AddSingleton<ILocalizer>(serviceProvider =>
            {
                var configuration = options.Invoke(serviceProvider);
                var instance = new Localizer(configuration);
                instance.SetServiceProvider(serviceProvider);
                return instance;
            });

            services.AddTransient<IResponseStatus>(serviceProvider =>
            {
                using var scope = serviceProvider.CreateScope();

                var localizer = scope.ServiceProvider.GetService<ILocalizer>();
                if (localizer == null)
                    throw new NullReferenceException($"Can't find {nameof(ILocalizer)} related service");

                var obj = new ResponseStatus();
                obj.SetLocalizer(localizer);
                return obj;
            });
            return services;
        }
    }
}