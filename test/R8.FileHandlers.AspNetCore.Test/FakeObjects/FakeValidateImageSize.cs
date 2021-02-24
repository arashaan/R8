using Microsoft.AspNetCore.Http;
using R8.Lib.Validatable;

namespace R8.FileHandlers.AspNetCore.Test.FakeObjects
{
    public class FakeValidateImageSize : ValidatableObject
    {
        [ImageSize(Height = 1000, Width = 1000)]
        public IFormFile? File { get; set; }

        [ImageSize(Height = 100, Width = 100)]
        public IFormFile? File2 { get; set; }
    }
}