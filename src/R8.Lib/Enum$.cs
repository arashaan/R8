using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        public static bool TryFromKebabCase(string enumValue, out T en)
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

        public static T FromKebabCase(string enumValue)
        {
            if (enumValue == null)
                throw new ArgumentNullException(nameof(enumValue));

            return enumValue
                .FromKebabCase()
                .Humanize(true)
                .ToEnum<T>();
        }

        public static string[] ToArray()
        {
            return (string[])Enum.GetValues(typeof(T));
        }

        public static List<int> ToList()
        {
            return Enum
                .GetValues(typeof(T))
                .Cast<int>()
                .ToList();
        }

        public static List<FieldInfo> GetFields()
        {
            return typeof(T)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .ToList();
        }

        public static Dictionary<int, string> ToDictionary()
        {
            var values = Enum.GetValues(typeof(T)).Cast<object>().ToList();
            if (values.Any())
                return values.ToDictionary(x => (int)x, x => x.ToString());

            return new Dictionary<int, string>();
        }

        public static List<T> ToListOrderBy(params T[] array)
        {
            var list = ToList().Select(x => (T)(object)x).ToList();
            var result = array.ToList().Intersect(list).Union(list).ToList();
            return result;
        }
    }
}