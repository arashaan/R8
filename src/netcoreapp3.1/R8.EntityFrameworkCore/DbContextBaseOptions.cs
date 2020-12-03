using System;

using EasyCaching.InMemory;

using EFCoreSecondLevelCacheInterceptor;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace R8.EntityFrameworkCore
{
    public static class DbContextBaseOptions
    {
        /// <summary>
        /// Adds some predefined registration for specific <see cref="DbContext"/>.
        /// </summary>
        public static void AddCustomPooledDbContextFactory<TContext>(this IServiceCollection services, Action<DbContextBaseConfiguration> config) where TContext : DbContext
        {
            var configuration = new DbContextBaseConfiguration();
            config?.Invoke(configuration);

            if (string.IsNullOrEmpty(configuration.ConnectionString))
                throw new NullReferenceException(nameof(configuration.ConnectionString));

            services.AddEntityFrameworkSqlServer();
            var providerName = typeof(TContext).Name;
            services.AddEFSecondLevelCache(options =>
                options.UseEasyCachingCoreProvider(providerName, isHybridCache: false).DisableLogging(true));

            services.AddEasyCaching(options =>
            {
                options.UseInMemory(memoryOptions =>
                {
                    memoryOptions.DBConfig = new InMemoryCachingOptions
                    {
                        ExpirationScanFrequency = 60,
                        SizeLimit = 100,
                        EnableReadDeepClone = false,
                        EnableWriteDeepClone = false,
                    };
                    memoryOptions.MaxRdSecond = 120;
                    memoryOptions.EnableLogging = false;
                    memoryOptions.LockMs = 5000;
                    memoryOptions.SleepMs = 300;
                }, providerName);
            });

            services.AddPooledDbContextFactory<TContext>((serviceProvider, builder) =>
            {
                builder.UseSqlServer(
                    configuration.ConnectionString,
                    optionsBuilder =>
                    {
                        optionsBuilder.CommandTimeout((int)configuration.CommandTimeout.TotalSeconds);
                        optionsBuilder.EnableRetryOnFailure();

                        if (!string.IsNullOrEmpty(configuration.MigrationAssembly))
                            optionsBuilder.MigrationsAssembly(configuration.MigrationAssembly);

                        configuration.Action?.Invoke(optionsBuilder);
                    });

                builder.EnableDetailedErrors();
                builder.EnableSensitiveDataLogging(); // Often also useful with EnableDetailedErrors
                builder.AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());
                builder.UseInternalServiceProvider(serviceProvider);
            });
        }
    }
}