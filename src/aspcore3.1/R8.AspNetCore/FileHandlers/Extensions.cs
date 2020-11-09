using Microsoft.AspNetCore.Http;

using R8.FileHandlers;

using System;

namespace R8.AspNetCore.FileHandlers
{
    public static class Extensions
    {
        /// <summary>
        /// Represents <see cref="FileTypes"/> for specific file extension
        /// </summary>
        /// <param name="file">An <see cref="IFormFile"/> interface that representing an open stream.</param>
        /// <returns>A <see cref="FileTypes"/> <see cref="Enum"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static FileTypes GetFileType(this IFormFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            var fileName = file.FileName;
            return R8.FileHandlers.Extensions.GetFileType(fileName);
        }
    }
}