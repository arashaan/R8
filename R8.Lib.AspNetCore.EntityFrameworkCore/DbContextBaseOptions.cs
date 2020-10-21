using System;
using EasyCaching.InMemory;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace R8.Lib.AspNetCore.EntityFrameworkCore
{
    public class DbContextBaseConfiguration
    {
        public string ConnectionString { get; set; }

        public string MigrationAssembly { get; set; }

        public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromMinutes(3);

        public ILoggerFactory LoggerFactory { get; set; }

        public Action<SqlServerDbContextOptionsBuilder> Action { get; set; }
    }

    public static class DbContextBaseOptions
    {
        public static void AddDbContextPoolR8<TContext>(this IServiceCollection services, Action<DbContextBaseConfiguration> config) where TContext : DbContext
        {
            var preConfig = new DbContextBaseConfiguration
            {
                CommandTimeout = TimeSpan.FromMinutes(3)
            };
            config?.Invoke(preConfig);

            var providerName = typeof(TContext).Name;
            services.AddEFSecondLevelCache(options =>
            {
                options.UseEasyCachingCoreProvider(providerName, isHybridCache: false).DisableLogging(true);
                // options.CacheAllQueries(CacheExpirationMode.Absolute, TimeSpan.FromDays(1));
            });
            services.AddEasyCaching(options =>
            {
                options.UseInMemory(config =>
                {
                    config.DBConfig = new InMemoryCachingOptions
                    {
                        ExpirationScanFrequency = 60,
                        SizeLimit = 100,
                        EnableReadDeepClone = false,
                        EnableWriteDeepClone = false,
                    };
                    config.MaxRdSecond = 120;
                    config.EnableLogging = false;
                    config.LockMs = 5000;
                    config.SleepMs = 300;
                }, providerName);
            });

            if (string.IsNullOrEmpty(preConfig.ConnectionString))
                throw new NullReferenceException(nameof(preConfig.ConnectionString));

            services.AddDbContextPool<TContext>((serviceProvider, builder) =>
            {
                builder.EnableDetailedErrors();
                builder.EnableSensitiveDataLogging(); // Often also useful with EnableDetailedErrors
                builder.Configure(preConfig);
                builder.AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());
            });
        }

        public static DbContextOptionsBuilder Configure(this DbContextOptionsBuilder builder, DbContextBaseConfiguration config)
        {
            builder.UseSqlServer(
              config.ConnectionString,
              optionsBuilder =>
              {
                  optionsBuilder.CommandTimeout((int)config.CommandTimeout.TotalSeconds);
                  optionsBuilder.EnableRetryOnFailure();

                  if (!string.IsNullOrEmpty(config.MigrationAssembly))
                      optionsBuilder.MigrationsAssembly(config.MigrationAssembly);

                  config.Action?.Invoke(optionsBuilder);
              });

            if (config.LoggerFactory != null)
                builder.UseLoggerFactory(config.LoggerFactory);

            builder.ConfigureWarnings(warnings => warnings.Log(CoreEventId.SaveChangesCompleted));
            return builder;
        }

        public static DbContextOptionsBuilder CreateBuilder(this DbContextBaseConfiguration config)
        {
            return new DbContextOptionsBuilder()
                .Configure(config);
        }

        public static DbContextOptionsBuilder<TContext> CreateBuilder<TContext>(this DbContextBaseConfiguration config)
            where TContext : DbContext
        {
            return new DbContextOptionsBuilder<TContext>().Configure(config);
        }

        public static DbContextOptionsBuilder<TContext> Configure<TContext>(this DbContextOptionsBuilder<TContext> builder, DbContextBaseConfiguration config) where TContext : DbContext
        {
            return (DbContextOptionsBuilder<TContext>)Configure((DbContextOptionsBuilder)builder, config);
        }
    }
}