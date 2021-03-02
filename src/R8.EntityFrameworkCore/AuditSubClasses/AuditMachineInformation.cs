using Newtonsoft.Json;

using R8.EntityFrameworkCore.JsonConverters;

using System.Globalization;
using System.Net;

namespace R8.EntityFrameworkCore.AuditSubClasses
{
    public class AuditMachineInformation
    {
        [JsonConverter(typeof(AuditCultureConverter))]
        public CultureInfo? Culture { get; set; }

        [JsonConverter(typeof(AuditIPAddressConverter))]
        public IPAddress RemoteIP { get; set; }

        [JsonConverter(typeof(AuditIPAddressConverter))]
        public IPAddress LocalIP { get; set; }

        [JsonConverter(typeof(AuditIPAddressConverter))]
        public IPAddress UserIP { get; set; }

        public string UserAgent { get; set; }
    }
}