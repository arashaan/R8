using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using R8.Lib.Validatable;

namespace R8.FileHandlers.AspNetCore.Test.FakeObjects
{
    public class FakeValidatableFile : ValidatableObject
    {
        [FileTypeValidation("png")]
        public IFormFile File { get; set; }

        [FileTypeValidation("png", "jpg")]
        public IFormFile File3 { get; set; }

        [FileTypeValidation("png", "jpg")]
        public List<IFormFile> Files { get; set; }
    }
}