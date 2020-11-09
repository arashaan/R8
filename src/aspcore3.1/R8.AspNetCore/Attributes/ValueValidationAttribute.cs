using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using R8.Lib;

namespace R8.AspNetCore.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValueValidationAttribute : ValidationAttribute, IClientModelValidator
    {
        public RegexPatterns Pattern { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return ValidationResult.Success;

            var result = !Regex.Match(value.ToString(), Pattern.GetDisplayName()).Success
              ? new ValidationResult(ErrorMessageString)
              : ValidationResult.Success;
            return result;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-regex", ErrorMessageString);
            context.Attributes.Add("data-val-regex-pattern", Pattern.GetDisplayName());
        }
    }
}