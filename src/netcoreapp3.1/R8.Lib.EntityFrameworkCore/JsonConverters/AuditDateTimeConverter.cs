using Newtonsoft.Json;

using System;
using System.Diagnostics.CodeAnalysis;

namespace R8.Lib.EntityFrameworkCore.JsonConverters
{
    public class AuditDateTimeConverter : JsonConverter<DateTime>
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
}