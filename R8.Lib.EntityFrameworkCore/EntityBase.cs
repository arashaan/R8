using Humanizer;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Newtonsoft.Json;

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace R8.Lib.EntityFrameworkCore
{
    /// <summary>
    /// Initializes a base type for entities.
    /// </summary>
    public class EntityBase : ValidatableObject, IEntityBase
    {
        [Required]
        public Guid Id { get; set; }

        [JsonIgnore, EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] RowVersion { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public AuditCollection Audits { get; set; }
    }

    /// <summary>
    /// Initializes a <see cref="EntityBase{TResult}"/> for entities.
    /// </summary>
    public abstract class EntityBase<TEntity> : EntityBase, IEntityTypeConfiguration<TEntity> where TEntity : class, IEntityBase
    {
        public static string GetTableName() => typeof(TEntity).Name.Pluralize();

        public virtual void Configure(EntityTypeBuilder<TEntity> builder) => builder
            .ApplyConfiguration()
            .ConfigureAuditCollection();
    }
}