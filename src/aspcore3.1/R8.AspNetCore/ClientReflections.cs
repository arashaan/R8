using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;

using Microsoft.AspNetCore.Html;

using R8.AspNetCore.Attributes;
using R8.Lib;

namespace R8.AspNetCore
{
    public static class ClientReflections
    {
        public static (IHtmlContent, IHtmlContent) GetRegularExpression<TModel>(this Expression<Func<TModel, object>> property, bool requireTag = true)
        {
            return GetRegularExpression(property.Type, property.Name);
        }

        public static Dictionary<string, object> GetValidator(this Type modelType, string expression, Type resourceType)
        {
            if (string.IsNullOrEmpty(expression))
                return default;

            var property = modelType.GetPublicProperties().FirstOrDefault(x => x.Name.Equals(expression, StringComparison.CurrentCulture));
            if (property == null)
                return default;

            var result = new Dictionary<string, object>
            {
                {
                    "data-val", "true"
                }
            };
            var regTerm = string.Empty;

            var required = property.GetCustomAttribute<RequiredAttribute>();
            if (required != null)
            {
                var finalRequired = string.Empty;
                if (required.ErrorMessageResourceType != null)
                {
                    var resourceManager = new ResourceManager(resourceType);

                    var displayName = property.GetDisplayName();
                    var requiredString = resourceManager.GetString(required.ErrorMessageResourceName);
                    finalRequired += string.Format(requiredString, displayName);
                }
                else
                {
                    finalRequired += required.ErrorMessage;
                }

                result.Add("data-val-required", finalRequired);
            }

            var (pattern, regex) = modelType.GetRegularExpression(expression);
            if (pattern != null && regex != null)
                regTerm = $"{pattern} {regex}";

            return result;
        }

        public static (IHtmlContent, IHtmlContent) GetRegularExpression(this Type modelType, string expression, bool requireTag = true)
        {
            if (modelType == null)
                return default;

            if (string.IsNullOrEmpty(expression))
                return default;

            var property = modelType.GetPublicProperties()
                .Find(x => x.Name.Equals(expression, StringComparison.CurrentCulture));
            if (property == null)
                return default;

            var validator = property.GetCustomAttribute<ValueValidationAttribute>();
            if (validator != null)
            {
                return new ValueTuple<IHtmlContent, IHtmlContent>
                {
                    Item1 = requireTag
                        ? new HtmlString($"data-val-regex-pattern=\"{validator.Pattern}\"")
                        : new HtmlString(validator.Pattern),
                    Item2 = requireTag
                        ? new HtmlString($"data-val-regex=\"{validator.ErrorMessage}\"")
                        : new HtmlString(validator.ErrorMessage)
                };
            }

            var regular = property.GetCustomAttribute<RegularExpressionAttribute>();
            if (regular != null)
            {
                return new ValueTuple<IHtmlContent, IHtmlContent>
                {
                    Item1 = requireTag
                        ? new HtmlString($"data-val-regex-pattern=\"{regular.Pattern}\"")
                        : new HtmlString(regular.Pattern),
                    Item2 = requireTag
                        ? new HtmlString($"data-val-regex=\"{regular.ErrorMessage}\"")
                        : new HtmlString(regular.ErrorMessage)
                };
            }

            return new ValueTuple<IHtmlContent, IHtmlContent>(null, null);
        }
    }
}