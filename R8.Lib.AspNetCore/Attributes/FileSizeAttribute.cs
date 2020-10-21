using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace R8.Lib.AspNetCore.Attributes
{
  [AttributeUsage(AttributeTargets.Property)]
  public class FileSizeAttribute : ValidationAttribute
  {
    public long Length;

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (!(value is IFormFile file))
        return base.IsValid(value, validationContext);

      return file?.Length <= Length
      ? ValidationResult.Success
      : new ValidationResult(ErrorMessageString);
    }
  }
}