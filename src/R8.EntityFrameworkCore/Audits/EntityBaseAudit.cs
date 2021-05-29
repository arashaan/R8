using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Newtonsoft.Json;

namespace R8.EntityFrameworkCore.Audits
{
    /// <summary>
    /// Initializes a base type for entities those needed to store audits.
    /// </summary>
    public class EntityBaseAudit : IEntityBaseAudit
    {
        [JsonIgnore]
        public AuditCollection Audits { get; set; } = new AuditCollection();
    }

    /// <summary>
    /// Initializes a <see cref="R8.EntityFrameworkCore.EntityBase{TEntity}"/> for entities.
    /// </summary>
    public abstract class EntityBaseAudit<TEntity> : EntityBaseAudit, IEntityTypeConfiguration<TEntity> where TEntity : class, IEntityBaseAudit
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder) => builder
            .ApplyAuditConfiguration();
    }
}