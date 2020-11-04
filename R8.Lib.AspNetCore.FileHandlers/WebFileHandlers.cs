using Microsoft.AspNetCore.Http;

using R8.Lib.FileHandlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R8.Lib.AspNetCore.FileHandlers
{
    public static class WebFileHandlers
    {
        public static async Task<MyFile> UploadImageAsync(this IFormFile file, Action<MyFileConfigurationImage> config)
        {
            if (file == null)
                return null;

            var result = await file.OpenReadStream().SaveAsync(file.FileName, config).ConfigureAwait(false);
            return result;
        }

        private static Task<MyFile> UploadAsync<TConfiguration>(this IFormFile file, Action<TConfiguration> config = null) where TConfiguration : MyFileConfiguration
        {
            using var stream = file.OpenReadStream();
            return stream.SaveAsync(file.FileName, config);
        }

        public static Task<List<MyFile>> UploadAsync(this IFormFileCollection files, Action<MyFileConfiguration> config = null)
        {
            return files.ToList().UploadAsync(config);
        }

        public static async Task<List<MyFile>> UploadAsync<TConfiguration>(this List<IFormFile> files,
            Action<TConfiguration> config) where TConfiguration : MyFileConfiguration
        {
            if (files?.Any() != true)
                return null;

            var validFiles = files.Where(x => x.Length > 0).ToList();
            if (validFiles?.Any() != true)
                return null;

            var list = new List<MyFile>();
            foreach (var validFile in validFiles)
            {
                var file = await validFile.UploadAsync(config).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(file))
                    list.Add(file);
            }

            var result = list.Where(x => !string.IsNullOrEmpty(x)).ToList();
            return result;
        }
    }
}