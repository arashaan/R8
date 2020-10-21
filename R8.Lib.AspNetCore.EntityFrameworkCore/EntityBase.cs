using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace R8.Lib.AspNetCore.EntityFrameworkCore
{
    public interface IEntityBase
    {
        public Guid Id { get; set; }

        public byte[] RowVersion { get; set; }

        public bool IsDeleted { get; set; }

        public AuditCollection Audits { get; set; }
    }

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

    public abstract class EntityBase<TEntity> : EntityBase, IEntityTypeConfiguration<TEntity> where TEntity : class, IEntityBase
    {
        public static string GetTableName() => typeof(TEntity).Name.Pluralize();

        public virtual void Configure(EntityTypeBuilder<TEntity> builder) => builder.ApplyConfiguration();
    }
}