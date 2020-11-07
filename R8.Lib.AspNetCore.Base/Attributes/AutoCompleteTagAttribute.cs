using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AutoCompletes = R8.Lib.AspNetCore.Base.Enums.AutoCompletes;

namespace R8.Lib.AspNetCore.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoCompleteTagAttribute : Attribute, IClientModelValidator
    {
        public AutoCompletes[] Patterns { get; set; }

        public AutoCompleteTagAttribute(AutoCompletes pattern)
        {
            this.Patterns = new[] { pattern };
        }

        public AutoCompleteTagAttribute(params AutoCompletes[] patternses)
        {
            this.Patterns = patternses;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var patts = Patterns.Select(x => x.GetDescription());
            var pattString = string.Join(" ", patts);
            MergeAttribute(context.Attributes, "autocomplete", pattString);
        }

        private void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key)) return;

            attributes.Add(key, value);
        }
    }
}