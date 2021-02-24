using Microsoft.AspNetCore.Http;
using R8.Lib.Validatable;

namespace R8.FileHandlers.AspNetCore.Test.FakeObjects
{
    public class FakeValidatableFile2 : ValidatableObject<FakeValidatableFile2>
    {
        [FileTypeValidation("png")]
        public IFormFile File { get; set; }

        [FileTypeValidation("png", "jpg")]
        public IFormFile File3 { get; set; }

        [FileTypeValidation]
        public IFormFile File2 { get; set; }
    }
}