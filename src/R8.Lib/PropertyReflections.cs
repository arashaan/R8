using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace R8.Lib
{
    public static class PropertyReflections
    {
        /// <summary>
        /// Returns underlying type under nullable type or generic type.
        /// </summary>
        /// <param name="type">A <see cref="Type"/>.</param>
        /// <param name="ignoreNullability">Checks if need to be bypassed nullable types and get underlying type.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="Type"/>.</returns>
        public static Type GetUnderlyingType(this Type type, bool ignoreNullability = true)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var resultType = type.GetEnumerableUnderlyingType() ?? type;
            if (ignoreNullability)
                resultType = Nullable.GetUnderlyingType(resultType) ?? resultType;

            return resultType;
        }

        /// <summary>
        /// Returns underlying type under a enumerable.
        /// </summary>
        /// <param name="type">A <see cref="Type"/>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="Type"/>.</returns>
        public static Type? GetEnumerableUnderlyingType(this Type type)
        {
            return type.GetGenericUnderlyingType(typeof(IList<>));
        }

        /// <summary>
        /// Returns underlying type under a generic type.
        /// </summary>
        /// <param name="type">A <see cref="Type"/>.</param>
        /// <param name="genericType"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="Type"/>.</returns>
        public static Type? GetGenericUnderlyingType(this Type type, Type genericType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var interfaces = type.GetInterfaces();
            if (!interfaces.Any())
                return null;

            foreach (var @interface in interfaces)
            {
                var hasGeneric = @interface.IsGenericType && @interface.GetGenericTypeDefinition() == genericType;
                if (!hasGeneric)
                    continue;

                var genericTypes = @interface.GetGenericArguments();
                if (genericTypes.Any())
                    return genericTypes[0];
            }

            return null;
        }

        /// <summary>
        /// Checks if given value is capable to keep in the given type and returns value with given type.
        /// </summary>
        /// <param name="type">A <see cref="Type"/> to check value type.</param>
        /// <param name="values">An <see cref="IEnumerable{T}"/> of <see cref="string"/> that representing list of values to be converted.</param>
        /// <param name="output">An <see cref="object"/> that representing output value in property type.</param>
        /// <returns>A <see cref="bool"/> that should be true when value is in given type, otherwise false.</returns>
        /// <remarks><c>output</c> parameter type will be same as given <see cref="Type"/>, if parse completes successfully.</remarks>
        public static bool TryParse(this Type type, IEnumerable<string> values, out object output)
        {
            output = null;

            if (type == null)
                return false;
            if (values == null)
                return false;

            var underlyingType = type.GetUnderlyingType();
            if (!(Activator.CreateInstance(typeof(List<>).MakeGenericType(underlyingType)) is IList list))
                return false;

            foreach (var value in values)
            {
                var validValue = underlyingType.TryParse(value, out var tempValue);
                if (!validValue)
                    continue;

                list.Add(tempValue);
            }

            if (list.Count == 0)
                return false;

            if (!type.IsArray)
            {
                output = list;
                return true;
            }

            var array = Array.CreateInstance(underlyingType, list.Count);
            for (var arrayIndex = 0; arrayIndex < list.Count; arrayIndex++)
                array.SetValue(list[arrayIndex], arrayIndex);

            output = array;
            return true;
        }

        /// <summary>
        /// Checks if given value is capable to keep in the given type and returns value with given type.
        /// </summary>
        /// <param name="type">A <see cref="Type"/> to check value type.</param>
        /// <param name="value">A <see cref="string"/> that representing value to be converted.</param>
        /// <param name="output">An <see cref="object"/> that representing output value in property type.</param>
        /// <returns>A <see cref="bool"/> that should be true when value is in given type, otherwise false.</returns>
        /// <remarks><c>output</c> parameter type will be same as given <see cref="Type"/>, if parse completes successfully.</remarks>
        public static bool TryParse(this Type type, string value, out object output)
        {
            output = null;
            if (type == null)
                return false;

            var isNullableType = Nullable.GetUnderlyingType(type) != null;
            if (!isNullableType && type != typeof(string))
            {
                if (string.IsNullOrEmpty(value))
                {
                    output = null;
                    return false;
                }
            }

            var propertyType = type.GetUnderlyingType();
            if (propertyType.IsEnum)
            {
                var isEnum = Enum.TryParse(propertyType, value, true, out var enumDetail);
                if (!isEnum)
                    return false;

                output = enumDetail;
                return true;
            }

            if (propertyType == typeof(int))
            {
                var isInt = int.TryParse(value, out var intDetail);
                if (!isInt)
                    return false;

                output = intDetail;
                return true;
            }

            if (propertyType == typeof(double))
            {
                var isDouble = double.TryParse(value, NumberStyles.Any, new CultureInfo("en-US"),
                    out var doubleDetail);
                if (!isDouble)
                    return false;

                output = doubleDetail;
                return true;
            }

            if (propertyType == typeof(long))
            {
                var isLong = long.TryParse(value, out var longDetail);
                if (!isLong)
                    return false;

                output = longDetail;
                return true;
            }

            if (propertyType == typeof(string))
            {
                if (!string.IsNullOrEmpty(value))
                    output = value;

                return true;
            }

            if (propertyType == typeof(DateTime))
            {
                var isYearDateTime = DateTime.TryParseExact(value, "yyyy", new CultureInfo("en-US"),
                    DateTimeStyles.AdjustToUniversal, out var year);
                if (!isYearDateTime)
                {
                    var isDateTime = DateTime.TryParse(value, new CultureInfo("en-US"),
                        DateTimeStyles.AdjustToUniversal, out var dateTimeDetail);
                    if (!isDateTime)
                        return false;

                    output = dateTimeDetail;
                    return true;
                }

                output = year;
                return true;
            }

            if (propertyType == typeof(bool))
            {
                if (!value.Contains("on", StringComparison.InvariantCultureIgnoreCase) &&
                    !value.Contains("off", StringComparison.InvariantCultureIgnoreCase) &&
                    !value.Contains("true", StringComparison.InvariantCultureIgnoreCase) &&
                    !value.Contains("false", StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }

                output = value.Equals("on", StringComparison.InvariantCultureIgnoreCase) ||
                         value.Equals("true", StringComparison.InvariantCultureIgnoreCase);
                return true;
            }

            return false;
        }

        public static object GetExpressionValue(this Expression expression)
        {
            var objectMember = Expression.Convert(expression, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            return getterLambda.Compile()();
        }

        public static MemberExpression GetMemberExpression(this LambdaExpression lambdaExpression)
        {
            if (lambdaExpression == null) throw new ArgumentNullException(nameof(lambdaExpression));
            return lambdaExpression.Body is UnaryExpression unaryExpression
              ? (MemberExpression)unaryExpression.Operand
              : (MemberExpression)lambdaExpression.Body;
        }

        /// <summary>
        /// Returns a <see cref="Dictionary{TKey,TValue}"/> from properties in given source.
        /// </summary>
        /// <typeparam name="TModel">A generic type for source.</typeparam>
        /// <param name="source">An object to get properties list.</param>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> object</returns>
        public static Dictionary<string, object> ToDictionary<TModel>(this TModel source) where TModel : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var dictionary = new Dictionary<string, object>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
            {
                var value = property.GetValue(source);
                if (value is DateTime)
                    value = $"{value:dd/MM/yyyy}";

                dictionary.Add(property.Name, value);
            }
            return dictionary;
        }

        /// <summary>
        /// Returns name of member from given expression.
        /// </summary>
        /// <typeparam name="T">A generic type</typeparam>
        /// <param name="expression">An <see cref="Expression{TDelegate}"/> that need to be checked for member name.</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns>A <see cref="string"/> value</returns>
        public static string GetMemberName<T>(this Expression<T> expression) => expression.Body switch
        {
            MemberExpression m => m.Member.Name,
            UnaryExpression u when u.Operand is MemberExpression m => m.Member.Name,
            _ => throw new NotImplementedException(expression.GetType().ToString())
        };

        /// <summary>
        /// Returns list of <see cref="Type"/> from given type to the first abstract type.
        /// </summary>
        /// <param name="type">A <see cref="Type"/> that should be checked for chain root.</param>
        /// <returns>A <see cref="List{T}"/> object.</returns>
        public static List<Type> GetTypesToRoot(this Type type)
        {
            var nestedTypes = new List<Type>();
            var found = false;
            do
            {
                nestedTypes.Add(type);

                if (type.IsAbstract || type.BaseType == null)
                    found = true;

                type = type.BaseType;
            } while (!found);
            return nestedTypes;
        }

        /// <summary>
        /// Returns a list of public <see cref="PropertyInfo"/> from given model type.
        /// </summary>
        /// <param name="modelType">A <see cref="Type"/> that need to be checked for properties</param>
        /// <returns>An <see cref="List{T}"/> object</returns>
        public static List<PropertyInfo> GetPublicProperties(this Type modelType)
        {
            return modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
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
        /// Checks if given <see cref="Type"/> has given base type.
        /// </summary>
        /// <param name="type">A <see cref="Type"/> that need to be checked to find base type.</param>
        /// <param name="baseType">A <see cref="Type"/> that expect to being found as base type.</param>
        /// <returns>A <see cref="bool"/> value.</returns>
        public static bool HasBaseType(this Type type, Type baseType)
        {
            try
            {
                if (type != baseType)
                    return type.BaseType == baseType || HasBaseType(type.BaseType, baseType);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns <see cref="DisplayAttribute"/> value from given member.
        /// </summary>
        /// <typeparam name="TMember">Type of given member.</typeparam>
        /// <param name="member">A generic object that should be checked for <see cref="DisplayAttribute"/> value</param>
        /// <returns>A <see cref="string"/> value.</returns>
        public static string GetDisplayName<TMember>(this TMember member) where TMember : MemberInfo
        {
            var display = member.GetCustomAttribute<DisplayAttribute>();
            return display != null
              ? display.GetName()
              : member.Name;
        }

        private static List<ExpressionArgument> GetArguments(this Expression expression, List<ExpressionArgument> currentList)
        {
            currentList ??= new List<ExpressionArgument>();
            if (expression == null)
                return currentList;

            if (!(expression is MethodCallExpression methodCall) || expression.NodeType != ExpressionType.Call)
                return currentList;

            var arguments = methodCall.Arguments;
            var currentName = methodCall.Method.Name;

            if (arguments.Count == 1)
                return currentList;

            var next = arguments[0];
            var current = arguments[1];
            var validLambda = current.TryGetLambda(out var lambda);
            if (validLambda)
                currentList.Add(new ExpressionArgument(currentName, lambda));

            var finalList = next.GetArguments(currentList);
            finalList?.Reverse();
            return finalList;
        }

        /// <summary>
        /// Returns arguments used in an chained expression.
        /// </summary>
        /// <param name="expression">Expression you want to decompile</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="List{T}"/> object</returns>
        /// <remarks>commonly should be used for IQueryable.</remarks>
        public static List<ExpressionArgument> GetArguments(this Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return expression.GetArguments(new List<ExpressionArgument>());
        }

        /// <summary>
        /// Try to gets <see cref="LambdaExpression"/> from given expression.
        /// </summary>
        /// <param name="expression">An <see cref="Expression"/> that should be checked for lambda expression.</param>
        /// <param name="output">The output lambda expression</param>
        /// <returns>A <see cref="bool"/> value that representing succession.</returns>
        /// <remarks>If true, output will be a type of <see cref="LambdaExpression"/>, otherwise will be null.</remarks>
        public static bool TryGetLambda(this Expression expression, out LambdaExpression? output)
        {
            switch (expression)
            {
                case LambdaExpression lambda:
                    output = lambda;
                    return true;

                case UnaryExpression unary when expression.NodeType == ExpressionType.Quote:
                    output = (LambdaExpression)unary.Operand;
                    return true;

                default:
                    output = null;
                    return false;
            }
        }
    }
}