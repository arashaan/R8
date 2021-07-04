using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using R8.Lib;

using System;
using System.Linq;

using AutoCompletes = R8.AspNetCore.Enums.AutoCompletes;

namespace R8.AspNetCore.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoCompleteTagAttribute : Attribute, IClientModelValidator
    {
        public bool Off { get; set; } = false;
        public AutoCompletes[] Patterns { get; set; }

        public AutoCompleteTagAttribute(bool off)
        {
            Off = off;
        }

        public AutoCompleteTagAttribute(AutoCompletes pattern) : this(false)
        {
            this.Patterns = new[] { pattern };
        }

        public AutoCompleteTagAttribute(params AutoCompletes[] patternses) : this(false)
        {
            this.Patterns = patternses;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            const string key = "autocomplete";
            string tagValue;
            if (!Off)
            {
                var patterns = Patterns.Select(x => x.GetDescription());
                tagValue = string.Join(" ", patterns);
            }
            else
            {
                tagValue = "off";
            }

            if (!context.Attributes.ContainsKey(key))
                context.Attributes.Add(key, tagValue);
        }
    }
}