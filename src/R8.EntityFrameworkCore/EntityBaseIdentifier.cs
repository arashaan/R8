using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Newtonsoft.Json;

using R8.Lib.Validatable;

using System;
using System.ComponentModel.DataAnnotations;

namespace R8.EntityFrameworkCore
{
    public class EntityBaseIdentifier : ValidatableObject, IEntityBaseIdentifier
    {
        [Required]
        public Guid Id { get; set; }
    }

    /// <summary>
    /// Initializes a <see cref="R8.EntityFrameworkCore.EntityBaseIdentifier{TEntity}"/> for entities.
    /// </summary>
    public abstract class EntityBaseIdentifier<TEntity> : EntityBaseIdentifier, IEntityTypeConfiguration<TEntity> where TEntity : class, IEntityBaseIdentifier
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder) => builder
            .ApplyIdentifierConfiguration();
    }
}