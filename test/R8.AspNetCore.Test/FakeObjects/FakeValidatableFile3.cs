using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Http;

using R8.AspNetCore.FileHandlers;
using R8.Lib.Validatable;

namespace R8.AspNetCore.Test.FakeObjects
{
    public class FakeValidatableFile3 : ValidatableObject
    {
        [FileTypeValidation]
        public IFormFile File2 { get; set; }
    }
}