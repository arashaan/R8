using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using R8.Lib.Validatable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace R8.EntityFrameworkCore
{
    public static class DbContextBaseExtensions
    {
        /// <summary>
        /// Sets Auto-Increment for specific Property in given types.
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="property"></param>
        /// <param name="start"></param>
        /// <param name="entityTypes"></param>
        /// <returns></returns>
        public static ModelBuilder HasAutoIncrementColumn(this ModelBuilder modelBuilder, string property, long start, params Type[] entityTypes)
        {
            if (entityTypes == null || !entityTypes.Any())
                return modelBuilder;

            foreach (var entityType in entityTypes)
            {
                var tableName = entityType.Name;
                modelBuilder.HasSequence<long>($"{tableName}_seq", schema: "dbo")
                    .StartsAt(start)
                    .IncrementsBy(1);

                modelBuilder.Entity(entityType)
                    .Property(property)
                    .HasDefaultValueSql($"NEXT VALUE FOR dbo.{tableName}_seq");
            }

            return modelBuilder;
        }

        /// <summary>
        /// Sets <see cref="EntityBase.IsDeleted"/> value to <c>false</c> in <see cref="EntityBase"/> typed class, based on <see cref="EntityBase"/> strategy.
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="bool"/> that indicates operations if successfully done.</returns>
        /// <remarks>In <see cref="EntityBase"/> strategy, any rows will not be deleted, Only <see cref="EntityBase.IsDeleted"/> will be set to <c>true</c>.</remarks>
        public static EntityEntry UnRemove<TDbContext, TSource>(this TDbContext dbContext, TSource entity) where TDbContext : DbContext where TSource : IEntityBase
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            entity.IsDeleted = false;
            var entry = dbContext.Update(entity);
            return entry;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="changesCount"></param>
        /// <param name="entries"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static bool NeedSave<TDbContext>(this TDbContext dbContext, out int changesCount, out List<EntityEntry> entries) where TDbContext : DbContext
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            entries = null;
            changesCount = 0;
            var allEntries = dbContext.ChangeTracker.Entries().ToList();
            if (!allEntries.Any())
                return false;

            entries = allEntries
                .Where(x => x.State != EntityState.Detached && x.State != EntityState.Unchanged)
                .ToList();
            changesCount = entries?.Count ?? 0;
            return changesCount != 0;
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="responses"></param>
        ///// <param name="errors"></param>
        ///// <exception cref="ArgumentNullException"></exception>
        ///// <returns></returns>
        //public static bool TryValidate<T>(this List<T> responses, out ValidatableResultCollection errors) where T : IResponseBaseDatabase
        //{
        //    if (responses == null) throw new ArgumentNullException(nameof(responses));

        //    errors = new ValidatableResultCollection();
        //    if (responses == null || responses.Count == 0)
        //        return false;

        //    var newList = responses
        //        .Cast<IResponseBaseDatabase>()
        //        .Where(responseBaseDatabase => responseBaseDatabase.GetType()
        //            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        //            .Any(propertyInfo => propertyInfo.Name == nameof(WrapperBase<object, string>.Result) &&
        //                                 propertyInfo.GetValue(responseBaseDatabase) is IEntityBase))
        //        .ToList();
        //    if (newList.Count == 0)
        //        return false;

        //    foreach (var value in from response in newList
        //                          let prop = response.GetType()
        //                              .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        //                              .FirstOrDefault(propertyInfo => propertyInfo.Name == nameof(WrapperBase<object, string>.Result) &&
        //                                                    propertyInfo.GetValue(response) is IEntityBase)
        //                          select (IEntityBase)prop.GetValue(response)
        //        into value
        //                          where value != null
        //                          select value)
        //    {
        //        value.TryValidate(out var tempErrors);
        //        errors.AddRange(tempErrors);
        //    }

        //    return errors?.Count == 0;
        //}
    }
}