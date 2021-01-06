using Microsoft.AspNetCore.Mvc;

using R8.Lib;
using R8.Lib.Search;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace R8.AspNetCore.Routing
{
    public static class BaseSearchExtensions
    {
        public static Dictionary<string, object> GetRouteData<T>(this T search) where T : ISearchBase
        {
            {
                var routeValues = new Dictionary<string, object>();

                var properties = search.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => x.GetCustomAttribute<FromQueryAttribute>() != null
                                || x.GetCustomAttribute<BindPropertyAttribute>() != null)
                     .Select(x => new
                     {
                         Property = x,
                         Value = x.GetValue(search),
                     })
                     .Where(x => x.Value != null)
                    .ToList();
                if (properties?.Any() != true)
                    return default;

                foreach (var complex in properties)
                {
                    string key;
                    var fromQueryAttr = complex.Property.GetCustomAttribute<FromQueryAttribute>();
                    if (fromQueryAttr == null)
                    {
                        var bindPropertyAttr = complex.Property.GetCustomAttribute<BindPropertyAttribute>();
                        if (bindPropertyAttr != null)
                        {
                            var _key = bindPropertyAttr.Name;
                            if (!string.IsNullOrEmpty(_key))
                            {
                                key = _key;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        var _key = fromQueryAttr.Name;
                        if (!string.IsNullOrEmpty(_key))
                        {
                            key = _key;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (complex.Property.Name == nameof(search.PageNo))
                        if (complex.Value is int valueInt)
                            if (valueInt == 1)
                                continue;

                    var propertyType = Nullable.GetUnderlyingType(complex.Property.PropertyType) ??
                                       complex.Property.PropertyType;
                    if (propertyType.IsArray)
                    {
                        var defaultValue = Array.CreateInstance(propertyType.GetElementType(), 0);
                        if (complex.Value.Equals(defaultValue))
                            continue;
                    }
                    else
                    {
                        if (propertyType != typeof(string) && !propertyType.IsEnum)
                        {
                            var defaultValue = Activator.CreateInstance(propertyType);
                            if (complex.Value.Equals(defaultValue))
                                continue;
                        }
                    }

                    object? finalValue;
                    if (propertyType.IsArray)
                    {
                        var arrValues = (IEnumerable)complex.Value;
                        finalValue = string.Join(SeparatedQueryStringValueProviderSeparator.Separator, arrValues.Cast<object>());
                    }
                    else if (propertyType.IsEnum)
                    {
                        //var ff = Enum.Parse(propertyType, complex.Value.ToString());
                        var @enum = EnumReflections.ToEnum(propertyType, complex.Value.ToString());
                        var enumValue = Convert.ToInt32(@enum);
                        finalValue = enumValue.ToString();
                    }
                    else
                    {
                        finalValue = complex.Value;
                    }
                    routeValues.Add(key, finalValue);
                    //if (searchParameterAttribute.Type != null)
                    //{
                    //if (property.PropertyType != typeof(string))
                    //    continue;

                    //var encodeJson = Jsons.Encode(value.ToString(), searchParameterAttribute.Type);
                    //routeValues.Add(searchParameter, encodeJson);
                    //}
                    //else
                    //{
                    //    routeValues.Add(searchParameter, value);
                    //}
                }

                return routeValues;
            }
        }
    }
}