using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;

namespace R8.Lib
{
    public static class TypeReflections
    {
        /// <summary>
        /// Returns an array of types can be assigned to given type.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="type"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Type[] GetTypesAssignableFrom(this Assembly assembly, Type type)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            List<Type> assemblyTypes;
            try
            {
                assemblyTypes = assembly.DefinedTypes.Select(x => x.AsType()).ToList();
            }
            catch (ReflectionTypeLoadException ex)
            {
                assemblyTypes = ex.Types.Where(t => t != null).Select(IntrospectionExtensions.GetTypeInfo).Select(x => x.AsType()).ToList();
            }

            return assemblyTypes.Where(type.IsAssignableFrom).ToArray();
        }

        /// <summary>
        /// Returns an array of types that deriven from given type.
        ///
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Type[] GetTypesAssignableFrom<TType>(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            return assembly.GetTypesAssignableFrom(typeof(TType));
        }

        /// <summary>
        /// Determines whether given type is a number type.
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>If given type is number returns true, otherwise false.</returns>
        public static bool IsNumeric(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;

                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.String:
                default:
                    return false;
            }
        }

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

            return (from @interface in interfaces
                    let hasGeneric = @interface.IsGenericType && @interface.GetGenericTypeDefinition() == genericType
                    where hasGeneric
                    select @interface.GetGenericArguments()
                into genericTypes
                    where genericTypes.Any()
                    select genericTypes[0]).FirstOrDefault();
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

            if (propertyType == typeof(decimal))
            {
                var isDecimal = decimal.TryParse(value, NumberStyles.Any, new CultureInfo("en-US"),
                    out var decimalDetail);
                if (!isDecimal)
                    return false;

                output = decimalDetail;
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

            if (propertyType == typeof(IPAddress))
            {
                var isIpAddress = IPAddress.TryParse(value, out var ipAddress);
                if (!isIpAddress)
                    return false;

                output = ipAddress;
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

        /// <summary>
        /// Checks if given <see cref="Type"/> has given base type.
        /// </summary>
        /// <param name="type">A <see cref="Type"/> that need to be checked to find base type.</param>
        /// <param name="baseType">A <see cref="Type"/> that expect to being found as base type.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="bool"/> value.</returns>
        public static bool HasRootType(this Type type, Type baseType)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (baseType == null) throw new ArgumentNullException(nameof(baseType));
            var rootTypes = type.GetTypesToRoot();

            var hasBaseType = rootTypes.Any(x => x == baseType);
            return hasBaseType;
        }

        /// <summary>
        /// Returns list of <see cref="Type"/> from given type to the first abstract type.
        /// </summary>
        /// <param name="type">A <see cref="Type"/> that should be checked for chain root.</param>
        /// <param name="stopOnAbstract"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="List{T}"/> object.</returns>
        /// <remarks>Using this method may impact your performance.</remarks>
        public static IEnumerable<Type> GetTypesToRoot(this Type type, bool stopOnAbstract = true)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var nestedTypes = new List<Type>();
            var found = false;
            do
            {
                nestedTypes.Add(type);

                if ((stopOnAbstract && type.IsAbstract) || type.BaseType == null)
                    found = true;

                type = type.BaseType;
            } while (!found);
            return nestedTypes;
        }
    }
}