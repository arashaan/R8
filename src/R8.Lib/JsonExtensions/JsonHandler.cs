using System;
using System.Linq;
using System.Linq.Expressions;

namespace R8.Lib.JsonExtensions
{
    /// <summary>
    /// An static class to get some information about specified type.
    /// </summary>
    /// <typeparam name="T">A generic type of class.</typeparam>
    public static class JsonHandler<T> where T : class
    {
        /// <summary>
        /// Represents name of property.
        /// </summary>
        /// <param name="expression">An <see cref="Expression{TDelegate}"/> value that representing specific property.</param>
        /// <returns>An <see cref="string"/> value.</returns>
        /// <remarks>If <c>JsonPropertyAttribute</c> is null returns Property's name, otherwise it will return <c>JsonPropertyAttribute</c> value.</remarks>
        public static string GetProperty(Expression<Func<T, object>> expression)
        {
            var propertyName = expression.GetPropertyInfo().Name;
            var propertyInfo = typeof(T).GetProperties().ToList().First(x => x.Name.Equals(propertyName));
            return propertyInfo.GetJsonName();
        }
    }
}