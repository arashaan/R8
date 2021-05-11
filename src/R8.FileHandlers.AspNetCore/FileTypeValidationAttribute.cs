using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace R8.FileHandlers.AspNetCore
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FileTypeValidationAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly List<string> _extensions;

        public List<IFormFile> AllowedFiles { get; }

        public FileTypeValidationAttribute(params string[] fileTypes)
        {
            if (fileTypes == null || !fileTypes.Any())
                throw new ArgumentNullException(nameof(fileTypes));

            _extensions = fileTypes?.Where(x => !string.IsNullOrEmpty(x))?.Select(x => x.ToLowerInvariant()).ToList();
            if (_extensions == null || !_extensions.Any())
                throw new NullReferenceException("Extracted list of extensions are empty. Please try review your given file types.");
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (_extensions?.Any() != true)
                throw new NullReferenceException(nameof(_extensions));

            if (value == null)
                return ValidationResult.Success;

            var finalFiles = new List<IFormFile>();
            switch (value)
            {
                case IFormFileCollection fileCollection:
                    {
                        if (fileCollection?.Any() == true)
                        {
                            finalFiles.AddRange(from file in fileCollection
                                let valid = file.TryValidateByExtensions(true, _extensions.ToArray())
                                where valid
                                select file);
                        }
                        break;
                    }
                case IEnumerable<IFormFile> files:
                    {
                        if (files?.Any() == true)
                        {
                            finalFiles.AddRange(from file in files
                                let valid = file.TryValidateByExtensions(true, _extensions.ToArray())
                                where valid
                                select file);
                        }
                        break;
                    }

                case IFormFile file:
                    {
                        var valid = file.TryValidateByExtensions(true, _extensions.ToArray());
                        if (valid) finalFiles.Add(file);
                        break;
                    }
                default:
                    return new ValidationResult(ErrorMessageString);
            }

            var result = finalFiles.Count == 0
              ? new ValidationResult(ErrorMessageString)
              : ValidationResult.Success;
            return result;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.Attributes.Add("accept",
                string.Join(",", _extensions.Select(x => Extensions.GetMimeType(x.StartsWith(".") ? x : $".{x}"))));
        }
    }
}