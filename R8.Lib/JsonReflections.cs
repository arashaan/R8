using Newtonsoft.Json;

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace R8.Lib
{
    public static class JsonReflections
    {
        public static string GetJsonProperty<TModel>(Expression<Func<TModel, object>> property)
        {
            var json = property.GetPropertyAttribute<TModel, JsonPropertyAttribute>();
            if (json != null)
                return json.PropertyName;

            var propertyInfo = property.GetProperty();
            return propertyInfo.GetJsonProperty();
        }

        public static string GetJsonProperty(this PropertyInfo propertyInfo)
        {
            var json = propertyInfo.GetPropertyAttribute<JsonPropertyAttribute>();
            return json != null
                ? json.PropertyName
                : propertyInfo.Name;
        }
    }
}