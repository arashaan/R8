using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace R8.Lib
{
    public static class EnumReflections
    {
        /// <summary>
        /// Returns specific <see cref="Attribute"/> from given enum member.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="enumMember"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>(this Enum enumMember)
          where TAttribute : Attribute
        {
            if (enumMember == null)
                throw new ArgumentNullException(nameof(enumMember));

            var enumType = enumMember.GetType();
            var enumName = Enum.GetName(enumType, enumMember);
            var memberInfos = enumType.GetMember(enumName);
            var memberInfo = memberInfos.Single();
            return memberInfo.GetCustomAttribute<TAttribute>();
        }

        /// <summary>
        /// Returns a <see cref="Enum"/> based on given string.
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static object ToEnum(Type enumType, string value)
        {
            if (enumType == null) throw new ArgumentNullException(nameof(enumType));
            if (value == null) throw new ArgumentNullException(nameof(value));
            return Enum.Parse(enumType, value, true);
        }

        /// <summary>
        /// Returns a <see cref="Enum"/> based on given string.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static TEnum ToEnum<TEnum>(this string value) where TEnum : Enum
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return (TEnum)ToEnum(typeof(TEnum), value);
        }

        /// <summary>
        /// Returns <see cref="DescriptionAttribute"/> value from given member.
        /// </summary>
        /// <param name="value">An enum member that should be checked for <see cref="DescriptionAttribute"/> value</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="string"/> value.</returns>
        public static string GetDescription(this Enum value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return value.GetAttribute<DescriptionAttribute>()?.Description ?? Enum.GetName(value.GetType(), value);
        }

        /// <summary>
        /// Returns <see cref="DisplayAttribute"/> value from given member.
        /// </summary>
        /// <param name="value">An enum member that should be checked for <see cref="DisplayAttribute"/> value</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="string"/> value.</returns>
        public static string GetDisplayName(this Enum value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return value.GetAttribute<DisplayAttribute>()?.GetName() ?? Enum.GetName(value.GetType(), value);
        }

        /// <summary>
        /// Create a <see cref="Dictionary{TKey,TValue}"/> from given enum type.
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static Dictionary<int, string> ToDictionary(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!type.IsEnum)
                throw new ArgumentException(nameof(type));

            var values = Enum.GetValues(type).Cast<object>().ToList();
            return values.Any()
                ? values.ToDictionary(x => (int)x, x => x.ToString())
                : new Dictionary<int, string>();
        }

        /// <summary>
        /// Creates an array from given <see cref="Enum{T}"/>
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>An array that contains the elements from the input sequence.</returns>
        public static int[] ToArray(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!type.IsEnum)
                throw new ArgumentException(nameof(type));

            return Enum
                .GetValues(type)
                .Cast<int>()
                .ToArray();
        }

        /// <summary>
        /// Creates a <see cref="List{T}"/> from given <see cref="Enum{T}"/>
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>A <see cref="List{T}"/> that contains elements from the input sequence.</returns>
        public static List<int> ToList(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!type.IsEnum)
                throw new ArgumentException(nameof(type));

            return ToArray(type).ToList();
        }
    }
}