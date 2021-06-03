using Humanizer;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
        /// Returns arguments used in an chained queryable.
        /// </summary>
        /// <param name="queryable">Expression you want to decompile</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="List{T}"/> object</returns>
        public static IEnumerable<Expression> GetExpressionTree(this IQueryable queryable)
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            var tempExpression = queryable.Expression;
            var result = new List<Expression>();
            while (tempExpression != null)
            {
                if (!(tempExpression is MethodCallExpression methodCall) ||
                    tempExpression.NodeType != ExpressionType.Call)
                {
                    if (tempExpression is QueryRootExpression queryExpression)
                    {
                        result.Add(queryExpression);
                        tempExpression = null;
                    }

                    break;
                }

                var arguments = methodCall.Arguments;
                // var name = methodCall.Method.Name; // for development
                //
                if (arguments.Count == 0)
                    break;

                var next = arguments[0];
                // var nextType = next.GetType(); // for development
                if (arguments.Count > 1)
                {
                    var current = arguments[1];
                    var lambda = current.GetLambdaOrNull();
                    if (lambda != null)
                        result.Add(methodCall);
                }
                else
                {
                    result.Add(methodCall);
                }

                tempExpression = next;
            }
            result?.Reverse();
            return result;
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
        public static string NormalizeTableName(Type entityType)
        {
            //var hasImplementation = entityType.IsAssignableFrom(typeof(IEntityBaseIdentifier));
            //return hasImplementation ? entityType.Name.Pluralize() : entityType.Name;
            return entityType.Name.Pluralize();
        }

        /// <summary>
        /// Returns name of table based on adapted strategy.
        /// </summary>
        /// <typeparam name="TEntity">A derived type from <see cref="IEntityBase"/>.</typeparam>
        /// <param name="entity">A <see cref="TEntity"/> object that representing entity.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="string"/> as name of the Table.</returns>
        public static string NormalizeTableName<TEntity>(this TEntity entity) where TEntity : class, IEntityBaseIdentifier
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return NormalizeTableName<TEntity>();
        }

        /// <summary>
        /// Returns name of table based on adapted strategy.
        /// </summary>
        /// <typeparam name="TEntity">A derived type from <see cref="IEntityBase"/>.</typeparam>
        /// <returns>A <see cref="string"/> as name of the Table.</returns>
        public static string NormalizeTableName<TEntity>() where TEntity : class, IEntityBaseIdentifier
        {
            return NormalizeTableName(typeof(TEntity));
        }
    }
}