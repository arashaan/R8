using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Http;

using R8.AspNetCore.FileHandlers;
using R8.Lib;

namespace R8.AspNetCore.Test.FakeObjects
{
    public class FakeValidateImageSize : ValidatableObject
    {
        [ImageSize(Height = 1000, Width = 1000)]
        public IFormFile? File { get; set; }

        [ImageSize(Height = 100, Width = 100)]
        public IFormFile? File2 { get; set; }
    }
}