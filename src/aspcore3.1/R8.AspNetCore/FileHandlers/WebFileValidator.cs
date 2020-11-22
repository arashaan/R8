using Microsoft.AspNetCore.Http;

using R8.Lib;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

using Path = System.IO.Path;

namespace R8.AspNetCore.FileHandlers
{
    public static class WebFileValidator
    {
        /// <summary>
        /// Validates a property manually, based on Data Annotations.
        /// </summary>
        /// <typeparam name="TModel">A generic type that need to be validated.</typeparam>
        /// <returns>A <see cref="bool"/> value</returns>
        /// <remarks>If true property value has been validated, and not validated otherwise.</remarks>
        /// <param name="model">A <see cref="TModel"/> object to be validated.</param>
        /// <param name="property">An <see cref="Expression{TDelegate}"/> that representing specific property in model.</param>
        /// <param name="files"></param>
        /// <param name="validatableResultCollection">An <see cref="ValidatableResultCollection"/> object that representing errors found based on Annotations rules.</param>
        /// <remarks>If true file has been validated, and not validated otherwise.</remarks>
        public static bool TryValidateFile<TModel>(this ValidatableObject<TModel> model, Expression<Func<TModel, List<IFormFile>>> property, List<IFormFile> files,
            out ValidatableResultCollection validatableResultCollection) where TModel : ValidatableObject
        {
            validatableResultCollection = new ValidatableResultCollection();

            var isValid = ValidatableObject.TryValidateProperty(property, files, out var errors);
            validatableResultCollection.Add(errors);

            if (!isValid)
                return false;

            var fileTypes = files.Select(x => Path.GetExtension(x.FileName)[1..]).ToList();
            foreach (var error in from fileType in fileTypes
                                  let validationContext = new ValidationContext(model) { MemberName = property.Name }
                                  let validationAttribute = new FileTypeValidationAttribute(fileType)
                                  let valid = validationAttribute.GetValidationResult(files, validationContext)
                                  where valid != null
                                  select validationAttribute.FormatErrorMessage(null))
            {
                validatableResultCollection.Add(new ValidatableResult(property.Name, new[] { error }.ToList()));
                return false;
            }

            return true;
        }
    }
}