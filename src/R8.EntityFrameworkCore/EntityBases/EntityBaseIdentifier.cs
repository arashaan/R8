using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using R8.Lib.Validatable;

namespace R8.EntityFrameworkCore.EntityBases
{
    public class EntityBaseIdentifier : ValidatableObject, IEntityBaseIdentifier
    {
        [Required]
        public Guid Id { get; set; }
    }

    /// <summary>
    /// Initializes a <see cref="EntityBaseIdentifier{TEntity}"/> for entities.
    /// </summary>
    public abstract class EntityBaseIdentifier<TEntity> : EntityBaseIdentifier, IEntityTypeConfiguration<TEntity> where TEntity : class, IEntityBaseIdentifier
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder) => builder
            .ApplyIdentifierConfiguration();
    }
}