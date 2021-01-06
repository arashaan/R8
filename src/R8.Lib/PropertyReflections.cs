using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace R8.Lib
{
    public static class PropertyReflections
    {
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