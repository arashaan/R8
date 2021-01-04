using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Newtonsoft.Json;

using R8.Lib;

namespace R8.EntityFrameworkCore
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

        public bool TryValidate(out ValidatableResultCollection errors)
        {
            errors = new ValidatableResultCollection();
            TryValidate(this, out var tempErrors);
            if (tempErrors?.Any() == true)
                errors.AddRange(tempErrors);

            if (errors?.Any() != true)
                return errors?.Count == 0;

            var ignoredNames = this.GetType().GetPublicProperties().ConvertAll(x => x.Name);
            var ignored = errors.Where(x => ignoredNames.Contains(x.Name)).ToList();
            if (!ignored.Any())
                return errors?.Count == 0;

            foreach (var result in ignored)
                errors.Remove(result);

            var finalValidation = this.Validate();
            return errors.Count == 0 && finalValidation;
        }
    }

    /// <summary>
    /// Initializes a <see cref="EntityBase{TResult}"/> for entities.
    /// </summary>
    public abstract class EntityBase<TEntity> : EntityBase, IEntityTypeConfiguration<TEntity> where TEntity : class, IEntityBase
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder) => builder
            .ApplyConfiguration()
            .ConfigureAuditCollection();
    }
}