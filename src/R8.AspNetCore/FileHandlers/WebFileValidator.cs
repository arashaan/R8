using Microsoft.AspNetCore.Http;

using R8.Lib.Validatable;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using R8.FileHandlers;
using SixLabors.ImageSharp.Formats.Jpeg;

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
        /// Checks file extension validation by given extensions
        /// </summary>
        /// <param name="file">An <see cref="IFormFile"/> object to check if it is validated.</param>
        /// <param name="extensions">An array of file extensions.</param>
        /// <returns></returns>
        public static async Task<bool> TryValidateByExtensionsAsync(this IFormFile file, params string[] extensions)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            if (file.Length == 0)
                return false;

            if (extensions == null || !extensions.Any())
                return true;

            var fileName = file.FileName;
            var fileExtension = Path.GetExtension(fileName);
            fileExtension = fileExtension.StartsWith(".")
                ? fileExtension[1..]
                : fileExtension;

            var matches = 0;
            foreach (var extension in extensions)
            {
                var ext = extension.StartsWith(".") ? extension[1..] : extension;
                if (ext.Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase))
                    matches++;
            }

            if (matches == 0)
                return false;

            switch (fileExtension)
            {
                case "bmp":
                case "jpg":
                case "tiff":
                case "jpeg":
                case "gif":
                case "png":
                    {
                        await using var fileStream = file.OpenReadStream();
                        return await fileStream.IsImageAsync();
                    }

                case "pdf":
                    {
                        await using var fileStream = file.OpenReadStream();
                        return await fileStream.IsPdfAsync();
                    }

                case "zip":
                case "rar":
                    {
                        await using var fileStream = file.OpenReadStream();
                        return fileStream.IsArchive(true);
                    }

                case "doc":
                case "docx":
                case "ppt":
                case "pptx":
                case "xls":
                case "xlsx":
                case "mp4":
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks file extension validation by given extensions
        /// </summary>
        /// <param name="file">An <see cref="IFormFile"/> object to check if it is validated.</param>
        /// <param name="extensions">An array of file extensions.</param>
        /// <returns></returns>
        public static bool TryValidateByExtensions(this IFormFile file, params string[] extensions)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            if (file.Length == 0)
                return false;

            if (extensions == null || !extensions.Any())
                return true;

            var fileName = file.FileName;
            var fileExtension = Path.GetExtension(fileName);
            fileExtension = fileExtension.StartsWith(".")
                ? fileExtension[1..]
                : fileExtension;

            var matches = 0;
            foreach (var extension in extensions)
            {
                var ext = extension.StartsWith(".") ? extension[1..] : extension;
                if (ext.Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase))
                    matches++;
            }

            if (matches == 0)
                return false;

            switch (fileExtension)
            {
                case "bmp":
                case "jpg":
                case "tiff":
                case "jpeg":
                case "gif":
                case "png":
                    {
                        using var fileStream = file.OpenReadStream();
                        return fileStream.IsImage();
                    }

                case "pdf":
                    {
                        using var fileStream = file.OpenReadStream();
                        return fileStream.IsPdf();
                    }

                case "zip":
                case "rar":
                    {
                        using var fileStream = file.OpenReadStream();
                        return fileStream.IsArchive(true);
                    }

                case "doc":
                case "docx":
                case "ppt":
                case "pptx":
                case "xls":
                case "xlsx":
                case "mp4":
                    return true;

                default:
                    return false;
            }
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