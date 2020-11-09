using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using R8.FileHandlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R8.AspNetCore.FileHandlers
{
    public static class WebFileHandlers
    {
        // /// <summary>
        // /// Saves and uploads given file into the host.
        // /// </summary>
        // /// <param name="file">An <see cref="IFormFile"/> interface that representing input file stream to save.</param>
        // /// <param name="config">A <see cref="Action{TResult}"/> instance that representing configurations to save.</param>
        // /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        // public static async Task<IMyFile> UploadImageAsync(this IFormFile file, Action<MyFileConfigurationImage> config)
        // {
        //     if (file == null)
        //         return null;
        //
        //     var f = new MyFileConfigurationImage();
        //     config.Invoke(f);
        //     try
        //     {
        //         await using var stream = file.OpenReadStream();
        //         return await stream.SaveAsync(file.FileName, config).ConfigureAwait(false);
        //     }
        //     catch (Exception)
        //     {
        //         return null;
        //     }
        // }

        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <typeparam name="TConfiguration">An generic type <see cref="MyFileConfiguration"/>.</typeparam>
        /// <param name="file">An <see cref="IFormFile"/> interface that representing input file stream to save.</param>
        /// <param name="config">A <see cref="Action{TResult}"/> instance that representing configurations to save.</param>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        public static async Task<IMyFile> UploadAsync<TConfiguration>(this IFormFile file, Action<TConfiguration> config) where TConfiguration : MyFileConfiguration
        {
            if (config == null)
                return null;
            if (file == null)
                return null;

            var newConfig = Activator.CreateInstance<TConfiguration>();
            var environment = newConfig.GetService<IWebHostEnvironment>();
            newConfig.Folder = environment.WebRootPath + "/uploads";
            config.Invoke(newConfig);
            try
            {
                await using var stream = file.OpenReadStream();
                return await stream.SaveAsync(file.FileName, newConfig).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <typeparam name="TConfiguration">An generic type <see cref="MyFileConfiguration"/>.</typeparam>
        /// <param name="files">An <see cref="IFormFileCollection"/> interface that representing input file streams to save.</param>
        /// <param name="config">A <see cref="Action{TResult}"/> instance that representing configurations to save.</param>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        public static Task<List<IMyFile>> UploadAsync<TConfiguration>(this IFormFileCollection files, Action<TConfiguration> config) where TConfiguration : MyFileConfiguration
        {
            return files.ToList().UploadAsync(config);
        }

        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <typeparam name="TConfiguration">An generic type <see cref="MyFileConfiguration"/>.</typeparam>
        /// <param name="files">An collection of <see cref="IFormFile"/> that representing input file streams to save.</param>
        /// <param name="config">A <see cref="Action{TResult}"/> instance that representing configurations to save.</param>
        /// <returns>A <see cref="Task{TResult}"/> object that representing asynchronous operation.</returns>
        public static async Task<List<IMyFile>> UploadAsync<TConfiguration>(this List<IFormFile> files,
            Action<TConfiguration> config) where TConfiguration : MyFileConfiguration
        {
            if (files?.Any() != true)
                return null;

            var validFiles = files.Where(x => x.Length > 0).ToList();
            if (validFiles?.Any() != true)
                return null;

            var list = new List<IMyFile>();
            foreach (var validFile in validFiles)
            {
                var file = await validFile.UploadAsync(config).ConfigureAwait(false);
                if (file != null && !string.IsNullOrEmpty(file.FilePath))
                    list.Add(file);
            }

            return list.Where(x => x != null).ToList();
        }
    }
}