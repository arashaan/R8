using System;

namespace R8.EntityFrameworkCore
{
    /// <summary>
    /// An <see cref="IEntityBase"/> interface that representing some basic data about specific entity.
    /// </summary>
    public interface IEntityBase
    {
        /// <summary>
        /// Gets or sets <see cref="Guid"/> Id for current entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets an array of bytes that representing row version to avoid conflicting data where users works on current entity simultaneously.
        /// </summary>
        public byte[] RowVersion { get; set; }

        /// <summary>
        /// Gets or sets shown status for current entity.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets a collection of <see cref="IAudit"/> to track entity changes.
        /// </summary>
        public AuditCollection Audits { get; set; }
    }
}