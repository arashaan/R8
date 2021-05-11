using Microsoft.AspNetCore.Http;

using R8.Lib;
using R8.Lib.Validatable;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace R8.FileHandlers.AspNetCore
{
    public static class WebFileValidator
    {
        /// <summary>
        /// Checks file extension validation by given extensions
        /// </summary>
        /// <param name="file">An <see cref="IFormFile"/> object to check if it is validated.</param>
        /// <param name="deepCheck">Checks stream based on given file extension.</param>
        /// <param name="allowedExtensions">An array of file extensions.</param>
        /// <returns></returns>
        public static async Task<bool> TryValidateByExtensionsAsync(this IFormFile file, bool deepCheck, params string[] allowedExtensions)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            if (file.Length == 0)
                return false;

            if (allowedExtensions == null || !allowedExtensions.Any())
                return true;

            allowedExtensions = allowedExtensions
                .Select(extension => extension.StartsWith(".") ? extension[1..] : extension)
                .ToArray();
            var fileName = file.FileName;
            var fileExtension = Path.GetExtension(fileName);
            fileExtension = fileExtension.StartsWith(".")
                ? fileExtension[1..]
                : fileExtension;

            var matchedExtension = allowedExtensions.Any(allowedExtension =>
                allowedExtension.Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase));
            if (!matchedExtension)
                return false;

            if (!deepCheck)
                return true;

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
                case "svg":
                    {
                        await using var fileStream = file.OpenReadStream();
                        return fileStream.IsSvg();
                    }
                case "zip":
                case "rar":
                    {
                        await using var fileStream = file.OpenReadStream();
                        return fileStream.IsArchive(true);
                    }

                case "doc":
                case "docx":
                    {
                        await using var fileStream = file.OpenReadStream();
                        return fileStream.IsWordDoc();
                    }
                case "ppt":
                case "pptx":
                    {
                        await using var fileStream = file.OpenReadStream();
                        return fileStream.IsPowerPoint();
                    }
                case "xls":
                case "xlsx":
                    {
                        await using var fileStream = file.OpenReadStream();
                        return fileStream.IsExcel();
                    }
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
        /// <param name="deepCheck">Checks stream based on given file extension.</param>
        /// <param name="allowedExtensions">An array of file extensions.</param>
        /// <returns></returns>
        public static bool TryValidateByExtensions(this IFormFile file, bool deepCheck, params string[] allowedExtensions)
        {
            return file.TryValidateByExtensionsAsync(deepCheck, allowedExtensions).GetAwaiter().GetResult();
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
        public static bool TryValidateFile<TModel>(string propertyName,
            IEnumerable<IFormFile> files,
            out ValidatableResultCollection validatableResultCollection) where TModel : class
        {
            return TryValidateFileCore<TModel>(propertyName, files, out validatableResultCollection);
        }

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
            return TryValidateFileCore<TModel>(propertyName, file, out validatableResultCollection);
        }

        internal static bool TryValidateFileCore<TModel>(string propertyName,
            object files,
            out ValidatableResultCollection validatableResultCollection) where TModel : class
        {
            validatableResultCollection = new ValidatableResultCollection();

            if (string.IsNullOrEmpty(propertyName))
                return false;

            if (files == null)
                return false;

            var model = Activator.CreateInstance<TModel>();
            var validationContext = new ValidationContext(model) { MemberName = propertyName };

            var property = model.GetType().GetProperty(propertyName);
            if (property == null)
                return false;

            switch (files)
            {
                case IFormFile iFormFile:
                    {
                        if (iFormFile.Length <= 0)
                            return false;
                        break;
                    }
                case IEnumerable<IFormFile> iFormFiles:
                    {
                        files = iFormFiles?.Where(x => x != null && x.Length > 0).ToList();
                        if (!iFormFiles.Any())
                            return false;
                        break;
                    }
                default:
                    {
                        var underlyingList = property.PropertyType.GetEnumerableUnderlyingType();
                        if (underlyingList == null || underlyingList != typeof(IFormFile))
                            return false;
                        break;
                    }
            }

            var validationAttribute = property.GetCustomAttribute<FileTypeValidationAttribute>();
            if (validationAttribute == null)
                return false;

            var validationResult = validationAttribute.GetValidationResult(files, validationContext);
            if (validationResult == null)
                return true;

            var error = validationAttribute.FormatErrorMessage(null);
            validatableResultCollection.Add(new ValidatableResult(propertyName, new[] { error }.ToList()));
            return false;
        }
    }
}