using Microsoft.AspNetCore.Http;

using System.IO;

namespace R8.FileHandlers.AspNetCore.Test.FakeObjects
{
    public static class Extensions
    {
        public static IFormFile GetFormFile(string fileName)
        {
            var memoryStream = new MemoryStream();
            using var fileStream = new FileStream(fileName, FileMode.Open);
            fileStream.CopyTo(memoryStream);

            var formFile = new FormFile(memoryStream, 0, memoryStream.Length, null, Path.GetFileName(fileName))
            {
                Headers = new HeaderDictionary(),
                ContentType = R8.FileHandlers.Extensions.GetMimeType(Path.GetFileName(fileName))
            };
            return formFile;
        }
    }
}