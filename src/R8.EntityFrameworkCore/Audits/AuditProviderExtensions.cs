using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace R8.EntityFrameworkCore.Audits
{
    public static class AuditProviderExtensions
    {
        /// <summary>
        /// Adds Audit tracking support to <see cref="EntityFrameworkCore"/>.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder AddEFAuditProviderInterceptor(this DbContextOptionsBuilder optionsBuilder, IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<AuditProviderConfiguration>();
            if (config == null)
                throw new NullReferenceException(
                    $"Try to register a provider configuration by using .{nameof(AddEFAuditProvider)}()");

            var provider = serviceProvider.GetRequiredService<IAuditDataProvider>();
            optionsBuilder.AddInterceptors(new AuditProviderInterceptor(provider, config));
            return optionsBuilder;
        }

        /// <summary>
        /// Adds dependency injection to support audit tracking of entities those implemented from <see cref="IEntityBaseAudit"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddEFAuditProvider(this IServiceCollection services, Action<AuditProviderConfiguration> config)
        {
            var configuration = new AuditProviderConfiguration();
            config(configuration);

            services.AddSingleton(configuration);
            services.AddSingleton(typeof(IAuditDataProvider), configuration.ProviderType);
            return services;
        }
    }
}