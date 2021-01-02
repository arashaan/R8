using Microsoft.AspNetCore.Http;

using R8.Lib;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Path = System.IO.Path;

namespace R8.AspNetCore.FileHandlers
{
    public static class WebFileValidator
    {
        /// <summary>
        /// Validates a property manually, based on Data Annotations.
        /// </summary>
        /// <returns>A <see cref="bool"/> value</returns>
        /// <remarks>If true property value has been validated, and not validated otherwise.</remarks>
        /// <param name="propertyName">An <see cref="string"/> value that representing name of given specific property.</param>
        /// <param name="file">A <see cref="IFormFile"/> object to being validated by the given rules.</param>
        /// <param name="validatableResultCollection">An <see cref="ValidatableResultCollection"/> object that representing errors found based on Annotations rules.</param>
        /// <remarks>If true file has been validated, and not validated otherwise.</remarks>
        public static bool TryValidateFile<TModel>(string propertyName, IFormFile file,
            out ValidatableResultCollection validatableResultCollection) where TModel : class
        {
            validatableResultCollection = new ValidatableResultCollection();

            if (string.IsNullOrEmpty(propertyName))
                return false;

            if (file == null || file.Length == 0)
                return false;

            var model = Activator.CreateInstance<TModel>();
            var validationContext = new ValidationContext(model) { MemberName = propertyName };

            var property = model.GetType().GetProperty(propertyName);
            if (property == null)
                return false;

            if (property.PropertyType != typeof(IFormFile) && property.PropertyType != typeof(List<IFormFile>) &&
                property.PropertyType != typeof(IFormFileCollection))
                return false;

            var validationAttribute = property.GetCustomAttribute<FileTypeValidationAttribute>();
            if (validationAttribute == null)
                return false;

            var validationResult = validationAttribute.GetValidationResult(file, validationContext);
            if (validationResult == null)
                return true;

            var error = validationAttribute.FormatErrorMessage(null);
            validatableResultCollection.Add(new ValidatableResult(propertyName, new[] { error }.ToList()));
            return false;
        }

        /// <summary>
        /// Validates a property manually, based on Data Annotations.
        /// </summary>
        /// <returns>A <see cref="bool"/> value</returns>
        /// <remarks>If true property value has been validated, and not validated otherwise.</remarks>
        /// <param name="propertyName">An <see cref="string"/> value that representing name of given specific property.</param>
        /// <param name="files">A collection of <see cref="IFormFile"/> objects to being validated by the given rules.</param>
        /// <param name="validatableResultCollection">An <see cref="ValidatableResultCollection"/> object that representing errors found based on Annotations rules.</param>
        /// <remarks>If true file has been validated, and not validated otherwise.</remarks>
        public static bool TryValidateFile<TModel>(string propertyName, List<IFormFile> files,
            out ValidatableResultCollection validatableResultCollection) where TModel : class
        {
            validatableResultCollection = new ValidatableResultCollection();
            if (files == null || !files.Any())
                return false;

            foreach (var file in files)
            {
                var isValid = TryValidateFile<TModel>(propertyName, file, out var tempResults);
                if (!isValid)
                    return false;
            }

            return true;
        }
    }
}