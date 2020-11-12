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
        public static IServiceCollection AddLocalizer(this IServiceCollection services, Func<IServiceProvider, LocalizerConfiguration> options)
        {
            services.AddSingleton<ILocalizer>(serviceProvider =>
            {
                var configuration = options.Invoke(serviceProvider);
                return new Localizer(configuration);
            });

            services.AddTransient<IResponseBase>(serviceProvider =>
            {
                using var scope = serviceProvider.CreateScope();

                var localizer = scope.ServiceProvider.GetService<ILocalizer>();
                if (localizer == null)
                    throw new NullReferenceException($"Can't find {nameof(ILocalizer)} related service");

                var obj = new ResponseBase();
                obj.SetLocalizer(localizer);
                return obj;
            });
            return services;
        }
    }
}