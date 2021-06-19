using System;

namespace R8.EntityFrameworkCore.EntityBases
{
    /// <summary>
    /// An <see cref="IEntityBaseIdentifier"/> interface that representing some basic data about specific entity.
    /// </summary>
    public interface IEntityBaseIdentifier
    {
        /// <summary>
        /// Gets or sets <see cref="Guid"/> Id for current entity.
        /// </summary>
        public Guid Id { get; set; }
    }
}