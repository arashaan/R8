using Microsoft.AspNetCore.Http;

using R8.Lib.FileHandlers;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace R8.Lib.AspNetCore.FileHandlers
{
    public static class WebFileValidator
    {
        public static bool ValidateFile<TModel>(this ValidatableObject<TModel> model, Expression<Func<TModel, List<IFormFile>>> property, List<IFormFile> files, FileTypes type,
            out ValidatableResultCollection results) where TModel : ValidatableObject
        {
            results = new ValidatableResultCollection();

            var isValid = ValidatableObject.ValidateProperty(property, files, out var errors);
            results.Add(errors);

            if (!isValid)
                return false;

            var validationContext = new ValidationContext(model) { MemberName = property.Name };
            var validationAttribute = new FileTypeValidationAttribute(type);
            var valid = validationAttribute.GetValidationResult(files, validationContext);
            if (valid == null)
                return true;

            var error = validationAttribute.FormatErrorMessage(null);
            results.Add(new ValidatableResult(property.Name, new[] { error }.ToList()));
            return false;
        }
    }
}