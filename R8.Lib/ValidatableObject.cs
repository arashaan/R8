using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace R8.Lib
{
    public class ValidatableObject<TModel> : ValidatableObject where TModel : class
    {
        public bool ValidateProperty<TProperty>(Expression<Func<TModel, TProperty>> property, out ValidatableResult results)
        {
            var prop = property.GetProperty();
            var key = prop.Name;

            var model = this as TModel;
            var value = property.Compile().Invoke(model);

            var context = new ValidationContext(this);
            var tempValidation = new List<ValidationResult>();
            context.MemberName = key;
            Validator.TryValidateProperty(value, context, tempValidation);
            var errors = tempValidation.Select(x => x.ErrorMessage).ToList();

            if (tempValidation.Count > 0)
            {
                results = new ValidatableResult(key, errors);
                return false;
            }

            results = null;
            return true;
        }
    }

    public abstract class ValidatableObject
    {
        public static bool Validate<TModel>(TModel model)
        {
            return TryValidate(model, out _);
        }

        public static bool ValidateProperty<TModel, TObject>(Expression<Func<TModel, TObject>> property, TObject arg, out ValidatableResult results)
        {
            if (arg == null)
            {
                results = null;
                return false;
            }

            var prop = property.GetProperty();
            var key = prop.Name;

            var instance = Activator.CreateInstance<TModel>();
            var context = new ValidationContext(instance);
            var tempValidation = new List<ValidationResult>();

            context.MemberName = key;
            Validator.TryValidateProperty(arg, context, tempValidation);
            var errors = tempValidation.Select(x => x.ErrorMessage).ToList();

            if (tempValidation.Count > 0)
            {
                results = new ValidatableResult(key, errors);
                return false;
            }

            results = null;
            return true;
        }

        public static bool TryValidate<TModel>(TModel model, out ValidatableResultCollection validationErrors)
        {
            validationErrors = new ValidatableResultCollection();

            if (model == null)
                return validationErrors.Count == 0;

            var type = model.GetType();
            var props = type
              .GetProperties(BindingFlags.Instance | BindingFlags.Public)
              .Where(x => x.GetSetMethod() != null)
              .Where(x => x.MemberType == MemberTypes.Property)
              .ToList();
            if (props.Count == 0)
                return false;

            var context = new ValidationContext(model);
            foreach (var prop in props)
            {
                var tempValidation = new List<ValidationResult>();

                var key = prop.Name;
                if (key.Equals("Item") && prop.PropertyType == typeof(object))
                    continue;

                var value = prop.GetValue(model);

                context.MemberName = key;
                Validator.TryValidateProperty(value, context, tempValidation);

                if (tempValidation?.Any() == true)
                {
                    var val = ToValidationResults(tempValidation, key);
                    if (val != null)
                        validationErrors.Add(val);
                }

                context.MemberName = null;
            }

            return validationErrors.Count == 0;
        }

        public static ValidatableResult ToValidationResults(IReadOnlyCollection<ValidationResult> results,
          string key)
        {
            var errors = results.Where(x => x != null && !string.IsNullOrEmpty(x.ErrorMessage)).Select(x => x.ErrorMessage)
              .ToList();
            var final = new ValidatableResult(key, errors);
            return final;
        }

        [NotMapped, EditorBrowsable(EditorBrowsableState.Never)]
        public ValidatableResultCollection ValidationErrors;

        public bool Validate()
        {
            var check = TryValidate(this, out var validationResults);
            ValidationErrors = validationResults;
            return check;
        }
    }
}