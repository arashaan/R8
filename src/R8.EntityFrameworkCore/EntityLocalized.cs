using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using R8.Lib.Localization;

using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace R8.EntityFrameworkCore
{
    /// <summary>
    /// Initializes a <see cref="EntityLocalized"/> instance that representing some basic data about and also globalized information  specific entity.
    /// </summary>
    public class EntityLocalized : EntityBase, IEntityLocalized
    {
        /// <summary>
        /// Initializes a <see cref="EntityLocalized"/> instance that representing some basic data about and also globalized information  specific entity.
        /// </summary>
        public EntityLocalized()
        {
            NameJson = new LocalizerContainer().Serialize();
            Name = new LocalizerContainer();
        }

        /// <summary>
        /// Gets or sets a kebab-case name stands for <see cref="Name"/> in English culture.
        /// </summary>
        public string Slug { get; set; }

        [Column("Name")]
        public string NameJson { get; set; }

        [NotMapped]
        public LocalizerContainer Name
        {
            get => LocalizerContainer.Deserialize(NameJson);
            set => NameJson = value.Serialize();
        }

        public LocalizerContainer UpdateName(string cultureTwoLetterIso, string value) =>
            this.UpdateName(CultureInfo.GetCultureInfo(cultureTwoLetterIso), value);

        public LocalizerContainer UpdateName(CultureInfo culture, string value)
        {
            this.Name = LocalizerContainer.Clone(this.Name, culture, value);
            return this.Name;
        }

        public LocalizerContainer UpdateName(string value) => this.UpdateName(CultureInfo.CurrentCulture, value);
    }

    /// <summary>
    /// Initializes a <see cref="EntityLocalized{TEntity}"/> instance that representing some basic data about and also globalized information  specific entity.
    /// </summary>
    public abstract class EntityLocalized<TEntity> : EntityLocalized, IEntityTypeConfiguration<TEntity> where TEntity : class, IEntityLocalized
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder) => builder
            .ApplyConfiguration();
    }
}