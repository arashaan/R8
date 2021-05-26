using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace R8.Lib
{
    public static class Enum<T> where T : struct, Enum
    {
        public static int Count
        {
            get
            {
                if (!typeof(T).IsEnum)
                    throw new ArgumentException("T must be an enumerated type");

                return Enum.GetNames(typeof(T)).Length;
            }
        }

        public static bool TryParseFromKebabCase(string enumValue, out T en)
        {
            en = (T)(object)0;
            if (string.IsNullOrEmpty(enumValue))
                return false;

            try
            {
                en = enumValue
                    .FromKebabCase()
                    .Replace(" ", "")
                    .ToEnum<T>();
                return true;
            }
            catch
            {
                en = (T)(object)0;
                return false;
            }
        }

        /// <summary>
        /// Parses a kebab-case'd parameter to given type of <see cref="Enum"/>.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static T? ParseFromKebabCase(string enumValue)
        {
            if (enumValue == null)
                throw new ArgumentNullException(nameof(enumValue));

            return enumValue
                .FromKebabCase()
                .Humanize(true)
                .ToEnum<T>();
        }

        /// <summary>
        /// Creates an array from given <see cref="Enum{T}"/>
        /// </summary>
        /// <returns>An array that contains the elements from the input sequence.</returns>
        public static int[] ToArray()
        {
            return EnumReflections.ToArray(typeof(T));
        }

        /// <summary>
        /// Creates a <see cref="List{T}"/> from given <see cref="Enum{T}"/>
        /// </summary>
        /// <returns>A <see cref="List{T}"/> that contains elements from the input sequence.</returns>
        public static List<int> ToList()
        {
            return ToArray().ToList();
        }

        public static List<FieldInfo> GetFields()
        {
            return typeof(T)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .ToList();
        }

        public static Dictionary<int, string> ToDictionary()
        {
            return EnumReflections.ToDictionary(typeof(T));
        }

        public static List<T> ToListOrderBy(params T[] array)
        {
            var result = ToList()
                .OrderBy(x => (T)(object)x, array)
                .Select(x => (T)(object)x)
                .ToList();
            return result;
        }
    }
}