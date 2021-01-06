using System;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

using R8.Lib;

namespace R8.EntityFrameworkCore.JsonConverters
{
    public class AuditDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime ReadJson(JsonReader reader, Type objectType, [AllowNull] DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var ticks = (long)reader.Value;
            return Dates.FromUnixTime(ticks);
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] DateTime value, JsonSerializer serializer)
        {
            var ticks = value.ToUnixTime();
            writer.WriteValue(ticks);
        }
    }
}