using System.Reflection;

namespace R8.Lib
{
    public static class MethodReflections
    {
        /// <summary>
        /// Checks if given method is overriden from base type.
        /// </summary>
        /// <param name="method"></param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool IsOverriden(this MethodInfo method)
        {
            return method.GetBaseDefinition().DeclaringType != method.DeclaringType;
        }
    }
}