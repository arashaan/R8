using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using R8.EntityFrameworkCore.Audits;
using R8.EntityFrameworkCore.Test.FakeDatabase.FakeEntities;

namespace R8.EntityFrameworkCore.Test.FakeDatabase
{
    public class FakeDbContext : DbContext
    {
        public FakeDbContext(DbContextOptions<FakeDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public static DbContextOptions<FakeDbContext> GetOptionsWithoutDeleteColumn()
        {
            var serviceCollection = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddMemoryCache()
                .AddEFAuditProvider(config =>
                {
                    config.ProviderType = typeof(FakeAuditProvider);
                    config.PermanentDelete = false;
                    config.DeleteColumn = null;
                    config.UntrackableColumns = new[] { nameof(EntityBase.RowVersion) };
                });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var optionsBuilder = new DbContextOptionsBuilder<FakeDbContext>();
            optionsBuilder.UseInMemoryDatabase(nameof(FakeDbContext))
                .UseInternalServiceProvider(serviceProvider)
                .AddEFAuditProviderInterceptor(serviceProvider);

            return optionsBuilder.Options;
        }

        public static DbContextOptions<FakeDbContext> GetOptionsPermanentDelete()
        {
            var serviceCollection = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddMemoryCache()
                .AddEFAuditProvider(config =>
                {
                    config.ProviderType = typeof(FakeAuditProvider);
                    config.PermanentDelete = true;
                    config.UntrackableColumns = new[] { nameof(EntityBase.RowVersion) };
                });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var optionsBuilder = new DbContextOptionsBuilder<FakeDbContext>();
            optionsBuilder.UseInMemoryDatabase(nameof(FakeDbContext))
                .UseInternalServiceProvider(serviceProvider)
                .AddEFAuditProviderInterceptor(serviceProvider);

            return optionsBuilder.Options;
        }

        public static DbContextOptions<FakeDbContext> GetOptions()
        {
            var serviceCollection = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddMemoryCache()
                .AddEFAuditProvider(config =>
                {
                    config.ProviderType = typeof(FakeAuditProvider);
                    config.PermanentDelete = false;
                    config.UntrackableColumns = new[] { nameof(EntityBase.RowVersion) };
                });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var optionsBuilder = new DbContextOptionsBuilder<FakeDbContext>();
            optionsBuilder.UseInMemoryDatabase(nameof(FakeDbContext))
                .UseInternalServiceProvider(serviceProvider)
                .AddEFAuditProviderInterceptor(serviceProvider);

            return optionsBuilder.Options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}