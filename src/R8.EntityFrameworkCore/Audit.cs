using Newtonsoft.Json;

using R8.EntityFrameworkCore.JsonConverters;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Net;

namespace R8.EntityFrameworkCore
{
    /// <summary>
    /// An object to track creation, modification, and deletion of specific entity.
    /// </summary>
    public class Audit : IAudit
    {
        /// <summary>
        /// An object to track creation, modification, and deletion of specific entity.
        /// </summary>
        public Audit()
        {
        }

        /// <summary>
        /// An object to track creation, modification, and deletion of specific entity.
        /// </summary>
        /// <param name="flag">A <see cref="AuditFlags"/> enumerator constant that representing type of current instance.</param>
        /// <param name="rowId">A <see cref="Guid"/> value that representing id of specific entity that changed.</param>
        public Audit(AuditFlags flag, Guid rowId)
        {
            Flag = flag;
            RowId = rowId;
        }

        /// <summary>
        /// An object to track creation, modification, and deletion of specific entity.
        /// </summary>
        /// <param name="userId">A <see cref="Nullable{TResult}"/> type of <see cref="Guid"/> that representing Internal ID of specific user that did changes.</param>
        /// <param name="remoteIpAddress">An <see cref="IPAddress"/> that representing current <see cref="HttpRequest"/>'s remote ip address.</param>
        /// <param name="localIpAddress">An <see cref="IPAddress"/> that representing current <see cref="HttpRequest"/>'s local ip address.</param>
        /// <param name="userAgent">A <see cref="string"/> that representing current <see cref="HttpRequest"/>'s user agent.</param>
        /// <param name="flag">A <see cref="AuditFlags"/> enumerator constant that representing type of current instance.</param>
        /// <param name="rowId">A <see cref="Guid"/> value that representing id of specific entity that changed.</param>
        /// <param name="oldValues">A <see cref="Dictionary{TKey,TValue}"/> that representing a dictionary of values that has been changed.</param>
        /// <param name="newValues">A <see cref="Dictionary{TKey,TValue}"/> that representing a dictionary of values that has been replaces with old values.</param>
        public Audit(Guid? userId, IPAddress remoteIpAddress, IPAddress localIpAddress, string userAgent, AuditFlags flag, Guid rowId, Dictionary<string, object> oldValues = null, Dictionary<string, object> newValues = null) : this(flag, rowId)
        {
            UserId = userId;
            DateTime = DateTime.UtcNow;
            Culture = CultureInfo.CurrentCulture;
            RemoteIpAddress = remoteIpAddress;
            LocalIpAddress = localIpAddress;
            UserAgent = userAgent;
            OldValues = oldValues ?? new Dictionary<string, object>();
            NewValues = newValues ?? new Dictionary<string, object>();
        }

        public string UserAgent { get; set; }

        [JsonConverter(typeof(AuditIPAddressConverter))]
        [Column("IpAddress")]
        public IPAddress RemoteIpAddress { get; set; }

        [JsonConverter(typeof(AuditIPAddressConverter))]
        public IPAddress LocalIpAddress { get; set; }

        public long Id { get; set; }

        [JsonConverter(typeof(AuditGuidIdConverter))]
        public Guid? UserId { get; set; }

        [JsonConverter(typeof(AuditCultureConverter))]
        public CultureInfo? Culture { get; set; }

        public Guid RowId { get; set; }

        [JsonConverter(typeof(AuditDateTimeConverter))]
        public DateTime DateTime { get; set; }

        public AuditFlags Flag { get; set; }

        public Dictionary<string, object> OldValues { get; set; } = new Dictionary<string, object>();

        public Dictionary<string, object> NewValues { get; set; } = new Dictionary<string, object>();

        public override string ToString()
        {
            return $"{Flag} at {DateTime.ToShortDateString()} {DateTime.ToShortTimeString()}";
        }
    }
}