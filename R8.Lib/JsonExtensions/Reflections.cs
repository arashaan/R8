using Newtonsoft.Json;

using System.Reflection;

namespace R8.Lib.JsonExtensions
{
    public static class Reflections
    {
        /// <summary>
        /// Returns Property's json name that already entered in <see cref="JsonPropertyAttribute"/>
        /// </summary>
        /// <param name="propertyInfo">An <see cref="PropertyInfo"/> that containing data about specific property</param>
        /// <returns>An <see cref="string"/> value</returns>
        public static string GetJsonName(this PropertyInfo propertyInfo)
        {
            var json = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();
            return json != null
                ? json.PropertyName
                : propertyInfo.Name;
        }
    }
}