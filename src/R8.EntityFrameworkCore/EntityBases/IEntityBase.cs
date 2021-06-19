using R8.EntityFrameworkCore.Audits;
using R8.Lib.Validatable;

namespace R8.EntityFrameworkCore.EntityBases
{
    /// <summary>
    /// An <see cref="IEntityBase"/> interface that representing some basic data about specific entity.
    /// </summary>
    public interface IEntityBase : IEntityBaseIdentifier, IEntityBaseAudit
    {
        /// <summary>
        /// Gets or sets shown status for current entity.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets an array of bytes that representing row version to avoid conflicting data where users works on current entity simultaneously.
        /// </summary>
        public byte[] RowVersion { get; set; }

        bool TryValidate(out ValidatableResultCollection errors);
    }
}