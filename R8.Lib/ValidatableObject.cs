using R8.Lib.Enums;
using R8.Lib.MethodReturn;

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
            var model = this as TModel;
            var value = property.Compile().Invoke(model);
            var checkValidation = ValidateProperty(property, value, out results);
            return checkValidation;
        }
    }

    public abstract class ValidatableObject : IValidatableObject
    {
        public static bool Validate<TModel>(TModel model)
        {
            return TryValidate(model, out _);
        }

        public Response<TSource> ToResponse<TSource>() where TSource : class
        {
            var valid = this.Validate();
            var flags = valid ? Flags.Success : Flags.ModelIsNotValid;
            var result = new Response<TSource>(flags, this.ValidationErrors);
            return result;
        }

        public static bool ValidateProperty<TModel, TObject>(Expression<Func<TModel, TObject>> property, TObject value, out ValidatableResult results)
        {
            if (value == null)
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
            Validator.TryValidateProperty(value, context, tempValidation);
            var errors = tempValidation.ConvertAll(x => x.ErrorMessage);

            if (tempValidation.Count > 0)
            {
                results = new ValidatableResult(key, errors);
                return false;
            }

            results = null;
            return true;
        }

        public static ValidatableResultCollection CheckValidation<TModel>(ValidationContext context, TModel model)
        {
            var validationErrors = new ValidatableResultCollection();
            if (model == null)
                return validationErrors;

            var type = model.GetType();
            var props = type
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
                    var val = ToValidationResults(tempValidation, key);
                    if (val != null)
                        validationErrors.Add(val);
                }

                context.MemberName = null;
            }

            return validationErrors;
        }

        public static bool TryValidate<TModel>(TModel model, out ValidatableResultCollection validationErrors)
        {
            validationErrors = new ValidatableResultCollection();
            var context = new ValidationContext(model);
            validationErrors = CheckValidation(context, model);
            return validationErrors.Count == 0;
        }

        public static ValidatableResult ToValidationResults(IEnumerable<ValidationResult> results,
          string key)
        {
            var errors = results
                .Where(x => x != null && !string.IsNullOrEmpty(x.ErrorMessage))
                .Select(x => x.ErrorMessage)
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var resultCollection = CheckValidation(validationContext, this);
            var validationResults = new List<ValidationResult>();
            if (resultCollection?.Any() == true)
            {
                validationResults.AddRange(from validatableResult in resultCollection
                                           from validatableResultError in validatableResult.Errors
                                           select new ValidationResult(validatableResultError, new List<string>
                    {
                        validatableResult.Name
                    }));
            }

            return validationResults;
        }
    }
}