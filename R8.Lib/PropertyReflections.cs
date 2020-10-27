using R8.Lib.Attributes;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace R8.Lib
{
    public static class PropertyReflections
    {
        public static object GetMemberValue(this MemberExpression expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            return expression.Expression switch
            {
                // expression is ConstantExpression or FieldExpression
                ConstantExpression constantExpression => (constantExpression.Value).GetType()
                .GetField(expression.Member.Name)
                .GetValue(constantExpression.Value),
                MemberExpression memberExpression => GetMemberValue(memberExpression),
                _ => throw new NotImplementedException()
            };
        }

        public static object GetExpressionValue(this Expression expression)
        {
            var objectMember = Expression.Convert(expression, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var rightValue = getterLambda.Compile()().ToString();
            return rightValue;
        }

        public static MemberExpression GetMemberExpression(this LambdaExpression lambdaExpression)
        {
            if (lambdaExpression == null) throw new ArgumentNullException(nameof(lambdaExpression));
            var memberExpression = lambdaExpression.Body is UnaryExpression unaryExpression
              ? (MemberExpression)unaryExpression.Operand
              : (MemberExpression)lambdaExpression.Body;

            return memberExpression;
        }

        public static TAttribute GetPropertyAttribute<TModel, TAttribute>(
            this Expression<Func<TModel, object>> expression) where TAttribute : Attribute
        {
            var propertyInfo = expression.GetProperty();
            return propertyInfo?.GetCustomAttributes(false).OfType<TAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// Cast object to Dictionary
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(this object source)
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

        public static string GetMemberName<T>(this Expression<T> expression)
        {
            return expression.Body switch
            {
                MemberExpression m => m.Member.Name,
                UnaryExpression u when u.Operand is MemberExpression m => m.Member.Name,
                _ => throw new NotImplementedException(expression.GetType().ToString())
            };
        }

        public static List<Type> GetBaseTypes(this Type type)
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

        public static List<PropertyInfo> SortByOrder(this Type type)
        {
            if (type == null)
                return default;

            var types = type.GetBaseTypes();
            types.Reverse();

            if (types?.Any() != true)
                return default;

            var result = new List<PropertyInfo>();
            var sortedProperties = from _type in types
                                   select _type.GetTypeInfo().DeclaredProperties.ToList()
                into properties
                                   where properties?.Any() == true
                                   select properties.ToDictionary(x => x, x => x.GetCustomAttribute<OrderAttribute>()?.X ?? 100)
                into sortDic
                                   select sortDic.OrderBy(x => x.Value).Select(x => x.Key).ToList();
            foreach (var sortedProperty in sortedProperties)
            {
                result.AddRange(sortedProperty);
            }

            return result;
        }

        public static Dictionary<string, string> Sort(this Dictionary<string, string> propertiesDictionary, Type modelType)
        {
            if (!propertiesDictionary.Any())
                return propertiesDictionary;

            if (modelType == null)
                return propertiesDictionary;

            var sorted = modelType.SortByOrder();
            if (sorted?.Any() != true)
                return propertiesDictionary;

            var properties = modelType.GetPublicProperties();
            if (properties?.Any() != true)
                return propertiesDictionary;

            var result = sorted
                .Where(x => propertiesDictionary.Any(c => c.Key == x.GetDisplayName()))
                .ToDictionary(x => x.GetDisplayName(), x => propertiesDictionary.FirstOrDefault(c => c.Key == x.GetDisplayName()).Value);
            return result;
        }

        public static TAttribute GetPropertyAttribute<TAttribute>(
            this PropertyInfo property) where TAttribute : Attribute
        {
            return property?.GetCustomAttributes(false).OfType<TAttribute>().FirstOrDefault();
        }

        public static object[] GetPropertyAttributes<TModel>(
            this Expression<Func<TModel, object>> expression)
        {
            var propertyInfo = expression.GetProperty();
            return propertyInfo?.GetCustomAttributes(false);
        }

        public static List<PropertyInfo> GetPublicProperties(this Type modelType)
        {
            return modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
        }

        public static List<PropertyInfo> GetPublicProperties<TModel>(this TModel model) where TModel : class
        {
            return model.GetType().GetPublicProperties();
        }

        public static PropertyInfo GetProperty<TModel>(this TModel model, string propertyName) where TModel : class
        {
            var type = model.GetType().GetProperty(propertyName);
            return type;
        }

        public static PropertyInfo GetProperty<TSource, TModel>(this Expression<Func<TSource, List<TModel>>> expression)
        {
            var lambdaExpression = expression as LambdaExpression;
            var memberExpression = lambdaExpression.GetMemberExpression();

            return memberExpression.Member as PropertyInfo;
        }

        public static PropertyInfo GetProperty<TSource, TModel>(this Expression<Func<TSource, TModel>> expression)
        {
            var lambdaExpression = expression as LambdaExpression;
            var memberExpression = lambdaExpression.GetMemberExpression();

            return memberExpression.Member as PropertyInfo;
        }

        public static PropertyInfo GetProperty<TModel>(this Expression<Func<TModel, object>> expression)
        {
            return expression.GetProperty<TModel, object>();
        }

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

        public static string GetDisplayName(this MemberInfo member)
        {
            var display = member.GetCustomAttribute<DisplayAttribute>();
            return display != null
              ? display.GetName()
              : member.Name;
        }

        public static string GetDisplayName(this PropertyInfo property)
        {
            var display = property.GetCustomAttribute<DisplayAttribute>();
            return display != null
                ? display.GetName()
                : property.Name;
        }

        public static string GetDisplayName<TModel>(this Expression<Func<TModel, object>> property)
        {
            var display = property.GetPropertyAttribute<TModel, DisplayAttribute>();
            return display != null
                ? display.GetName()
                : property.Name;
        }

        /// <summary>
        /// Return arguments used in an chained expression ( commonly IQueryable )
        /// </summary>
        /// <param name="expression">Expression you want to decompile</param>
        /// <param name="currentList">Leave it blank</param>
        /// <returns></returns>
        public static List<ExpressionArgument> GetArguments([NotNull] this Expression expression, List<ExpressionArgument> currentList = null)
        {
            if (currentList == null)
                currentList = new List<ExpressionArgument>();

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
            var lambda = current.GetLambdaOrNull();
            if (lambda != null)
                currentList.Add(new ExpressionArgument(currentName, lambda));

            var finalList = next.GetArguments(currentList);
            finalList?.Reverse();
            return finalList;
        }

        public static LambdaExpression GetLambdaOrNull(this Expression expression)
          => expression is LambdaExpression lambda
            ? lambda
            : expression is UnaryExpression unary && expression.NodeType == ExpressionType.Quote
              ? (LambdaExpression)unary.Operand
              : null;
    }
}