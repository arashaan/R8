using EasyCaching.InMemory;

using EFCoreSecondLevelCacheInterceptor;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;

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

                // builder.ConfigureWarnings(warnings => warnings.Log(CoreEventId.SaveChangesCompleted));
                builder.EnableDetailedErrors();
                builder.EnableSensitiveDataLogging(); // Often also useful with EnableDetailedErrors
                builder.AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());
                builder.UseInternalServiceProvider(serviceProvider);
            });
        }

        ///// <summary>
        ///// Adds some predefined caching registration and configuration for specific <see cref="DbContext"/>.
        ///// </summary>
        //public static void AddDbContextCaching<TContext>(this IServiceCollection services) where TContext : DbContext
        //{
        //    var providerName = typeof(TContext).Name;
        //    services.AddEFSecondLevelCache(options =>
        //        options.UseEasyCachingCoreProvider(providerName, isHybridCache: false).DisableLogging(true));

        //    services.AddEasyCaching(options =>
        //    {
        //        options.UseInMemory(config =>
        //        {
        //            config.DBConfig = new InMemoryCachingOptions
        //            {
        //                ExpirationScanFrequency = 60,
        //                SizeLimit = 100,
        //                EnableReadDeepClone = false,
        //                EnableWriteDeepClone = false,
        //            };
        //            config.MaxRdSecond = 120;
        //            config.EnableLogging = false;
        //            config.LockMs = 5000;
        //            config.SleepMs = 300;
        //        }, providerName);
        //    });
        //}

        //public static DbContextOptionsBuilder Configure(this DbContextOptionsBuilder builder, DbContextBaseConfiguration config)
        //{
        //    builder.UseSqlServer(
        //      config.ConnectionString,
        //      optionsBuilder =>
        //      {
        //          optionsBuilder.CommandTimeout((int)config.CommandTimeout.TotalSeconds);
        //          optionsBuilder.EnableRetryOnFailure();

        //          if (!string.IsNullOrEmpty(config.MigrationAssembly))
        //              optionsBuilder.MigrationsAssembly(config.MigrationAssembly);

        //          config.Action?.Invoke(optionsBuilder);
        //      });

        //    if (config.LoggerFactory != null)
        //        builder.UseLoggerFactory(config.LoggerFactory);

        //    builder.ConfigureWarnings(warnings => warnings.Log(CoreEventId.SaveChangesCompleted));
        //    return builder;
        //}

        //public static DbContextOptionsBuilder CreateBuilder(this DbContextBaseConfiguration config)
        //{
        //    return new DbContextOptionsBuilder()
        //        .Configure(config);
        //}

        //public static DbContextOptionsBuilder<TContext> CreateBuilder<TContext>(this DbContextBaseConfiguration config)
        //    where TContext : DbContext
        //{
        //    return new DbContextOptionsBuilder<TContext>().Configure(config);
        //}

        //public static DbContextOptionsBuilder<TContext> Configure<TContext>(this DbContextOptionsBuilder<TContext> builder, DbContextBaseConfiguration config) where TContext : DbContext
        //{
        //    return (DbContextOptionsBuilder<TContext>)Configure((DbContextOptionsBuilder)builder, config);
        //}
    }
}