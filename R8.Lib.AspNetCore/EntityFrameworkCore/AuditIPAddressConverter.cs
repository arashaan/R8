using Newtonsoft.Json;

using System;
using System.Net;

namespace R8.Lib.AspNetCore.EntityFrameworkCore
{
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

            return string.IsNullOrEmpty(ip)
                ? IPAddress.None
                : IPAddress.Parse(ip);
        }
    }
}