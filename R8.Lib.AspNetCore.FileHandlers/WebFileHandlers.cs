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
        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <param name="file">An <see cref="IFormFile"/> interface that representing input file stream to save.</param>
        /// <param name="config">A <see cref="Action{TResult}"/> instance that representing configurations to save.</param>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        public static async Task<MyFile> UploadImageAsync(this IFormFile file, Action<MyFileConfigurationImage> config)
        {
            if (file == null)
                return null;

            try
            {
                await using var stream = file.OpenReadStream();
                return await stream.SaveAsync(file.FileName, config).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <typeparam name="TConfiguration">An generic type <see cref="MyFileConfiguration"/>.</typeparam>
        /// <param name="file">An <see cref="IFormFile"/> interface that representing input file stream to save.</param>
        /// <param name="config">A <see cref="Action{TResult}"/> instance that representing configurations to save.</param>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        public static async Task<MyFile> UploadAsync<TConfiguration>(this IFormFile file, Action<TConfiguration> config) where TConfiguration : MyFileConfiguration
        {
            if (file == null)
                return null;

            try
            {
                await using var stream = file.OpenReadStream();
                return await stream.SaveAsync(file.FileName, config);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <param name="files">An <see cref="IFormFileCollection"/> interface that representing input file streams to save.</param>
        /// <param name="config">A <see cref="Action{TResult}"/> instance that representing configurations to save.</param>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        public static Task<List<MyFile>> UploadAsync(this IFormFileCollection files, Action<MyFileConfiguration> config)
        {
            return files.ToList().UploadAsync(config);
        }

        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <typeparam name="TConfiguration">An generic type <see cref="MyFileConfiguration"/>.</typeparam>
        /// <param name="files">An collection of <see cref="IFormFile"/> that representing input file streams to save.</param>
        /// <param name="config">A <see cref="Action{TResult}"/> instance that representing configurations to save.</param>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
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

            return list.Where(x => !string.IsNullOrEmpty(x)).ToList();
        }
    }
}