using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace R8.AspNetCore.FileHandlers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FileTypeValidationAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly List<string> _extensions;

        public List<IFormFile> AllowedFiles;

        public FileTypeValidationAttribute(params string[] fileTypes)
        {
            var fullTypes = string.Join("|", fileTypes.ToList());
            _extensions = fullTypes.Split("|").Select(x => x.ToLower()).ToList();
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
                case IEnumerable<IFormFile> files:
                    finalFiles.AddRange(files.ToList());
                    break;

                case IFormFile file:
                    finalFiles.Add(file);
                    break;

                default:
                    return new ValidationResult(ErrorMessageString);
            }

            var allowedFiles = finalFiles?
              .Where(x => x != null)
              .Where(x => x.Length > 0)
              .Where(x => _extensions.Contains(Path.GetExtension(x.FileName).Substring(1).ToLower()));

            AllowedFiles = allowedFiles.ToList();
            var result = AllowedFiles.Count == 0
              ? new ValidationResult(ErrorMessageString)
              : ValidationResult.Success;
            return result;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.Attributes.Add("accept", string.Join(",", _extensions.Select(x => $".{x}")));
        }
    }
}