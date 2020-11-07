using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace R8.Lib.AspNetCore.Base
{
    public abstract class BaseSearchModel : ValidatableObject, IBaseSearch
    {
        private string _pageNo;

        public object this[string key]
        {
            get
            {
                var property = Array.Find(GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance),
                  x => x.Name.Equals(key, StringComparison.CurrentCulture));
                if (property == null)
                    throw new NullReferenceException($"{GetType().Name} haven't {nameof(key)} property.");

                return property.GetValue(this);
            }
            set
            {
                var property = Array.Find(GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance),
                  x => x.Name.Equals(key, StringComparison.CurrentCulture));
                if (property == null)
                    throw new NullReferenceException($"{GetType().Name} haven't {nameof(key)} property.");

                property.SetValue(this, value);
            }
        }

        [FromRoute(Name = "pageNo")]
        [HiddenInput]
        [Required]
        public int PageNo
        {
            get => Numbers.FixPageNumber(_pageNo);
            set => _pageNo = value.ToString();
        }

        [HiddenInput]
        [JsonIgnore]
        public int PageSize { get; set; } = 10;

        public Dictionary<string, object> GetRouteData()
        {
            {
                var routeValues = new Dictionary<string, object>();

                var properties = GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => x.GetCustomAttribute<FromQueryAttribute>() != null
                                || x.GetCustomAttribute<BindPropertyAttribute>() != null)
                     .Select(x => new
                     {
                         Property = x,
                         Value = x.GetValue(this),
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

                    if (complex.Property.Name == nameof(PageNo))
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