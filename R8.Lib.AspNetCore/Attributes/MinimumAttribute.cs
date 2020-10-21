using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using System;

namespace R8.Lib.AspNetCore.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RangeAttribute : System.ComponentModel.DataAnnotations.RangeAttribute, IClientModelValidator
    {
        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-range", ErrorMessageString);
            context.Attributes.Add("data-val-range-max", Maximum.ToString());
            context.Attributes.Add("data-val-range-min", Minimum.ToString());
            context.Attributes.Add("min", Minimum.ToString());
            context.Attributes.Add("max", Maximum.ToString());
        }

        public RangeAttribute(double minimum, double maximum) : base(minimum, maximum)
        {
        }

        public RangeAttribute(int minimum, int maximum) : base(minimum, maximum)
        {
        }

        public RangeAttribute(Type type, string minimum, string maximum) : base(type, minimum, maximum)
        {
        }
    }
}