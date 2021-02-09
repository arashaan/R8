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
        public static TAttribute GetAttribute<TAttribute>(this Enum enumMember)
          where TAttribute : Attribute
        {
            if (enumMember == null) throw new ArgumentNullException(nameof(enumMember));
            var enumType = enumMember.GetType();
            var enumName = Enum.GetName(enumType, enumMember);
            if (enumName == null)
                return null;

            var memberInfos = enumType.GetMember(enumName);
            var memberInfo = memberInfos.Single();
            return memberInfo.GetCustomAttribute<TAttribute>();
        }

        public static object ToEnum(Type enumType, string value)
        {
            return Enum.Parse(enumType, value, true);
        }

        public static TEnum ToEnum<TEnum>(this string value) where TEnum : Enum
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }

        public static string GetDescription(this Enum value)
        {
            return value.GetAttribute<DescriptionAttribute>()?.Description ?? Enum.GetName(value.GetType(), value);
        }

        public static string GetDisplayName(this Enum value)
        {
            return value.GetAttribute<DisplayAttribute>()?.GetName() ?? Enum.GetName(value.GetType(), value);
        }

        public static string Display(Type enumType, string name)
        {
            var result = name;

            var field = enumType.GetField(name);
            if (field == null)
                return result;

            var attribute = field
              .GetCustomAttributes(inherit: false)
              .OfType<DisplayAttribute>()
              .FirstOrDefault();

            if (attribute != null)
                result = attribute.GetName();

            return result;
        }

        public static Dictionary<int, string> ToDictionary(Type enumType)
        {
            return Enum
              .GetValues(enumType)
              .Cast<object>()
              .ToDictionary(foo => (int)foo, foo => foo.ToString());
        }

        public static Dictionary<int, string> ToDictionary<TEnum>() where TEnum : Enum
        {
            return Enum
              .GetValues(typeof(TEnum))
              .Cast<object>()
              .ToDictionary(x => (int)x, x => x.ToString());
        }

        public static List<int> ToList<TEnum>() where TEnum : Enum
        {
            return Enum
              .GetValues(typeof(TEnum))
              .Cast<int>()
              .ToList();
        }

        public static string[] ToArray<TEnum>() where TEnum : Enum
        {
            if (!typeof(TEnum).IsEnum)
                throw new InvalidOperationException();

            return (string[])Enum.GetValues(typeof(TEnum));
        }
    }
}