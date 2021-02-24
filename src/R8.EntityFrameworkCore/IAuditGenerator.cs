using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System;

namespace R8.EntityFrameworkCore
{
    /// <summary>
    /// An interface to present a <see cref="DbContext"/> potentially can save audit information.
    /// </summary>
    public interface IAuditGenerator
    {
        /// <summary>
        /// A <see cref="Func{TResult}"/> to invoke each time each entity need to be saved in Database.
        /// </summary>
        Func<EntityEntry, AuditOptions> AuditOptions { get; }
    }
}