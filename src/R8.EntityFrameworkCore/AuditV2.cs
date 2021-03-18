using Newtonsoft.Json;

using R8.EntityFrameworkCore.AuditSubClasses;
using R8.Lib.JsonConverters;

using System;
using System.Collections.Generic;

namespace R8.EntityFrameworkCore
{
    public class AuditV2
    {
        public Guid Id { get; set; }

        public Guid RowId { get; set; }

        [JsonConverter(typeof(JsonDateTimeToUnixConverter))]
        public DateTime DateTime { get; set; }

        [JsonConverter(typeof(JsonGuidConverter))]
        public Guid? UserId { get; set; }

        public string TransactionId { get; set; }
        public AuditFlags Flag { get; set; }
        public List<AuditChanges> Changes { get; set; }
        public AuditMachineInformation Machine { get; set; }
        public string Additional { get; set; }

        public override string ToString()
        {
            return $"{Flag} at {DateTime.ToShortDateString()} {DateTime.ToShortTimeString()}";
        }
    }
}