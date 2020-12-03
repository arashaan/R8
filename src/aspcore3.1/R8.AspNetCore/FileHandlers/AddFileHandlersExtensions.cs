using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace R8.AspNetCore.FileHandlers
{
    public static class AddFileHandlersExtensions
    {
        public static IServiceCollection AddFileHandlers(this IServiceCollection services, Action<IWebHostEnvironment, FileHandlerConfiguration> config)
        {
            services.AddSingleton(serviceProvider =>
            {
                using var scope = serviceProvider.CreateScope();

                var hostEnvironment = scope.ServiceProvider.GetService<IWebHostEnvironment>();
                var configuration = new FileHandlerConfiguration(hostEnvironment);
                config.Invoke(hostEnvironment, configuration);
                return configuration;
            });
            return services;
        }

        public static IApplicationBuilder UseFileHandlers(this IApplicationBuilder app)
        {
            FileHandlersConnection.Services = app.ApplicationServices;
            return app;
        }
    }
}