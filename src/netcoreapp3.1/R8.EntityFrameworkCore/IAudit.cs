using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace R8.EntityFrameworkCore
{
    /// <summary>
    /// An <see cref="IAudit"/> interface that tracks changes for specific entity.
    /// </summary>
    public interface IAudit
    {
        /// <summary>
        /// Gets or sets calling method name based on <see cref="CallerMemberNameAttribute"/>.
        /// </summary>
        string CallingMethodName { get; set; }

        /// <summary>
        /// Gets of sets calling method path based on <see cref="CallerFilePathAttribute"/>.
        /// </summary>
        string CallingMethodPath { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="long"/> value that representing Context-generated id for current instance.
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// User's Internal Identifier of the <see cref="Audit"/> Provider.
        /// </summary>
        Guid? UserId { get; set; }

        /// <summary>
        /// Gets or sets <see cref="CultureInfo"/> that currently used in platform.
        /// </summary>
        CultureInfo? Culture { get; set; }

        /// <summary>
        /// Gets or sets an specific table row id that current instance is related to.
        /// </summary>
        Guid RowId { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DateTime"/> object that representing when current instance is created.
        /// </summary>
        DateTime DateTime { get; set; }

        /// <summary>
        /// Gets or sets type of <see cref="IAudit"/>.
        /// </summary>
        AuditFlags Flag { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey,TValue}"/> that representing old values that they changed.
        /// </summary>
        /// <remarks>This property only works when <see cref="Flag"/> is <see cref="AuditFlags.Changed"/></remarks>
        Dictionary<string, object> OldValues { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey,TValue}"/> that representing new values that they replaced with <see cref="OldValues"/> iterates.
        /// </summary>
        /// <remarks>This property only works when <see cref="Flag"/> is <see cref="AuditFlags.Changed"/></remarks>
        Dictionary<string, object> NewValues { get; set; }
    }
}