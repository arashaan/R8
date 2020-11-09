using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;

namespace R8.AspNetCore.FileHandlers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ImageSizeAttribute : ValidationAttribute
    {
        public int Height;
        public int Width;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is IFormFile file))
                return base.IsValid(value, validationContext);

            var stream = file.OpenReadStream();
            bool result;
            using var image = Image.Load(stream);
            {
                result = Width <= image.Width && Height <= image.Height;
            }

            return result
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessageString);
        }
    }
}