using Newtonsoft.Json;

using R8.Lib.JsonConverters;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace R8.EntityFrameworkCore.Audits
{
    /// <summary>
    /// An object to track creation, modification, and deletion of specific entity.
    /// </summary>
    public class Audit
    {
        /// <summary>
        /// Gets or sets a <see cref="Guid"/> value that representing Context-generated id for current instance.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets an specific table row id that current instance is related to.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid RowId { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DateTime"/> object that representing when current instance is created.
        /// </summary>
        [JsonConverter(typeof(JsonDateTimeToUnixConverter))]
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Gets or sets type of <see cref="Audit"/>.
        /// </summary>
        public AuditFlags Flag { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of <see cref="AuditChange"/> that representing changed values.
        /// </summary>
        /// <remarks>This property only works when <see cref="Flag"/> is <see cref="AuditFlags.Changed"/></remarks>
        public AuditChangesCollection Changes { get; set; } = new AuditChangesCollection();

        /// <summary>
        /// A place to adding anything further.
        /// </summary>
        public object Additional { get; set; }

        public override string ToString()
        {
            return $"{Flag} at {DateTime.ToShortDateString()} {DateTime.ToShortTimeString()}";
        }
    }
}