using System;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

namespace R8.Lib
{
    public static class JsonReflections
    {
        public static string GetJsonProperty(this MemberInfo memberInfo)
        {
            var json = memberInfo.GetCustomAttribute<JsonPropertyAttribute>();
            if (json != null)
                return json.PropertyName;

            var contractResolver = JsonSettingsExtensions.JsonNetSettings.ContractResolver;
            if (contractResolver is NullToEmptyContractResolver nullToEmptyContractResolver)
                return nullToEmptyContractResolver.GetResolvedPropertyName(memberInfo.Name);

            return memberInfo.Name;
        }

        public static PropertyInfo GetProperty<TModel>(this string propertyName)
        {
            var modelType = typeof(TModel);

            var property = Array.Find(modelType.GetProperties(), x =>
            {
                if (x.Name == propertyName)
                    return true;

                return GetJsonProperty((MemberInfo)x) == propertyName;
            });

            return property;
        }

        public static string GetJsonProperty<TModel>(this PropertyInfo _, Expression<Func<TModel, object>> property)
        {
            var json = property.GetPropertyAttribute<TModel, JsonPropertyAttribute>();
            if (json != null)
                return json.PropertyName;

            var propertyInfo = property.GetProperty();
            var contractResolver = JsonSettingsExtensions.JsonNetSettings.ContractResolver;
            if (contractResolver is NullToEmptyContractResolver nullToEmptyContractResolver)
                return nullToEmptyContractResolver.GetResolvedPropertyName(propertyInfo.Name);

            return propertyInfo.Name;
        }

        public static string GetJsonProperty<TModel>(this Expression<Func<TModel, object>> property)
        {
            var json = property.GetPropertyAttribute<TModel, JsonPropertyAttribute>();
            if (json != null)
                return json.PropertyName;

            var propertyInfo = property.GetProperty();
            var contractResolver = JsonSettingsExtensions.JsonNetSettings.ContractResolver;
            if (contractResolver is NullToEmptyContractResolver nullToEmptyContractResolver)
                return nullToEmptyContractResolver.GetResolvedPropertyName(propertyInfo.Name);

            return propertyInfo.Name;
        }

        public static string GetJsonProperty(this PropertyInfo propertyInfo)
        {
            var json = propertyInfo.GetPropertyAttribute<JsonPropertyAttribute>();
            if (json != null)
                return json.PropertyName;

            var contractResolver = JsonSettingsExtensions.JsonNetSettings.ContractResolver;
            if (contractResolver is NullToEmptyContractResolver nullToEmptyContractResolver)
                return nullToEmptyContractResolver.GetResolvedPropertyName(propertyInfo.Name);

            return propertyInfo.Name;
        }
    }
}