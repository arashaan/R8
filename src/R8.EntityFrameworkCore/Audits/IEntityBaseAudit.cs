namespace R8.EntityFrameworkCore.Audits
{
    /// <summary>
    /// An <see cref="IEntityBaseAudit"/> interface that representing needed properties to store audits.
    /// </summary>
    public interface IEntityBaseAudit
    {
        /// <summary>
        /// Gets or sets a collection of <see cref="IAudit"/> to track entity changes.
        /// </summary>
        public AuditCollection Audits { get; set; }
    }
}