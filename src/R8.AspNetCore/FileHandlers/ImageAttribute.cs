using System;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace R8.AspNetCore.FileHandlers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ImageSizeAttribute : ValidationAttribute
    {
        public int Height;
        public int Width;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (!(value is IFormFile file))
                return new ValidationResult(ErrorMessageString);

            var stream = file.OpenReadStream();
            using var image = Image.Load(stream);
            var result = Width <= image.Width && Height <= image.Height;
            return result
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessageString);
        }
    }
}