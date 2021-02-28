using Humanizer;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using System;
using System.Reflection;

namespace R8.EntityFrameworkCore
{
    public static class Extensions
    {
        /// <summary>
        /// Returns name of table that given <see cref="EntityEntry"/> affecting on.
        /// </summary>
        /// <param name="entry">An <see cref="EntityEntry"/> object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>Name of a Database table.</returns>
        public static string GetTableName(this EntityEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));
            return entry.Context.GetTableName(entry.Entity.GetType());
        }

        /// <summary>
        /// Returns name of table that given <see cref="Type"/> of entity affecting on.
        /// </summary>
        /// <typeparam name="T">A entity type.</typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>Name of a Database table.</returns>
        public static string GetTableName<T>(this EntityTypeBuilder<T> builder) where T : class
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.Metadata.AsEntityType().GetTableName();
        }

        /// <summary>
        /// Returns name of table that given <see cref="Type"/> of entity affecting on.
        /// </summary>
        /// <typeparam name="TDbContext">A derived type of <see cref="DbContext"/>.</typeparam>
        /// <param name="context">A <see cref="DbContext"/> object.</param>
        /// <param name="type">Specific CLR <see cref="Type"/> of an entity.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>Name of a Database table.</returns>
        public static string GetTableName<TDbContext>(this TDbContext context, Type type) where TDbContext : DbContext
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (type == null) throw new ArgumentNullException(nameof(type));

            var entityType = context.Model.FindEntityType(type);
            return entityType.GetTableName();
        }

        /// <summary>
        /// Returns name of table based on adapted strategy.
        /// </summary>
        /// <param name="entityType">A <see cref="Type"/> object.</param>
        /// <exception cref="InvalidCastException"></exception>
        /// <returns>A <see cref="string"/> as name of the Table.</returns>
        public static string GetTableName(Type entityType)
        {
            var hasImplementation = entityType.GetInterface(nameof(IEntityBase));
            if (hasImplementation != null)
                return entityType.Name.Pluralize();

            throw new InvalidCastException($"Given type must be implemented from {typeof(IEntityBase)}.");
        }

        /// <summary>
        /// Returns name of table based on adapted strategy.
        /// </summary>
        /// <typeparam name="TEntity">A derived type from <see cref="IEntityBase"/>.</typeparam>
        /// <param name="entity">A <see cref="TEntity"/> object that representing entity.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="string"/> as name of the Table.</returns>
        public static string GetTableName<TEntity>(this TEntity entity) where TEntity : class, IEntityBase
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return GetTableName<TEntity>();
        }

        /// <summary>
        /// Returns name of table based on adapted strategy.
        /// </summary>
        /// <typeparam name="TEntity">A derived type from <see cref="IEntityBase"/>.</typeparam>
        /// <returns>A <see cref="string"/> as name of the Table.</returns>
        public static string GetTableName<TEntity>() where TEntity : class, IEntityBase
        {
            return GetTableName(typeof(TEntity));
        }
    }
}