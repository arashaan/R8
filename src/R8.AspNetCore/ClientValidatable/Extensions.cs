using R8.AspNetCore.ClientValidatable.Types;
using R8.Lib;
using R8.Lib.Validatable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace R8.AspNetCore.ClientValidatable
{
    public static class Extensions
    {
        public const string ExceptionDivider = "...";
        public const string ExceptionKeyValueDivider = "|";
        public const string ExceptionErrorsDivider = ";";

        public static string ToQueryString(this ValidatableResultCollection collection)
        {
            var errs = new List<string>();
            foreach (var (name, errors) in collection)
            {
                var errorExceptions = string.Join(ExceptionErrorsDivider, errors);

                var finalValue = name + ExceptionKeyValueDivider + errorExceptions;
                errs.Add(finalValue);
            }

            return string.Join(ExceptionDivider, errs);
        }

        public static string ToQueryString(this ValidatableResultCollection collection, Type modelType)
        {
            var errs = new List<string>();
            foreach (var (name, errors) in collection)
            {
                var keyProp = modelType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(c => c.Name == name);
                if (keyProp == null)
                    continue;

                var key = keyProp.GetDisplayName();
                var errorExceptions = string.Join(ExceptionErrorsDivider, errors);

                var finalValue = key + ExceptionKeyValueDivider + errorExceptions;
                errs.Add(finalValue);
            }

            return string.Join(ExceptionDivider, errs);
        }

        public static string ToQueryString<T>(this ValidatableObject<T> obj) where T : class
        {
            return obj.ValidationErrors.ToQueryString(obj.GetType());
        }

        public static string ToQueryString(this ValidatableObject obj)
        {
            return obj.ValidationErrors.ToQueryString(obj.GetType());
        }

        public static ClientValidation GetClientValidators(string propertyName, Action<Configuration> configure)
        {
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var configuration = new Configuration();
            configure(configuration);

            if (configuration.PropertyType == null)
                throw new ArgumentNullException(nameof(configuration.PropertyType));

            var output = new Dictionary<string, object>();
            if (configuration.PropertyType == typeof(string))
            {
                var @string = new ClientValidatorString();
                configuration.String(@string);

                var regex = @string.RegularExpression;
                if (!string.IsNullOrEmpty(regex))
                {
                    output.Add("data-val-regex", "");
                    output.Add("data-val-regex-pattern", regex);
                }
            }
            else if (configuration.PropertyType == typeof(int) || configuration.PropertyType == typeof(double))
            {
                var @int = new ClientValidatorInteger();
                configuration.Integer(@int);

                var range = new ClientValidatorIntegerRange();
                @int.Range(range);

                if (range.Minimum != null && range.Maximum != null)
                {
                    output.Add("data-val-range", "");
                    output.Add("data-val-range-min", range.Minimum);
                    output.Add("data-val-range-max", range.Maximum);
                }
                else
                {
                    if (range.Minimum != null)
                    {
                        output.Add("min", range.Minimum.ToString());
                    }
                    if (range.Maximum != null)
                    {
                        output.Add("max", range.Maximum.ToString());
                    }
                }
            }

            var required = configuration.IsRequired;
            if (required)
                output.Add("data-val-required", "");

            if (output?.Any() == true)
                output.Add("data-val", "true");

            var result = new ClientValidation
            {
                Name = propertyName,
                Validations = output
            };
            return result;
        }
    }
}