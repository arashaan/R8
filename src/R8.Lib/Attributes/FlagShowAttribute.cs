using System;
using System.Linq;
using System.Reflection;

namespace R8.Lib.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FlagShowAttribute : Attribute
    {
    }

    public static class FlagShowExtensions
    {
        public static bool HasFlagShow(this Enum field)
        {
            return field
                .GetType()
                .GetMember(field.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<FlagShowAttribute>() != null;
        }
    }
}