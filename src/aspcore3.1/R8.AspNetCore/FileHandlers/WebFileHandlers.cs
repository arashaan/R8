using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using R8.FileHandlers;
using R8.Lib;

namespace R8.AspNetCore.FileHandlers
{
    public static class WebFileHandlers
    {
        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <param name="file">An <see cref="IFormFile"/> interface that representing input file stream to save.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        public static Task<IMyFile> UploadAsync(this IFormFile file) =>
            file.UploadAsync(Activator.CreateInstance<MyFileConfiguration>());

        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <param name="stream">An <see cref="Stream"/> object that representing input file stream to save.</param>
        /// <param name="filename">An  <see cref="string"/> value that representing file's name.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        public static Task<IMyFile> UploadAsync(this Stream stream, string filename) =>
            stream.UploadAsync(filename, Activator.CreateInstance<MyFileConfiguration>());

        /// <summary>
        /// Saves and uploads given image file into the host.
        /// </summary>
        /// <param name="file">An <see cref="IFormFile"/> interface that representing input file stream to save.</param>
        /// <param name="config">A <see cref="Action{TResult}"/> instance that representing configurations to save.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        public static Task<IMyFile> UploadImageAsync(this IFormFile file, Action<MyFileConfigurationImage> config) =>
            file.UploadAsync(config);

        /// <summary>
        /// Saves and uploads given image file into the host.
        /// </summary>
        /// <param name="stream">An <see cref="Stream"/> object that representing input file stream to save.</param>
        /// <param name="filename">An  <see cref="string"/> value that representing file's name.</param>
        /// <param name="config">A <see cref="Action{TResult}"/> instance that representing configurations to save.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        public static Task<IMyFile> UploadImageAsync(this Stream stream, string filename, Action<MyFileConfigurationImage> config) =>
            stream.UploadAsync(filename, config);

        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <typeparam name="TConfiguration">An generic type <see cref="MyFileConfiguration"/>.</typeparam>
        /// <param name="file">An <see cref="IFormFile"/> interface that representing input file stream to save.</param>
        /// <param name="config">A <see cref="Action{TResult}"/> instance that representing configurations to save.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        public static Task<IMyFile> UploadAsync<TConfiguration>(this IFormFile file, Action<TConfiguration> config) where TConfiguration : MyFileConfiguration
        {
            var configuration = Activator.CreateInstance<TConfiguration>();
            config(configuration);
            return file.UploadAsync(configuration);
        }

        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <typeparam name="TConfiguration">An generic type <see cref="MyFileConfiguration"/>.</typeparam>
        /// <param name="stream">An <see cref="Stream"/> object that representing input file stream to save.</param>
        /// <param name="filename">An  <see cref="string"/> value that representing file's name.</param>
        /// <param name="config">A <see cref="Action{TResult}"/> instance that representing configurations to save.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        public static Task<IMyFile> UploadAsync<TConfiguration>(this Stream stream, string filename, Action<TConfiguration> config) where TConfiguration : MyFileConfiguration
        {
            var configuration = Activator.CreateInstance<TConfiguration>();
            config(configuration);
            return stream.UploadAsync(filename, configuration);
        }

        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <typeparam name="TConfiguration">An generic type <see cref="MyFileConfiguration"/>.</typeparam>
        /// <param name="file">An <see cref="IFormFile"/> interface that representing input file stream to save.</param>
        /// <param name="configuration"></param>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        internal static Task<IMyFile> UploadAsync<TConfiguration>(this IFormFile file, TConfiguration configuration)
            where TConfiguration : MyFileConfiguration =>
            file.OpenReadStream().UploadAsync(file.FileName, configuration);

        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <typeparam name="TConfiguration">An generic type <see cref="MyFileConfiguration"/>.</typeparam>
        /// <param name="stream">An <see cref="Stream"/> object that representing input file stream to save.</param>
        /// <param name="filename">An  <see cref="string"/> value that representing file's name.</param>
        /// <param name="configuration"></param>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        internal static async Task<IMyFile> UploadAsync<TConfiguration>(this Stream stream, string filename, TConfiguration configuration) where TConfiguration : MyFileConfiguration
        {
            if (stream == null)
                return null;
            if (filename == null)
                return null;
            if (configuration == null)
                return null;

            var options = FileHandlersConnection.Options;
            var environment = options.Environment;
            if (environment == null)
                throw new NullReferenceException($"{nameof(AddFileHandlersExtensions.AddFileHandlers)} must be registered in dependencies injection.");

            var baseUrl = environment.WebRootPath;
            if (baseUrl.EndsWith("/"))
                baseUrl = baseUrl[..^1];

            configuration.Internal ??= new Dictionary<string, object>();
            configuration.Internal["baseUrl"] = baseUrl;
            configuration.Internal["isWeb"] = true;
            configuration.Path ??= options.Path;
            configuration.HierarchicallyDateFolders ??= options.HierarchicallyDateFolders;
            configuration.OverwriteExistingFile ??= options.OverwriteExistingFile;
            configuration.SaveAsRealName ??= options.SaveAsRealName;

            IMyFile? file;
            switch (configuration)
            {
                case MyFileConfigurationImage imageConfig:
                    {
                        var imageConfiguration = options.Runtimes.OfType<IMyFileConfigurationImageBase>()
                            .FirstOrDefault();
                        if (imageConfiguration == null)
                            throw new NullReferenceException($"{nameof(IMyFileConfigurationImageBase)} must be added to configurations.");

                        imageConfig.WatermarkPath ??= imageConfiguration.WatermarkPath;

                        if (imageConfig.ImageEncoder == null)
                        {
                            if (imageConfiguration.ImageEncoder == null)
                                throw new NullReferenceException($"{nameof(IMyFileConfigurationImageBase.ImageEncoder)} has not to be null.");

                            imageConfig.ImageEncoder = imageConfiguration.ImageEncoder;
                        }
                        imageConfig.ResizeToSize ??= imageConfiguration.ResizeToSize;

                        file = await stream.SaveAsync(filename, imageConfig).ConfigureAwait(false);
                        break;
                    }
                case MyFileConfigurationPdf pdfConfig:
                    {
                        var pdfConfiguration = options.Runtimes.OfType<IMyFileConfigurationPdfBase>().FirstOrDefault();
                        if (pdfConfiguration == null)
                            throw new NullReferenceException($"{nameof(IMyFileConfigurationPdfBase)} must be added to configurations.");

                        pdfConfig.GhostScriptDllPath ??= pdfConfiguration.GhostScriptDllPath;
                        pdfConfig.ImageQuality ??= pdfConfiguration.ImageQuality;
                        pdfConfig.ResolutionDpi ??= pdfConfiguration.ResolutionDpi;

                        file = await stream.SaveAsync(filename, pdfConfig).ConfigureAwait(false);
                        break;
                    }
                default:
                    {
                        var fileType = Path.GetExtension(filename);
                        if (string.IsNullOrEmpty(fileType))
                            throw new ArgumentException($"{filename} must have a valid extension.");

                        fileType = fileType[1..];
                        if (fileType.Equals("pdf", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var pdf = new MyFileConfigurationPdf();
                            configuration.CopyTo(pdf);
                            return await stream.UploadAsync(filename, pdf);
                        }

                        if (fileType.Equals("jpg", StringComparison.InvariantCultureIgnoreCase) ||
                            fileType.Equals("gif", StringComparison.InvariantCultureIgnoreCase) ||
                            fileType.Equals("png", StringComparison.InvariantCultureIgnoreCase) ||
                            fileType.Equals("bmp", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var image = new MyFileConfigurationImage();
                            configuration.CopyTo(image);
                            return await stream.UploadAsync(filename, image);
                        }

                        file = await stream.SaveAsync(filename, configuration).ConfigureAwait(false);
                        break;
                    }
            }

            if (file == null)
                return null;

            file.FilePath = file.FilePath.Replace(baseUrl.Replace("\\", "/"), null);
            file.FilePath = file.FilePath.StartsWith("/") ? file.FilePath : $"/{file.FilePath}";

            if (string.IsNullOrEmpty(file.ThumbnailPath))
                return file;

            file.ThumbnailPath = file.ThumbnailPath.Replace(baseUrl.Replace("\\", "/"), null);
            file.ThumbnailPath = file.ThumbnailPath.StartsWith("/") ? file.ThumbnailPath : $"/{file.ThumbnailPath}";

            return file;
        }

        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <typeparam name="TConfiguration">An generic type <see cref="MyFileConfiguration"/>.</typeparam>
        /// <param name="file">An <see cref="IFormFile"/> interface that representing input file stream to save.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        public static Task<IMyFile> UploadAsync<TConfiguration>(this IFormFile file)
            where TConfiguration : MyFileConfiguration => file.UploadAsync(Activator.CreateInstance<TConfiguration>());

        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <typeparam name="TConfiguration">An generic type <see cref="MyFileConfiguration"/>.</typeparam>
        /// <param name="stream">An <see cref="Stream"/> object that representing input file stream to save.</param>
        /// <param name="filename">An  <see cref="string"/> value that representing file's name.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        public static Task<IMyFile> UploadAsync<TConfiguration>(this Stream stream, string filename)
            where TConfiguration : MyFileConfiguration => stream.UploadAsync(filename, Activator.CreateInstance<TConfiguration>());

        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <param name="file">An <see cref="IFormFile"/> interface that representing input file stream to save.</param>
        /// <param name="config">A <see cref="Action{TResult}"/> instance that representing configurations to save.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        public static Task<IMyFile> UploadAsync(this IFormFile file, Action<MyFileConfiguration> config) =>
            file.UploadAsync<MyFileConfiguration>(config);

        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <param name="stream">An <see cref="Stream"/> object that representing input file stream to save.</param>
        /// <param name="filename">An  <see cref="string"/> value that representing file's name.</param>
        /// <param name="config">A <see cref="Action{TResult}"/> instance that representing configurations to save.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        public static Task<IMyFile> UploadAsync(this Stream stream, string filename, Action<MyFileConfiguration> config) =>
            stream.UploadAsync<MyFileConfiguration>(filename, config);

        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <typeparam name="TConfiguration">An generic type <see cref="MyFileConfiguration"/>.</typeparam>
        /// <param name="files">An <see cref="IFormFileCollection"/> interface that representing input file streams to save.</param>
        /// <param name="config">A <see cref="Action{TResult}"/> instance that representing configurations to save.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A <see cref="Task{TResult}"/> object thar representing asynchronous operation.</returns>
        public static Task<List<IMyFile>> UploadAsync<TConfiguration>(this IFormFileCollection files,
            Action<TConfiguration> config) where TConfiguration : MyFileConfiguration =>
            files.ToList().UploadAsync(config);

        /// <summary>
        /// Saves and uploads given file into the host.
        /// </summary>
        /// <typeparam name="TConfiguration">An generic type <see cref="MyFileConfiguration"/>.</typeparam>
        /// <param name="files">An collection of <see cref="IFormFile"/> that representing input file streams to save.</param>
        /// <param name="config">A <see cref="Action{TResult}"/> instance that representing configurations to save.</param>
        /// <exception cref="NullReferenceException"></exception>
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