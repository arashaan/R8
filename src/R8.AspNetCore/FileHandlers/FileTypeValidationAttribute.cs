using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using MimeKit;

namespace R8.AspNetCore.FileHandlers
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
                            foreach (var file in fileCollection)
                            {
                                var valid = file.TryValidateByExtensions(_extensions.ToArray());
                                if (valid) finalFiles.Add(file);
                            }
                        }
                        break;
                    }
                case IEnumerable<IFormFile> files:
                    {
                        if (files?.Any() == true)
                        {
                            foreach (var file in files)
                            {
                                var valid = file.TryValidateByExtensions(_extensions.ToArray());
                                if (valid) finalFiles.Add(file);
                            }
                        }
                        break;
                    }

                case IFormFile file:
                    {
                        var valid = file.TryValidateByExtensions(_extensions.ToArray());
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
                string.Join(",", _extensions.Select(x => MimeTypes.GetMimeType(x.StartsWith(".") ? x : $".{x}"))));
        }
    }
}