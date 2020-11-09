using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using R8.Lib.FileHandlers;

using System;

namespace R8.Lib.AspNetCore.FileHandlers
{
    public static class AddFileHandlerExtensions
    {
        /// <summary>
        /// Registers <see cref="MyFileConfiguration"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns></returns>
        public static IServiceCollection AddFileHandlers(this IServiceCollection services)
        {
            services.AddTransient(serviceProvider =>
            {
                using var scope = serviceProvider.CreateScope();

                var hostEnvironment = scope.ServiceProvider.GetService<IWebHostEnvironment>();
                if (hostEnvironment == null)
                    throw new NullReferenceException(
                        $"Can't find {nameof(IWebHostEnvironment)} related service");

                var instance = new MyFileConfiguration();
                instance.AddService(hostEnvironment);
                return instance;
            });
            return services;
        }
    }
}