using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

using R8.AspNetCore.FileHandlers;
using R8.Lib;

namespace R8.AspNetCore.Test.FakeObjects
{
    public class FakeValidatableFile : ValidatableObject
    {
        [FileTypeValidation("png")]
        public IFormFile File { get; set; }

        [FileTypeValidation("png", "jpg")]
        public IFormFile File3 { get; set; }

        [FileTypeValidation("png", "jpg")]
        public List<IFormFile> Files { get; set; }

        [FileTypeValidation]
        public IFormFile File2 { get; set; }
    }
}