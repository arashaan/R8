using Newtonsoft.Json;

using R8.Lib.JsonConverters;

using System.Globalization;
using System.Net;

namespace R8.EntityFrameworkCore.AuditSubClasses
{
    public class AuditMachineInformation
    {
        [JsonConverter(typeof(JsonCultureConverter))]
        public CultureInfo? Culture { get; set; }

        [JsonConverter(typeof(JsonIPAddressConverter))]
        public IPAddress RemoteIP { get; set; }

        [JsonConverter(typeof(JsonIPAddressConverter))]
        public IPAddress LocalIP { get; set; }

        [JsonConverter(typeof(JsonIPAddressConverter))]
        public IPAddress UserIP { get; set; }

        public string UserAgent { get; set; }
    }
}