using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using R8.AspNetCore.Attributes;
using R8.Lib;
using R8.Lib.Validatable;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;

namespace R8.AspNetCore
{
    public static class Reflections
    {
        public static (IHtmlContent, IHtmlContent) GetRegularExpression<TModel>(this Expression<Func<TModel, object>> property, bool requireTag = true)
        {
            return GetRegularExpression(property.Type, property.Name);
        }

        /// <summary>
        /// Returns <see cref="ModelMetadata"/> for given model.
        /// </summary>
        /// <typeparam name="T">Derived type of <see cref="ValidatableObject"/>.</typeparam>
        /// <param name="model">A model of type <see cref="ValidatableObject{TModel}"/>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="ModelMetadata"/> object.</returns>
        public static ModelExplorer GetModelExplorer<T>(this T model) where T : ValidatableObject
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var modelType = model.GetType();
            var provider = new EmptyModelMetadataProvider();
            var explorer = provider.GetModelExplorerForType(modelType, model);
            return explorer;
        }

        /// <summary>
        /// Returns <see cref="ModelMetadata"/> for given property in model.
        /// </summary>
        /// <typeparam name="T">Derived type of <see cref="ValidatableObject"/>.</typeparam>
        /// <param name="model">A model of type <see cref="ValidatableObject{TModel}"/>.</param>
        /// <param name="property">A <see cref="Expression{TDelegate}"/> that representing specific property in model.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>A <see cref="ModelMetadata"/> object.</returns>
        public static ModelExplorer GetExplorerForProperty<T>(this T model, Expression<Func<T, object>> property) where T : ValidatableObject
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (property == null) throw new ArgumentNullException(nameof(property));

            if (!(property.Body is MemberExpression memberExpression))
                throw new ArgumentException($"Expression' body must be a type of {nameof(MemberExpression)}");

            var member = memberExpression.Member;
            var propertyExplorer = model.GetExplorerForProperty((PropertyInfo)member);
            return propertyExplorer;
        }

        /// <summary>
        /// Returns <see cref="ModelMetadata"/> for given property in model.
        /// </summary>
        /// <typeparam name="T">Derived type of <see cref="ValidatableObject"/>.</typeparam>
        /// <param name="model">A model of type <see cref="ValidatableObject{TModel}"/>.</param>
        /// <param name="property">A <see cref="Expression{TDelegate}"/> that representing specific property in model.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <returns>A <see cref="ModelMetadata"/> object.</returns>
        public static DefaultModelMetadata GetMetadataForProperty<T>(this T model, Expression<Func<T, object>> property) where T : ValidatableObject
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (property == null) throw new ArgumentNullException(nameof(property));

            if (!(property.Body is MemberExpression memberExpression))
                throw new ArgumentException($"Expression' body must be a type of {nameof(MemberExpression)}");

            var modelType = model.GetType();
            var member = memberExpression.Member;
            var provider = new EmptyModelMetadataProvider();
            var metadata = provider.GetMetadataForProperty((PropertyInfo)member, modelType);
            return metadata.GetDefaultModelMetadata();
        }

        /// <summary>
        /// Returns <see cref="ModelMetadata"/> for given property in model.
        /// </summary>
        /// <typeparam name="T">Derived type of <see cref="ValidatableObject"/>.</typeparam>
        /// <param name="model">A model of type <see cref="ValidatableObject{TModel}"/>.</param>
        /// <param name="propertyName">A <see cref="string"/> that representing specific property in model.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <returns>A <see cref="ModelMetadata"/> object.</returns>
        public static DefaultModelMetadata GetMetadataForProperty<T>(this T model, string propertyName) where T : ValidatableObject
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

            var propertyInfo = model.GetType().GetProperty(propertyName);
            var modelType = model.GetType();
            var provider = new EmptyModelMetadataProvider();
            var metadata = provider.GetMetadataForProperty(propertyInfo, modelType);
            return metadata.GetDefaultModelMetadata();
        }

        /// <summary>
        /// Returns <see cref="ModelMetadata"/> for given property in model.
        /// </summary>
        /// <typeparam name="T">Derived type of <see cref="ValidatableObject"/>.</typeparam>
        /// <param name="model">A model of type <see cref="ValidatableObject{TModel}"/>.</param>
        /// <param name="propertyInfo">A <see cref="PropertyInfo"/> that representing specific property in model.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <returns>A <see cref="ModelMetadata"/> object.</returns>
        public static DefaultModelMetadata GetMetadataForProperty<T>(this T model, PropertyInfo propertyInfo) where T : ValidatableObject
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            var modelType = model.GetType();
            var provider = new EmptyModelMetadataProvider();
            var metadata = provider.GetMetadataForProperty(propertyInfo, modelType);
            return metadata.GetDefaultModelMetadata();
        }

        /// <summary>
        /// Returns <see cref="ModelMetadata"/> for given property in model.
        /// </summary>
        /// <typeparam name="T">Derived type of <see cref="ValidatableObject"/>.</typeparam>
        /// <param name="model">A model of type <see cref="ValidatableObject{TModel}"/>.</param>
        /// <param name="propertyInfo">A <see cref="PropertyInfo"/> that representing specific property in model.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <returns>A <see cref="ModelMetadata"/> object.</returns>
        public static ModelExplorer GetExplorerForProperty<T>(this T model, PropertyInfo propertyInfo) where T : ValidatableObject
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            var modelExplorer = model.GetModelExplorer();
            var propertyExplorer = modelExplorer.GetExplorerForProperty(propertyInfo.Name);
            return propertyExplorer;
        }

        /// <summary>
        /// Returns <see cref="DefaultModelMetadata"/> for given <see cref="ModelMetadata"/>.
        /// </summary>
        /// <param name="metadata">A <see cref="ModelMetadata"/> object.</param>
        /// <returns>A <see cref="DefaultModelMetadata"/> object.</returns>
        public static DefaultModelMetadata GetDefaultModelMetadata(this ModelMetadata metadata)
        {
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            var defaultModelMetadata = (DefaultModelMetadata)metadata;
            return defaultModelMetadata;
        }

        /// <summary>
        /// Returns <see cref="ModelMetadata"/> for given property in model.
        /// </summary>
        /// <typeparam name="T">Derived type of <see cref="ValidatableObject"/>.</typeparam>
        /// <param name="model">A model of type <see cref="ValidatableObject{TModel}"/>.</param>
        /// <param name="propertyName">A <see cref="string"/> that representing specific property in model.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <returns>A <see cref="ModelMetadata"/> object.</returns>
        public static ModelExplorer GetExplorerForProperty<T>(this T model, string propertyName) where T : ValidatableObject
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

            var propertyInfo = model.GetType().GetProperty(propertyName);
            var propertyExplorer = model.GetExplorerForProperty(propertyInfo);
            return propertyExplorer;
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