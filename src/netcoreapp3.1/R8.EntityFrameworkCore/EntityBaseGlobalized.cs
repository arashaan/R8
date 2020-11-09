using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using R8.Lib.Localization;

namespace R8.EntityFrameworkCore
{
    /// <summary>
    /// Initializes a <see cref="EntityBaseGlobalized"/> instance that representing some basic data about and also globalized information  specific entity.
    /// </summary>
    public class EntityBaseGlobalized : EntityBase, IEntityBaseGlobalized
    {
        /// <summary>
        /// Initializes a <see cref="EntityBaseGlobalized"/> instance that representing some basic data about and also globalized information  specific entity.
        /// </summary>
        public EntityBaseGlobalized()
        {
            NameJson = new LocalizerContainer().Serialize();
            Name = new LocalizerContainer();
        }

        /// <summary>
        /// Gets or sets a kebab-case name stands for <see cref="Name"/> in English culture.
        /// </summary>
        public string CanonicalName { get; set; }

        [Column("Name")]
        public string NameJson { get; set; }

        [NotMapped]
        public LocalizerContainer Name
        {
            get => LocalizerContainer.Deserialize(NameJson);
            set => NameJson = value.Serialize();
        }
    }

    /// <summary>
    /// Initializes a <see cref="EntityBaseGlobalized{TResult}"/> instance that representing some basic data about and also globalized information  specific entity.
    /// </summary>
    public abstract class EntityBaseGlobalized<TEntity> : EntityBaseGlobalized, IEntityTypeConfiguration<TEntity> where TEntity : class, IEntityBaseGlobalized
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder) => builder
                .ApplyConfiguration()
                .ConfigureAuditCollection();
    }
}