using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

using R8.Lib;
using R8.Lib.Localization;

namespace R8.AspNetCore.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterAttribute : ValidationAttribute
    {
        private readonly Type _enumType;
        private readonly Dictionary<int, string> _allowed;

        public FilterAttribute(Type enumType, string[] included, string[] excluded)
        {
            if (enumType == null)
                throw new NullReferenceException(nameof(enumType));

            if (!enumType.IsEnum)
                throw new Exception(nameof(enumType));

            _enumType = enumType;
            var enums = EnumReflections.ToDictionary(enumType);
            if (enums?.Any() != true)
                return;

            Dictionary<int, string> allowedMembers;
            if (included?.Any() != true && excluded?.Any() != true)
            {
                allowedMembers = enums;
            }
            else
            {
                if (excluded != null && included?.Any(excluded.Contains) == true)
                    throw new Exception("Included and excluded should be unique");

                var including = new List<KeyValuePair<int, string>>();
                if (included?.Any() == true)
                    including = enums.Where(x => included.Any(c => c == x.Value)).ToList();

                var excluding = new List<KeyValuePair<int, string>>();
                if (excluded?.Any() == true)
                    excluding = enums.Where(x => excluded.Any(c => c == x.Value)).ToList();

                allowedMembers = including.Count > 0 ? new Dictionary<int, string>() : enums;
                if (including.Count > 0)
                {
                    allowedMembers.Clear();
                    foreach (var (id, name) in including)
                        allowedMembers.Add(id, name);
                }

                if (excluding.Count > 0)
                {
                    allowedMembers = allowedMembers
                        .Except(excluding)
                        .ToDictionary(x => x.Key, x => x.Value);
                }
            }

            _allowed = allowedMembers;
        }

        public List<SelectListItem> GetSelectListItems(ILocalizer localizer)
        {
            return _allowed?.Select(x => new SelectListItem
            {
                Text = GetFromResource(_enumType, x.Value, localizer),
                Value = x.Key.ToString()
            }).ToList();
        }

        private static string GetFromResource(Type enumType, string enumValue, ILocalizer localizer)
        {
            var valid = Enum.TryParse(enumType, enumValue, out var enumm);
            if (!valid || enumm == null)
                return null;

            var str = localizer[enumm.ToString()];
            return str;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return _allowed.ContainsValue(value.ToString())
              ? ValidationResult.Success
              : new ValidationResult("Forbidden item");
        }
    }
}