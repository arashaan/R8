using Microsoft.AspNetCore.Http;
using R8.Lib.Validatable;

namespace R8.FileHandlers.AspNetCore.Test.FakeObjects
{
    public class FakeValidatableFile3 : ValidatableObject
    {
        [FileTypeValidation]
        public IFormFile File2 { get; set; }
    }
}