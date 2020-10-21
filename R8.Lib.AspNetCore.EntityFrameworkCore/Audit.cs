using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using Newtonsoft.Json;

using R8.Lib.AspNetCore.Base;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;

namespace R8.Lib.AspNetCore.EntityFrameworkCore
{
    public sealed class Audit
    {
        public Audit()
        {
        }

        public Audit(Guid? userId, IPAddress ip, string userAgent, EntityEntry entry, AuditFlags flag, Guid rowId, string callingMethodName, string callingMethodPath) : this()
        {
            UserId = userId;
            IpAddress = ip;
            UserAgent = userAgent;
            Flag = flag;
            DateTime = DateTime.UtcNow;
            Culture = CultureInfo.CurrentCulture;
            RowId = rowId;
            CallingMethodName = callingMethodName;
            CallingMethodPath = callingMethodPath;

            if (entry.State != EntityState.Modified && entry.State != EntityState.Added)
                return;

            var propertyEntries = entry.Members
                .Where(x => x.Metadata.Name != nameof(EntityBase.Id)
                            && x.Metadata.Name != nameof(EntityBase.Audits)
                            && x.Metadata.Name != nameof(EntityBase.RowVersion)
                            && x is PropertyEntry)
                .Cast<PropertyEntry>()
                .ToList();
            if (!propertyEntries.Any())
                return;

            foreach (var propertyEntry in propertyEntries)
            {
                var propertyName = propertyEntry.Metadata.Name;
                var newValue = propertyEntry.CurrentValue;
                var oldValue = propertyEntry.OriginalValue;

                if (newValue == null && oldValue == null)
                    continue;

                if (newValue?.Equals(oldValue) == true)
                    continue;

                OldValues.Add(propertyName, oldValue);
                NewValues.Add(propertyName, newValue);
            }
        }

        public string UserAgent { get; set; }

        public string CallingMethodName { get; set; }

        public string CallingMethodPath { get; set; }

        public long Id { get; set; }

        [JsonConverter(typeof(AuditIPAddressConverter))]
        public IPAddress IpAddress { get; set; }

        /// <summary>
        /// User's Internal Identifier of the <see cref="Audit"/> Provider —based on <seealso cref="CurrentUser"/>
        /// </summary>
        [JsonConverter(typeof(AuditUserIdConverter))]
        public Guid? UserId { get; set; }

        [JsonConverter(typeof(AuditCultureConverter))]
        public CultureInfo? Culture { get; set; }

        //public long Id { get; set; }

        public Guid RowId { get; set; }

        [JsonConverter(typeof(AuditDateTimeConverter))]
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Action's flag
        /// </summary>
        public AuditFlags Flag { get; set; }

        /// <summary>
        /// Dictionary of changes in type of <see cref="Dictionary{TKey,TValue}"/> —when flag is <see cref="AuditFlags.Changed"/>
        /// </summary>
        public Dictionary<string, object> OldValues { get; set; } = new Dictionary<string, object>();

        public Dictionary<string, object> NewValues { get; set; } = new Dictionary<string, object>();

        public override string ToString()
        {
            return $"{Flag} at {DateTime.ToShortDateString()}";
        }

        internal class AuditIPAddressConverter : JsonConverter<IPAddress>
        {
            public override void WriteJson(JsonWriter writer, IPAddress value, JsonSerializer serializer)
            {
                var plainIp = value.ToString();
                writer.WriteValue(plainIp);
            }

            public override IPAddress ReadJson(JsonReader reader, Type objectType, IPAddress existingValue, bool hasExistingValue,
                JsonSerializer serializer)
            {
                var ip = (string)reader.Value;

                if (string.IsNullOrEmpty(ip))
                    return IPAddress.None;

                return IPAddress.Parse(ip);
            }
        }

        internal class AuditCultureConverter : JsonConverter<CultureInfo?>
        {
            public override void WriteJson(JsonWriter writer, CultureInfo? value, JsonSerializer serializer)
            {
                if (value == null)
                    return;

                var culture = value.Name;
                writer.WriteValue(culture);
            }

            public override CultureInfo? ReadJson(JsonReader reader, Type objectType, CultureInfo? existingValue, bool hasExistingValue,
                JsonSerializer serializer)
            {
                var culture = (string)reader.Value;
                return string.IsNullOrEmpty(culture)
                    ? null
                    : CultureInfo.GetCultureInfo(culture);
            }
        }

        internal class AuditDateTimeConverter : JsonConverter<DateTime>
        {
            public override DateTime ReadJson(JsonReader reader, Type objectType, [AllowNull] DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var ticks = (long)reader.Value;
                return ticks.FromUnixTime();
            }

            public override void WriteJson(JsonWriter writer, [AllowNull] DateTime value, JsonSerializer serializer)
            {
                var ticks = value.ToUnixTime();
                writer.WriteValue(ticks);
            }
        }

        internal class AuditUserIdConverter : JsonConverter<Guid?>
        {
            public override void WriteJson(JsonWriter writer, Guid? value, JsonSerializer serializer)
            {
                if (value == null)
                    writer.WriteValue((string)null);
                else
                    writer.WriteValue(value.ToString());
            }

            public override Guid? ReadJson(JsonReader reader, Type objectType, Guid? existingValue, bool hasExistingValue,
                JsonSerializer serializer)
            {
                var s = (string)reader.Value;
                if (string.IsNullOrEmpty(s))
                    return null;

                return Guid.Parse(s);
            }
        }
    }

    public class AuditCollection : List<Audit>
    {
        public override string ToString()
        {
            var toString = $"{this.Count} Audits.";
            if (Last == null) return toString;

            var lastText = $"Last: {Last.Flag} - {Last.DateTime}";
            return $"{toString}. {lastText}";
        }

        /// <summary>
        /// Returns iterates with <see cref="AuditFlags.Created"/> flag
        /// </summary>
        public Audit Created => this.First(x => x.Flag == AuditFlags.Created);

        private static JsonSerializerSettings SerializerSettings
        {
            get
            {
                var settings = JsonSettingsExtensions.JsonNetSettings;
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;

                return settings;
            }
        }

        public string Serialize()
        {
            var json = JsonConvert.SerializeObject(this, SerializerSettings);
            return json;
        }

        public static explicit operator AuditCollection(string json)
        {
            return Deserialize(json);
        }

        public static implicit operator string(AuditCollection collection)
        {
            return collection.Serialize();
        }

        public static AuditCollection Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                return new AuditCollection();

            AuditCollection obj;
            try
            {
                obj = JsonConvert.DeserializeObject<AuditCollection>(json, SerializerSettings) ?? new AuditCollection();
            }
            catch
            {
                obj = new AuditCollection();
            }

            return obj;
        }

        /// <summary>
        /// Returns last iterates
        /// </summary>
        public Audit Last => this.OrderByDescending(x => x.DateTime).FirstOrDefault();
    }
}