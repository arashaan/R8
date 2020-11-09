using System;
using System.Collections.Generic;
using System.Linq;
using R8.Lib;

namespace R8.AspNetCore
{
    public class ClientValidation
    {
        public string Name { get; set; }
        public Dictionary<string, object> Validations { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public void Deconstruct(out string name, out Dictionary<string, object> validations)
        {
            name = Name;
            validations = Validations;
        }
    }

    public class ClientValidatorString
    {
        public string RegularExpression { get; set; }
        public int? MaxLength { get; set; }
    }

    public class ClientValidatorIntegerRange
    {
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }
    }

    public class ClientValidatorInteger
    {
        public Action<ClientValidatorIntegerRange> Range { get; set; }
    }

    public static class ValidatableObjectExtensions
    {
        public static string ToQueryString<T>(this ValidatableObject<T> obj) where T : class
        {
            return obj.ValidationErrors.ToQueryString(obj.GetType());
        }

        public static string ToQueryString(this ValidatableObject obj)
        {
            return obj.ValidationErrors.ToQueryString(obj.GetType());
        }

        public static ClientValidation GetClientValidators(string propertyName, Action<ClientValidatorConfiguration> configure)
        {
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var configuration = new ClientValidatorConfiguration();
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