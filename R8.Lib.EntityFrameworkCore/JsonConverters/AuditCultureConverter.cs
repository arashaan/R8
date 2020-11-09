using Newtonsoft.Json;

using System;
using System.Globalization;

namespace R8.Lib.EntityFrameworkCore.JsonConverters
{
    public class AuditCultureConverter : JsonConverter<CultureInfo?>
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
}