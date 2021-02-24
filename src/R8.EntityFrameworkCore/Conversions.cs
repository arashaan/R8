using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Newtonsoft.Json;

using System;
using System.Globalization;

namespace R8.EntityFrameworkCore
{
    public static class Conversions
    {
        /// <summary>
        /// Converts given property to TWO-LETTER ISO LANGUAGE NAME of THE CULTURE to save in Database and get back in original type for scaffolding.
        /// </summary>
        /// <param name="property">A <see cref="PropertyBuilder"/> object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A programmed <see cref="PropertyBuilder"/>.</returns>
        public static PropertyBuilder<CultureInfo> HasCultureConversion(this PropertyBuilder<CultureInfo> property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            return property.HasConversion(
                x => x.Name,
                v => !string.IsNullOrEmpty(v) ? CultureInfo.GetCultureInfo(v) : null);
        }

        /// <summary>
        /// Converts given property to JSON to save in Database and get back in original type for scaffolding.
        /// </summary>
        /// <typeparam name="T">Type of given property.</typeparam>
        /// <param name="property">A <see cref="PropertyBuilder"/> object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A programmed <see cref="PropertyBuilder"/>.</returns>
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

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

        /// <summary>
        /// Converts all CLR types of <see cref="DateTime"/> to UTC format to save in Database and back.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static ModelBuilder HasDateTimeUtcConversion(this ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

            var converter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                foreach (var property in entityType.GetProperties())
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                        property.SetValueConverter(converter);

            return modelBuilder;
        }
    }
}