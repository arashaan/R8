using System;
using System.IO;
using System.Threading.Tasks;

using R8.Lib;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace R8.FileHandlers
{
    public static class FileHandlers
    {
        /// <summary>
        /// Represents a <see cref="IMyFile"/> instance that contains saved file absolute path
        /// </summary>
        /// <param name="stream">An <see cref="Stream"/> that representing stream data to save</param>
        /// <param name="filename">An <see cref="string"/> value that representing file's full path</param>
        /// <param name="config">An <see cref="Action"/> to set configurations inside <see cref="MyFileConfiguration"/> instance</param>
        /// <returns>Returns an <see cref="IMyFile"/> instance that representing saved file path</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IMyFile? Save(this Stream stream, string filename, Action<MyFileConfiguration> config) =>
            stream.SaveAsync(filename, config).GetAwaiter().GetResult();

        /// <summary>
        /// Represents a <see cref="IMyFile"/> instance that contains saved file absolute path
        /// </summary>
        /// <param name="stream">An <see cref="Stream"/> that representing stream data to save</param>
        /// <param name="filename">An <see cref="string"/> value that representing file's full path</param>
        /// <param name="config">An <see cref="Action"/> to set configurations inside <see cref="MyFileConfiguration"/> instance</param>
        /// <returns>Returns an <see cref="IMyFile"/> instance that representing saved file path</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IMyFile? Save<TConfiguration>(this Stream stream, string filename, Action<TConfiguration> config) where TConfiguration : MyFileConfiguration =>
            stream.SaveAsync(filename, config).GetAwaiter().GetResult();

        /// <summary>
        /// Represents a <see cref="IMyFile"/> instance that contains saved file absolute path
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <param name="stream">An <see cref="Stream"/> that representing stream data to save</param>
        /// <param name="filename">An <see cref="string"/> value that representing file's full path</param>
        /// <param name="config">An <see cref="Action"/> to set configurations inside <see cref="MyFileConfiguration"/> instance</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="UnknownImageFormatException"></exception>
        /// <exception cref="InvalidImageContentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static Task<IMyFile?> SaveAsync<TConfiguration>(this Stream stream, string filename, Action<TConfiguration> config) where TConfiguration : MyFileConfiguration
        {
            var thisConfig = Activator.CreateInstance<TConfiguration>();
            config?.Invoke(thisConfig);
            return stream.SaveAsync(filename, thisConfig);
        }

        /// <summary>
        /// Represents a <see cref="IMyFile"/> instance that contains saved file absolute path
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <param name="stream">An <see cref="Stream"/> that representing stream data to save</param>
        /// <param name="filename">An <see cref="string"/> value that representing file's full path</param>
        /// <param name="config">A generic type of <see cref="MyFileConfiguration"/> to set configurations.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="UnknownImageFormatException"></exception>
        /// <exception cref="InvalidImageContentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static async Task<IMyFile?> SaveAsync<TConfiguration>(this Stream stream, string filename, TConfiguration config) where TConfiguration : MyFileConfiguration
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));

            if (stream.Length == 0)
                return null;

            filename = filename.Replace("\\", "/");
            return config switch
            {
                MyFileConfigurationImage imageConfiguration => await stream.SaveImageAsync(filename, imageConfiguration)
                    .ConfigureAwait(false),
                MyFileConfigurationPdf pdfConfiguration => await stream.SavePdfAsync(filename, pdfConfiguration)
                    .ConfigureAwait(false),
                _ => await stream.SaveAsync(filename, (MyFileConfiguration)config).ConfigureAwait(false)
            };
        }

        /// <summary>
        /// Represents a <see cref="IMyFile"/> instance that contains saved pdf absolute path
        /// </summary>
        /// <param name="stream">An <see cref="Stream"/> instance that representing pdf stream</param>
        /// <param name="filename">An <see cref="string"/> value that representing filename</param>
        /// <param name="config">An <see cref="Action"/> to set configurations inside <see cref="MyFileConfiguration"/> instance</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <returns>Returns an <see cref="IMyFile"/> instance that representing saved file path</returns>
        public static IMyFile SavePdf(this Stream stream, string filename,
            MyFileConfigurationPdf config) => stream.SavePdfAsync(filename, config).GetAwaiter().GetResult();

        internal const int DefaultPdfImageQuality = 80;
        internal const int DefaultPdfImageResolution = 300;

        /// <summary>
        /// Represents a <see cref="IMyFile"/> instance that contains saved pdf absolute path
        /// </summary>
        /// <param name="stream">An <see cref="Stream"/> instance that representing pdf stream</param>
        /// <param name="filename">An <see cref="string"/> value that representing filename</param>
        /// <param name="config">An <see cref="Action"/> to set configurations inside <see cref="MyFileConfiguration"/> instance</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static async Task<IMyFile> SavePdfAsync(this Stream stream, string filename,
            MyFileConfigurationPdf config)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));

            var fileExtension = Path.GetExtension(filename);
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException(null, nameof(filename));

            if (!fileExtension[1..].Equals("pdf", StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException(null, nameof(filename));

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrEmpty(config.GhostScriptDllPath))
                throw new NullReferenceException(
                    $"{nameof(config.GhostScriptDllPath)} expected to been filled to use PDF features");

            filename = filename.Replace("\\", "/");

            var isPdf = await stream.IsPdfAsync().ConfigureAwait(false);
            if (!isPdf)
                throw new NullReferenceException($"{nameof(filename)} should be a valid pdf file");

            await using var outputPdf = new MemoryStream();
            await using var outputThumbnail = new MemoryStream();
            var tempThumbnailStream = await stream.PdfToImageAsync(config.GhostScriptDllPath,
                config.ImageQuality ?? DefaultPdfImageQuality, config.ResolutionDpi ?? DefaultPdfImageResolution).ConfigureAwait(false);

            await tempThumbnailStream.CopyToAsync(outputThumbnail).ConfigureAwait(false);

            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(outputPdf).ConfigureAwait(false);
            await tempThumbnailStream.DisposeAsync().ConfigureAwait(false);
            await stream.DisposeAsync().ConfigureAwait(false);

            outputPdf.Position = 0;
            outputPdf.Seek(0, SeekOrigin.Begin);
            outputThumbnail.Position = 0;
            outputThumbnail.Seek(0, SeekOrigin.Begin);

            var imageConfig = new MyFileConfigurationImage();
            ((MyFileConfiguration)config).CopyTo(imageConfig);
            imageConfig.OverwriteExistingFile = true;
            imageConfig.SaveAsRealName = true;

            var pdfFilePath = config.GetFilePath(filename, "pdf");
            var imageFilePath =
                imageConfig.GetFilePath($"{Path.GetFileNameWithoutExtension(pdfFilePath)}_thumbnail", null);
            var thumbnail = await outputThumbnail
                .SaveImageAsync(imageFilePath, imageConfig)
                .ConfigureAwait(false);
            if (thumbnail == null)
                return null;

            var pdf = await outputPdf
                .SaveFileAsync(pdfFilePath, config.OverwriteExistingFile ?? false, config.TestDevelopment)
                .ConfigureAwait(false);
            if (pdf == null)
            {
                File.Delete(thumbnail.FilePath);
                return null;
            }

            await outputPdf.DisposeAsync();
            await outputThumbnail.DisposeAsync();

            return new MyFile
            {
                FilePath = pdf.FilePath,
                ThumbnailPath = thumbnail.FilePath,
                FileSize = new FileInfo(pdf.FilePath).Length
            };
        }

        /// <summary>
        /// Represents a <see cref="IMyFile"/> instance that contains saved file absolute path
        /// </summary>
        /// <param name="stream">An <see cref="Stream"/> that representing stream data to save</param>
        /// <param name="filename">An <see cref="string"/> value that representing file's full path</param>
        /// <param name="config">A generic type of <see cref="MyFileConfiguration"/> to set configurations.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<IMyFile?> SaveAsync(this Stream stream, string filename, MyFileConfiguration config)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));

            if (stream.Length == 0)
                return null;

            filename = filename.Replace("\\", "/");
            var fileExtension = Path.GetExtension(filename);
            if (string.IsNullOrEmpty(fileExtension))
                throw new NullReferenceException($"{nameof(filename)} should have an extension for filename");

            filename = config.GetFilePath(filename, fileExtension[1..]);
            var isValid = fileExtension[1..].ToLowerInvariant() switch
            {
                "zip" => stream.IsArchive(false),
                "gzip" => stream.IsArchive(false),
                "bzip2" => stream.IsArchive(false),
                "tar" => stream.IsArchive(false),
                "svg" => stream.IsSvg(),
                _ => true
            };

            if (!isValid)
                return null;

            var output = await stream
                .SaveFileAsync(filename, config.OverwriteExistingFile ?? false)
                .ConfigureAwait(false);
            return output;
        }

        /// <summary>
        /// Saves an <see cref="Image"/> instance into <see cref="Stream"/>
        /// </summary>
        /// <param name="image">An <see cref="Image"/> instance that representing image stream</param>
        /// <param name="name">An <see cref="string"/> value that representing file's full path</param>
        /// <param name="config">An <see cref="Action"/> to set configurations inside <see cref="MyFileConfiguration"/> instance</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="UnknownImageFormatException"></exception>
        /// <exception cref="InvalidImageContentException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static async Task<IMyFile?> SaveImageAsync(this Image image, string name, MyFileConfigurationImage config)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (config.ResizeToSize != null)
                image.Resize(config.ResizeToSize.Value);

            var encoder = config.ImageEncoder ??= new JpegEncoder();
            await using var outputStream = new MemoryStream();
            {
                if (!string.IsNullOrEmpty(config.WatermarkPath))
                {
                    using var watermark = await Image.LoadAsync(config.WatermarkPath).ConfigureAwait(false);
                    {
                        watermark.ResizeScaleByDimensions(image.Width, image.Height);
                        using var watermarkedImage = new Image<Rgba32>(image.Width, image.Height);
                        {
                            watermarkedImage.Mutate(o => o
                                .DrawImage(image, 1f)
                                .DrawImage(watermark, 1f)
                            );
                            switch (encoder)
                            {
                                case JpegEncoder jpeg:
                                    await watermarkedImage.SaveAsJpegAsync(outputStream, jpeg).ConfigureAwait(false);
                                    break;

                                case PngEncoder png:
                                    await watermarkedImage.SaveAsPngAsync(outputStream, png).ConfigureAwait(false);
                                    break;

                                case BmpEncoder bmp:
                                    await watermarkedImage.SaveAsBmpAsync(outputStream, bmp).ConfigureAwait(false);
                                    break;

                                case GifEncoder gif:
                                    await watermarkedImage.SaveAsGifAsync(outputStream, gif).ConfigureAwait(false);
                                    break;
                            }
                        }

                        watermarkedImage.Dispose();
                    }
                    watermark.Dispose();
                }
                else
                {
                    await image.SaveAsync(outputStream, encoder).ConfigureAwait(false);
                }

                image.Dispose();
                try
                {
                    name = Path.GetFileNameWithoutExtension(name);
                }
                catch
                {
                    // ignored
                }

                var ext = encoder.GetImageFormat().GetImageExtension();
                var finalFileName = config.GetFilePath($"{name}.{ext}", ext);
                var outputImage = await outputStream.SaveFileAsync(finalFileName, config.OverwriteExistingFile ?? false, config.TestDevelopment).ConfigureAwait(false);
                await outputStream.DisposeAsync().ConfigureAwait(false);

                if (outputImage != null && string.IsNullOrEmpty(outputImage.FilePath))
                    return null;

                return outputImage;
            }
        }

        /// <summary>
        /// Saves an <see cref="Image"/> instance into <see cref="Stream"/>
        /// </summary>
        /// <param name="image">An <see cref="Image"/> instance that representing image stream</param>
        /// <param name="name">An <see cref="string"/> value that representing file's full path</param>
        /// <param name="config">An <see cref="Action"/> to set configurations inside <see cref="MyFileConfiguration"/> instance</param>
        /// <returns>Returns an <see cref="IMyFile"/> instance that representing saved file path</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="UnknownImageFormatException"></exception>
        /// <exception cref="InvalidImageContentException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static IMyFile? SaveImage(this Image image, string name, MyFileConfigurationImage config) =>
            image.SaveImageAsync(name, config).GetAwaiter().GetResult();

        /// <summary>
        /// Saves an <see cref="Stream"/> into an <see cref="IMyFile"/> instance.
        /// </summary>
        /// <param name="stream">An <see cref="Stream"/> instance that representing file's data.</param>
        /// <param name="name">An <see cref="string"/> value that representing a path to save file in.</param>
        /// <param name="overwriteFile">An <see cref="bool"/> value that representing if overwriteFile on existing file or not</param>
        /// <returns>An <see cref="IMyFile"/> instance that representing saved file location</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static IMyFile? SaveFile(this Stream stream, string name, bool overwriteFile) =>
            stream.SaveFileAsync(name, overwriteFile).GetAwaiter().GetResult();

        /// <summary>
        /// Saves an <see cref="Stream"/> into an <see cref="IMyFile"/> instance.
        /// </summary>
        /// <param name="stream">An <see cref="Stream"/> instance that representing file's data.</param>
        /// <param name="filename">An <see cref="string"/> value that representing a path to save file in.</param>
        /// <param name="overwriteFile">An <see cref="bool"/> value that representing if overwriteFile on existing file or not</param>
        /// <param name="test"></param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        private static async Task<IMyFile?> SaveFileAsync(this Stream stream, string filename, bool overwriteFile, bool test)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));

            if (stream.Length == 0)
                return null;

            if (!stream.CanRead || !stream.CanSeek)
                return null;

            stream.Seek(0, SeekOrigin.Begin);
            stream.Position = 0;

            if (overwriteFile)
            {
                if (test) 
                    return new MyFile(filename);

                await using var fileStream = new FileStream(filename, FileMode.OpenOrCreate);
                {
                    await stream.CopyToAsync(fileStream).ConfigureAwait(false);
                    await fileStream.DisposeAsync().ConfigureAwait(false);
                }
            }
            else
            {
                var checkForDuplication = File.Exists(filename);
                if (checkForDuplication)
                    filename = Extensions.GetUniqueFileName(filename);

                if (test) 
                    return new MyFile(filename);

                await using var fileStream = new FileStream(filename, FileMode.Create);
                {
                    await stream.CopyToAsync(fileStream).ConfigureAwait(false);
                    await fileStream.DisposeAsync().ConfigureAwait(false);
                }
            }

            return new MyFile(filename);
        }

        /// <summary>
        /// Saves an <see cref="Stream"/> into an <see cref="IMyFile"/> instance.
        /// </summary>
        /// <param name="stream">An <see cref="Stream"/> instance that representing file's data.</param>
        /// <param name="filename">An <see cref="string"/> value that representing a path to save file in.</param>
        /// <param name="overwriteFile">An <see cref="bool"/> value that representing if overwriteFile on existing file or not</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static Task<IMyFile?> SaveFileAsync(this Stream stream, string filename, bool overwriteFile) =>
            stream.SaveFileAsync(filename, overwriteFile, false);

        /// <summary>
        /// Represents a <see cref="IMyFile"/> instance that contains saved file absolute path
        /// </summary>
        /// <param name="stream">An <see cref="Stream"/> that representing stream data to save</param>
        /// <param name="name">An <see cref="string"/> value that representing file's full path</param>
        /// <param name="config">An <see cref="Action"/> to set configurations inside <see cref="MyFileConfiguration"/> instance</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="UnknownImageFormatException"></exception>
        /// <exception cref="InvalidImageContentException"></exception>
        public static async Task<IMyFile?> SaveImageAsync(this Stream stream, string name, MyFileConfigurationImage config)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);

            using var image = await Image.LoadAsync(stream).ConfigureAwait(false);
            return await image.SaveImageAsync(name, config).ConfigureAwait(false);
        }

        /// <summary>
        /// Represents a <see cref="IMyFile"/> instance that contains saved file absolute path
        /// </summary>
        /// <param name="stream">An <see cref="Stream"/> that representing stream data to save</param>
        /// <param name="name">An <see cref="string"/> value that representing file's full path</param>
        /// <param name="config">An <see cref="Action"/> to set configurations inside <see cref="MyFileConfiguration"/> instance</param>
        /// <returns>Returns an <see cref="IMyFile"/> instance that representing saved file path</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="UnknownImageFormatException"></exception>
        /// <exception cref="InvalidImageContentException"></exception>
        public static IMyFile? SaveImage(this Stream stream, string name, MyFileConfigurationImage config) =>
            stream.SaveImageAsync(name, config).GetAwaiter().GetResult();
    }
}