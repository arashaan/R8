using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace R8.Lib.Validatable
{
    /// <summary>
    /// Initializes a <see cref="ValidatableObject{TResult}"/> to validate object's properties
    /// </summary>
    /// <typeparam name="TModel">An object type that need to be validated.</typeparam>
    public class ValidatableObject<TModel> : ValidatableObject where TModel : class
    {
        /// <summary>
        /// Validates a property manually, based on Data Annotations.
        /// </summary>
        /// <typeparam name="T">A generic type that need to be validated.</typeparam>
        /// <param name="property">An <see cref="Expression{TDelegate}"/> that representing specific property in model.</param>
        /// <param name="validatableResult">An <see cref="ValidatableResult"/> object that representing errors found based on Annotations rules.</param>
        /// <returns>A <see cref="bool"/> value</returns>
        /// <remarks>If true property value has been validated, and not validated otherwise.</remarks>
        public bool TryValidateProperty<T>(Expression<Func<TModel, T>> property, out ValidatableResult validatableResult)
        {
            var model = this as TModel;
            var value = property.Compile().Invoke(model);
            return TryValidateProperty(property, value, out validatableResult);
        }
    }

    /// <summary>
    /// Initializes a <see cref="ValidatableObject"/> to validate object's properties
    /// </summary>
    public abstract class ValidatableObject : IValidatableObject
    {
        /// <summary>
        /// Validates object properties based on Data Annotations.
        /// </summary>
        /// <typeparam name="TModel">An object type that need to be validated.</typeparam>
        /// <param name="model">A <see cref="TModel"/> object to be validated.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>If true object has been validated, and not validated otherwise.</returns>
        public static bool Validate<TModel>(TModel model) where TModel : class
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return TryValidate(model, out _);
        }

        /// <summary>
        /// Validates object properties based on Data Annotations.
        /// </summary>
        /// <typeparam name="TModel">An object type that need to be validated.</typeparam>
        /// <param name="model">A <see cref="TModel"/> object to be validated.</param>
        /// <param name="validatableResultCollection">A <see cref="ValidatableResultCollection"/> object to show a collection of errors</param>
        /// <returns>A <see cref="bool"/> value</returns>
        /// <remarks>If true object has been validated, and not validated otherwise.</remarks>
        public static bool TryValidate<TModel>(TModel model, out ValidatableResultCollection validatableResultCollection) where TModel : class
        {
            validatableResultCollection = null;
            if (model == null)
                return false;

            var context = new ValidationContext(model);
            validatableResultCollection = Validate(context, model);
            return validatableResultCollection.Count == 0;
        }

        /// <summary>
        /// Validates a property manually, based on Data Annotations.
        /// </summary>
        /// <typeparam name="TModel">An object type that containing specific property.</typeparam>
        /// <param name="context">A <see cref="ValidationContext"/></param>
        /// <param name="propertyName">A <see cref="string"/> value that representing name of <see cref="PropertyInfo"/></param>
        /// <param name="value">A <see cref="object"/> value that should be set to property info to check if validated.</param>
        /// <param name="validationResult">An <see cref="ValidatableResult"/> object that representing errors found based on Annotations rules.</param>
        /// <returns>A <see cref="bool"/> value</returns>
        /// <remarks>If true property value has been validated, and not validated otherwise.</remarks>
        public static bool TryValidateProperty<TModel>(ValidationContext context, string propertyName, object value, out ValidatableResult validationResult) where TModel : class
        {
            validationResult = null;
            if (context == null)
                return false;

            validationResult = null;
            var prop = typeof(TModel)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetSetMethod() != null && x.MemberType == MemberTypes.Property)
                .FirstOrDefault(x => x.Name == propertyName);
            if (prop == null)
                return false;

            var tempValidation = new List<ValidationResult>();

            var key = prop.Name;
            if (key.Equals("Item") && prop.PropertyType == typeof(object))
                return false;

            context.MemberName = key;
            Validator.TryValidateProperty(value, context, tempValidation);
            validationResult = ToValidatableResult(tempValidation, key);
            context.MemberName = null;

            return validationResult == null;
        }

        /// <summary>
        /// Validates a property manually, based on Data Annotations.
        /// </summary>
        /// <typeparam name="TModel">An object type that containing specific property.</typeparam>
        /// <param name="context">A <see cref="ValidationContext"/></param>
        /// <param name="model">A <see cref="TModel"/> object that containing specific property to be validated.</param>
        /// <param name="propertyName">A <see cref="string"/> value that representing name of <see cref="PropertyInfo"/></param>
        /// <param name="validationResult">An <see cref="ValidatableResult"/> object that representing errors found based on Annotations rules.</param>
        /// <returns>A <see cref="bool"/> value</returns>
        /// <remarks>If true property value has been validated, and not validated otherwise.</remarks>
        public static bool TryValidateProperty<TModel>(ValidationContext context, TModel model, string propertyName, out ValidatableResult validationResult) where TModel : class
        {
            validationResult = null;
            if (context == null)
                return false;

            validationResult = null;
            var prop = model
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetSetMethod() != null && x.MemberType == MemberTypes.Property)
                .FirstOrDefault(x => x.Name == propertyName);
            if (prop == null)
                return false;

            var tempValidation = new List<ValidationResult>();

            var key = prop.Name;
            if (key.Equals("Item") && prop.PropertyType == typeof(object))
                return false;

            var value = prop.GetValue(model);

            context.MemberName = key;
            Validator.TryValidateProperty(value, context, tempValidation);
            validationResult = ToValidatableResult(tempValidation, key);
            context.MemberName = null;

            return validationResult == null;
        }

        /// <summary>
        /// Validates a property manually, based on Data Annotations.
        /// </summary>
        /// <param name="context">A <see cref="ValidationContext"/></param>
        /// <param name="propertyName">A <see cref="string"/> value that representing name of <see cref="PropertyInfo"/></param>
        /// <param name="validationResult">An <see cref="ValidatableResult"/> object that representing errors found based on Annotations rules.</param>
        /// <returns>A <see cref="bool"/> value</returns>
        /// <remarks>If true property value has been validated, and not validated otherwise.</remarks>
        public bool TryValidateProperty(ValidationContext context, string propertyName,
            out ValidatableResult validationResult) =>
            TryValidateProperty(context, this, propertyName, out validationResult);

        /// <summary>
        /// Validates a property manually, based on Data Annotations.
        /// </summary>
        /// <typeparam name="TModel">An object type that containing specific property.</typeparam>
        /// <param name="property">An <see cref="Expression{TDelegate}"/> that representing specific property in model.</param>
        /// <param name="validationResult">An <see cref="ValidatableResult"/> object that representing errors found based on Annotations rules.</param>
        /// <returns>A <see cref="bool"/> value</returns>
        /// <remarks>If true property value has been validated, and not validated otherwise.</remarks>
        public static bool TryValidateProperty<TModel>(Expression<Func<TModel, object>> property, out ValidatableResult validationResult) where TModel : class
        {
            var prop = property.GetPropertyInfo();
            var key = prop.Name;

            TModel instance;
            try
            {
                instance = Activator.CreateInstance<TModel>();
            }
            catch (Exception e)
            {
                validationResult = null;
                return false;
            }

            var context = new ValidationContext(instance);
            return TryValidateProperty(context, instance, key, out validationResult);
        }

        /// <summary>
        /// Validates a property manually, based on Data Annotations.
        /// </summary>
        /// <typeparam name="TModel">An object type that containing specific property.</typeparam>
        /// <typeparam name="T">A generic type that need to be validated.</typeparam>
        /// <param name="property">An <see cref="Expression{TDelegate}"/> that representing specific property in model.</param>
        /// <param name="value">An object that representing value to be validated.</param>
        /// <param name="validatableResult">An <see cref="ValidatableResult"/> object that representing errors found based on Annotations rules.</param>
        /// <returns>A <see cref="bool"/> value</returns>
        /// <remarks>If true property value has been validated, and not validated otherwise.</remarks>
        public static bool TryValidateProperty<TModel, T>(Expression<Func<TModel, T>> property, T value, out ValidatableResult validatableResult) where TModel : class
        {
            var prop = property.GetPropertyInfo();
            var key = prop.Name;

            TModel instance;
            try
            {
                instance = Activator.CreateInstance<TModel>();
            }
            catch (Exception e)
            {
                validatableResult = null;
                return false;
            }

            var context = new ValidationContext(instance);
            return TryValidateProperty<TModel>(context, key, value, out validatableResult);
        }

        /// <summary>
        /// Validates object properties based on Data Annotations in given context.
        /// </summary>
        /// <typeparam name="TModel">An object type that need to be validated.</typeparam>
        /// <param name="context">A <see cref="ValidationContext"/></param>
        /// <param name="model">An object to be validated.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>If true object has been validated, and not validated otherwise.</returns>
        public static ValidatableResultCollection Validate<TModel>(ValidationContext context, TModel model) where TModel : class
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var validationErrors = new ValidatableResultCollection();
            if (model == null)
                return validationErrors;

            var props = model.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetSetMethod() != null)
                .Where(x => x.MemberType == MemberTypes.Property)
                .ToList();
            if (props.Count == 0)
                return validationErrors;

            foreach (var prop in props)
            {
                var tempValidation = new List<ValidationResult>();

                var key = prop.Name;
                if (key.Equals("Item") && prop.PropertyType == typeof(object))
                    continue;

                var value = prop.GetValue(model);

                context.MemberName = key;
                Validator.TryValidateProperty(value, context, tempValidation);

                if (tempValidation.Count > 0)
                {
                    var val = ToValidatableResult(tempValidation, key);
                    if (val != null)
                        validationErrors.Add(val);
                }

                context.MemberName = null;
            }

            return validationErrors;
        }

        /// <summary>
        /// Converts a complex <see cref="ValidatableResult"/> object to a collection of <see cref="ValidationResult"/>.
        /// </summary>
        /// <param name="validationResult">A <see cref="ValidatableResult"/> object that representing a collection of errors.</param>
        /// <returns>A <see cref="ValidatableResult"/> object.</returns>
        public static IEnumerable<ValidationResult> FromValidatableResult(ValidatableResult validationResult)
        {
            var (name, errors) = validationResult;
            return from validatableResultError in errors
                   select new ValidationResult(validatableResultError, new List<string>
                {
                    name
                });
        }

        /// <summary>
        /// Converts a collection of <see cref="ValidationResult"/> to a <see cref="ValidatableResult"/> object.
        /// </summary>
        /// <param name="results">A <see cref="IEnumerable{T}"/> for validation validationResult.</param>
        /// <param name="key">a <see cref="string"/> value that represents property name.</param>
        /// <returns>A <see cref="ValidatableResult"/> object.</returns>
        public static ValidatableResult ToValidatableResult(IEnumerable<ValidationResult> results,
          string key)
        {
            var validationResults = results.ToList();
            if (validationResults.Count == 0)
                return null;

            var errors = validationResults
                .Where(x => x != null && !string.IsNullOrEmpty(x.ErrorMessage))
                .Select(x => x.ErrorMessage)
                .ToList();
            return new ValidatableResult(key, errors);
        }

        /// <summary>
        /// A <see cref="ValidatableResultCollection"/> that holds failed-validation information.
        /// </summary>
        [NotMapped, EditorBrowsable(EditorBrowsableState.Never)]
        public ValidatableResultCollection ValidationErrors;

        /// <summary>
        /// Determines whether the specified object is valid.
        /// </summary>
        /// <returns>If true, object is valid, not valid otherwise.</returns>
        public virtual bool Validate()
        {
            var check = TryValidate(this, out var validationResults);
            ValidationErrors = validationResults;
            return check;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var resultCollection = Validate(validationContext, this);
            return resultCollection?.Any() == true
                ? resultCollection.SelectMany(FromValidatableResult)
                : new List<ValidationResult>();
        }
    }
}