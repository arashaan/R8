using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

using R8.EntityFrameworkCore;
using R8.EntityFrameworkCore.JsonConverters;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;

namespace R8.AspNetCore.EntityFrameworkCore
{
    /// <summary>
    /// An object to track creation, modification, and deletion of specific entity.
    /// </summary>
    public partial class Audit : IAudit
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
        /// <param name="userId">A <see cref="Nullable{TResult}"/> type of <see cref="Guid"/> that representing Internal ID of specific user that did changes.</param>
        /// <param name="ipAddress">An <see cref="IPAddress"/> that representing current <see cref="HttpRequest"/>'s ip address.</param>
        /// <param name="userAgent">A <see cref="string"/> that representing current <see cref="HttpRequest"/>'s user agent.</param>
        /// <param name="flag">A <see cref="AuditFlags"/> enumerator constant that representing type of current instance.</param>
        /// <param name="rowId">A <see cref="Guid"/> value that representing id of specific entity that changed.</param>
        /// <param name="callingMethodName">A <see cref="string"/> value that representing method or member name that prepared to change data.</param>
        /// <param name="callingMethodPath">A <see cref="string"/> value that representing file path of method or member name that prepared to change data.</param>
        /// <param name="oldValues">A <see cref="Dictionary{TKey,TValue}"/> that representing a dictionary of values that has been changed.</param>
        /// <param name="newValues">A <see cref="Dictionary{TKey,TValue}"/> that representing a dictionary of values that has been replaces with old values.</param>
        public Audit(Guid? userId, IPAddress ipAddress, string userAgent, AuditFlags flag, Guid rowId, string callingMethodName, string callingMethodPath, Dictionary<string, object> oldValues = null, Dictionary<string, object> newValues = null)
        {
            IpAddress = ipAddress;
            UserAgent = userAgent;
            UserId = userId;
            Flag = flag;
            DateTime = DateTime.UtcNow;
            Culture = CultureInfo.CurrentCulture;
            RowId = rowId;
            CallingMethodName = callingMethodName;
            CallingMethodPath = callingMethodPath;
            OldValues = oldValues ?? new Dictionary<string, object>();
            NewValues = newValues ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets an <see cref="string"/> that representing User-Agent according to <see cref="HttpRequest"/>'s Headers.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="IPAddress"/> that representing IP Address according to <see cref="HttpRequest"/>.
        /// </summary>
        [JsonConverter(typeof(AuditIPAddressConverter))]
        public IPAddress IpAddress { get; set; }

        public string CallingMethodName { get; set; }

        public string CallingMethodPath { get; set; }

        public long Id { get; set; }

        [JsonConverter(typeof(AuditUserIdConverter))]
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
            return $"{Flag} at {DateTime.ToShortDateString()}";
        }
    }
}