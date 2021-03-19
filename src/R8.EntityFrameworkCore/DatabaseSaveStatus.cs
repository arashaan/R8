using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System.Collections.Generic;

namespace R8.EntityFrameworkCore
{
    /// <summary>
    /// A class that indicates <see cref="DbContext"/> save state.
    /// </summary>
    /// <remarks>This object has direct cast to <see cref="DatabaseSaveState"/> enum based on <see cref="Save"/> value.</remarks>
    public class DatabaseSaveStatus
    {
        /// <summary>
        /// Gets <see cref="DbContext"/> save state.
        /// </summary>
        public DatabaseSaveState Save { get; internal set; }

        /// <summary>
        /// Gets an error is encountered while saving to the database.
        /// </summary>
        public DbUpdateException DbUpdateException { get; internal set; }

        /// <summary>
        ///     <para>
        ///         A concurrency violation is encountered while saving to the database.
        ///         A concurrency violation occurs when an unexpected number of rows are affected during save.
        ///         This is usually because the data in the database has been modified since it was loaded into memory.
        ///     </para>
        /// </summary>
        public DbUpdateConcurrencyException DbUpdateConcurrencyException { get; internal set; }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of entity entries that should be saved.
        /// </summary>
        public IEnumerable<EntityEntry> EntityEntries { get; internal set; }

        /// <summary>
        /// Get total count of changes before preparing to save changes.
        /// </summary>
        public int ChangesBeforeSave { get; internal set; }

        /// <summary>
        /// Gets the number of state entries written to the database.
        /// </summary>
        public int ChangesAfterSave { get; internal set; }

        public static implicit operator DatabaseSaveState(DatabaseSaveStatus obj)
        {
            return obj.Save;
        }
    }
}