using Humanizer;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Newtonsoft.Json;

using R8.Lib.MethodReturn;

using System;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace R8.Lib.AspNetCore.EntityFrameworkCore
{
    public static class EntityBaseExtensions
    {
        public static void SaveDatabase<TModel, TDbContext>(this TModel model, TDbContext dbContext)
            where TModel : IResponseDatabase where TDbContext : DbContextBase
        {
            model.Save = dbContext.SaveChanges();
        }

        public static async Task SaveDatabaseAsync<TModel, TDbContext>(this TModel model, TDbContext dbContext, CancellationToken cancellationToken = default)
            where TModel : IResponseDatabase where TDbContext : DbContextBase
        {
            model.Save = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public static string GetTableName(this EntityEntry entry)
        {
            var entityType = entry.Context.Model.FindEntityType(entry.Entity.GetType());
            var tableName = entityType.GetTableName();

            return tableName;
        }

        public static PropertyBuilder<CultureInfo> HasCultureConversion(this PropertyBuilder<CultureInfo> property)
        {
            return property.HasConversion(
                    x => x.Name,
                    v => !string.IsNullOrEmpty(v) ? CultureInfo.GetCultureInfo(v) : null);
        }

        public static EntityTypeBuilder<TEntity> ApplyConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, IEntityBase
        {
            builder.ToTable(typeof(TEntity).Name.Pluralize());
            builder.HasKey(x => x.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.RowVersion).IsRowVersion();
            builder.ConfigureAuditCollection();
            builder.HasQueryFilter(entity => !entity.IsDeleted);
            return builder;
        }

        public static void ConfigureAuditCollection<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, IEntityBase
        {
            builder.OwnsMany(x => x.Audits, action =>
            {
                action.WithOwner().HasForeignKey(x => x.RowId);
                action.HasKey(x => x.Id);
                action.Property(x => x.Id).ValueGeneratedOnAdd();
                action.Property(x => x.IpAddress).HasIPAddressConversion();
                action.Property(x => x.Culture).HasCultureConversion();
                action.Property(x => x.NewValues).HasJsonConversion();
                action.Property(x => x.OldValues).HasJsonConversion();
            });
        }

        public const string LocalizedNameColumn = "Name";

        public static PropertyBuilder<IPAddress> HasIPAddressConversion(this PropertyBuilder<IPAddress> property)
        {
            return property.HasConversion(
                    x => x.ToString(),
                    x => IPAddress.Parse(x));
        }

        public static PropertyBuilder<GlobalizationCollectionJson> HasGlobalizationContainerConversion(this PropertyBuilder<GlobalizationCollectionJson> property)
        {
            property.HasConversion(
                v => v.Serialize(),
                v => GlobalizationCollectionJson.Deserialize(v))
                .Metadata.SetValueComparer(
                new ValueComparer<GlobalizationCollectionJson>(
                    (l, r) => l.Serialize() == r.Serialize(),
                    v => v == null ? 0 : v.Serialize().GetHashCode(),
                    v => GlobalizationCollectionJson.Deserialize(v.Serialize())));

            return property;
        }

        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> property)
        {
            property.HasConversion(
                x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<T>(x))
                .Metadata.SetValueComparer(new ValueComparer<T>(
                    (l, r) => JsonConvert.SerializeObject(l) == JsonConvert.SerializeObject(r),
                    v => v == null ? 0 : JsonConvert.SerializeObject(v).GetHashCode(),
                    v => JsonConvert.DeserializeObject<T>(
                        JsonConvert.SerializeObject(v))));
            return property;
        }

        public static void HasDateTimeUtcConversion(this ModelBuilder modelBuilder)
        {
            var converter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                foreach (var property in entityType.GetProperties())
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                        property.SetValueConverter(converter);
        }
    }
}