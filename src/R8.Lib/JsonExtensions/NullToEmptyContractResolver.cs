using Newtonsoft.Json.Serialization;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace R8.Lib.JsonExtensions
{
    /// <summary>
    /// Initializes an <see cref="NullToEmptyContractResolver"/> that shows empty instead of null based on property type
    /// </summary>
    public class NullToEmptyContractResolver : DefaultContractResolver
    {
        protected override IValueProvider CreateMemberValueProvider(MemberInfo member)
        {
            var provider = base.CreateMemberValueProvider(member);
            if (member.MemberType != MemberTypes.Property) return provider;

            var propType = ((PropertyInfo)member).PropertyType;
            if (propType.IsGenericType
                && propType.GetGenericTypeDefinition() == typeof(List<>))
            {
                return new EmptyListValueProvider(provider, propType);
            }

            return propType == typeof(string)
                ? new NullToEmptyStringValueProvider(provider)
                : provider;
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToCamelCase(culture: CultureInfo.InvariantCulture);
        }

        private class EmptyListValueProvider : IValueProvider
        {
            private readonly IValueProvider _provider;
            private readonly object _defaultValue;

            public EmptyListValueProvider(IValueProvider innerProvider, Type listType)
            {
                _provider = innerProvider;
                _defaultValue = Activator.CreateInstance(listType);
            }

            public void SetValue(object target, object value)
            {
                _provider.SetValue(target, value ?? _defaultValue);
            }

            public object GetValue(object target)
            {
                return _provider.GetValue(target) ?? _defaultValue;
            }
        }

        private sealed class NullToEmptyStringValueProvider : IValueProvider
        {
            private readonly IValueProvider _provider;

            public NullToEmptyStringValueProvider(IValueProvider provider)
            {
                _provider = provider;
            }

            public object GetValue(object target)
            {
                return _provider.GetValue(target) ?? "";
            }

            public void SetValue(object target, object value)
            {
                _provider.SetValue(target, value);
            }
        }
    }
}