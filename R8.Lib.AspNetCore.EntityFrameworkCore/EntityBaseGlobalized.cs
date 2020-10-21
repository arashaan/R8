using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace R8.Lib.AspNetCore.EntityFrameworkCore
{
    public interface IEntityGlobalized
    {
        public string NameJson { get; set; }

        public GlobalizationCollectionJson Name
        {
            get => GlobalizationCollectionJson.Deserialize(NameJson);
            set => NameJson = value.Serialize();
        }
    }

    public interface IEntityBaseGlobalized : IEntityBase, IEntityGlobalized
    {
    }

    public class EntityBaseGlobalized : EntityBase, IEntityBaseGlobalized
    {
        public EntityBaseGlobalized()
        {
            NameJson = new GlobalizationCollectionJson().Serialize();
            Name = new GlobalizationCollectionJson();
        }

        public string CanonicalName { get; set; }

        [Column("Name")]
        public string NameJson { get; set; }

        [NotMapped]
        public GlobalizationCollectionJson Name
        {
            get => GlobalizationCollectionJson.Deserialize(NameJson);
            set => NameJson = value.Serialize();
        }
    }

    public static class EntityBaseGlobalizedExtensions
    {
        public static IQueryable<TSource> WhereHas<TSource>(this IQueryable<TSource> source, string canonicalName, string localizedName) where TSource : EntityBaseGlobalized
        {
            // {"en":"Test"}
            var twoLetter = CultureInfo.CurrentCulture.GetTwoLetterCulture();
            return source.Where(x =>
                x.CanonicalName.Equals(canonicalName) ||
                EF.Functions.Like(x.NameJson, $"%\"{twoLetter}\":\"{localizedName}\"%"));
            //return source.Where(x =>
            //    x.NameJson.Any(c => c.CultureString == CultureInfo.CurrentCulture.EnglishName && c.Text == name));
        }
    }

    public abstract class EntityBaseGlobalized<TEntity> : EntityBaseGlobalized, IEntityTypeConfiguration<TEntity> where TEntity : class, IEntityBaseGlobalized
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.ApplyConfiguration();
        }
    }
}