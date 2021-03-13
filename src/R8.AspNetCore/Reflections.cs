using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using R8.Lib.Validatable;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace R8.AspNetCore
{
    public static class Reflections
    {
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
        /// Returns an <see cref="IEnumerable{T}"/> of <see cref="Attribute"/> for given metadata.
        /// </summary>
        /// <param name="metadata">A <see cref="ModelMetadata"/> object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Attribute"/></returns>
        public static IEnumerable<object> GetAttributes(this ModelMetadata metadata)
        {
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));

            var attributes = metadata.GetDefaultModelMetadata().Attributes.PropertyAttributes;
            return attributes;
        }

        /// <summary>
        /// Returns whether given metadata is required or not.
        /// </summary>
        /// <param name="metadata">A <see cref="ModelMetadata"/> object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="bool"/> value.</returns>
        public static bool IsRequired(this ModelMetadata metadata)
        {
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));

            var attributes = metadata.GetAttributes();
            var required = attributes.Any(x => x is RequiredAttribute);
            return required;
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
            var metadata = model.GetMetadataForProperty(propertyInfo);
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
    }
}