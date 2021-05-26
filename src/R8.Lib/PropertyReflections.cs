using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace R8.Lib
{
    public static class PropertyReflections
    {
        /// <summary>
        /// Returns name of member from given expression.
        /// </summary>
        /// <typeparam name="T">A generic type</typeparam>
        /// <param name="expression">An <see cref="Expression{TDelegate}"/> that need to be checked for member name.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidExpressionException"></exception>
        /// <returns>A <see cref="string"/> value</returns>
        public static string GetMemberName<T>(this Expression<T> expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return expression.Body switch
            {
                MemberExpression m => m.Member.Name,
                UnaryExpression { Operand: MemberExpression m } => m.Member.Name,
                _ => throw new InvalidExpressionException(expression.GetType().ToString())
            };
        }

        /// <summary>
        /// Returns a <see cref="Dictionary{TKey,TValue}"/> from properties in given source.
        /// </summary>
        /// <typeparam name="TModel">A generic type for source.</typeparam>
        /// <param name="source">An object to get properties list.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> object</returns>
        public static Dictionary<string, object> ToDictionary<TModel>(this TModel source) where TModel : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var dictionary = new Dictionary<string, object>();
            foreach (var property in source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = property.GetValue(source);
                if (value is DateTime)
                    value = $"{value:dd/MM/yyyy}";

                dictionary.Add(property.Name, value);
            }
            return dictionary;
        }

        /// <summary>
        /// Returns <see cref="PropertyInfo"/> from expression.
        /// </summary>
        /// <typeparam name="TModel">A object generic type that containing specific property.</typeparam>
        /// <param name="expression">An <see cref="Expression{TDelegate}"/> to get given property from given model</param>
        /// <returns>An <see cref="PropertyInfo"/> object</returns>
        public static MemberInfo GetPropertyInfo<TModel>(this Expression<Func<TModel, object>> expression)
        {
            var memberExpression = expression.GetMemberExpression();
            return memberExpression.Member;
        }

        /// <summary>
        /// Returns <see cref="PropertyInfo"/> from expression.
        /// </summary>
        /// <param name="expression">An <see cref="Expression{TDelegate}"/> to get given property from given model</param>
        /// <returns>An <see cref="PropertyInfo"/> object</returns>
        public static PropertyInfo GetPropertyInfo(this Expression expression)
        {
            if (!(expression is LambdaExpression lambda))
                return null;

            var memberExpression = lambda.GetMemberExpression();
            return memberExpression.Member as PropertyInfo;
        }

        /// <summary>
        /// Returns <see cref="DisplayAttribute"/> value from given member.
        /// </summary>
        /// <typeparam name="TMember">Type of given member.</typeparam>
        /// <param name="member">A generic object that should be checked for <see cref="DisplayAttribute"/> value</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="string"/> value.</returns>
        public static string GetDisplayName<TMember>(this TMember member) where TMember : MemberInfo
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            return member.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? member.Name;
        }
    }
}