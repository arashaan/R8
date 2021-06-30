using Humanizer;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace R8.EntityFrameworkCore.Audits
{
    public static class AuditExtensions
    {
        internal static EntityTypeBuilder<TEntity> ApplyAuditConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, IEntityBaseAudit
        {
            builder.OwnsMany(x => x.Audits, action =>
            {
                action.WithOwner().HasForeignKey(x => x.RowId);
                action.ToTable($"{builder.GetTableName()}_{nameof(Audit).Pluralize()}");
                action.HasKey(x => x.Id);
                action.Property(x => x.Id).ValueGeneratedOnAdd();
                action.Property(x => x.Additional).HasJsonConversion();
                action.Property(x => x.Changes).HasJsonConversion();
            });
            return builder;
        }
    }
}