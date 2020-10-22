using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;

using NeoSmart.StreamCompare;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace R8.Lib.FileHandlers
{
    public static class FileHandlers
    {
        public const int MaxImageSize = 1500;

        public static bool Delete(string fileName, string rootPath,
            string uploadFolder = "uploads")
        {
            try
            {
                var inHostPath = Path.Combine(uploadFolder, fileName);
                var fullPath = Path.Combine(rootPath, inHostPath);
                if (!File.Exists(fullPath)) return false;
                File.Delete(fullPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> CompareAsync(this Stream source, Stream toBeCompared)
        {
            var comparer = new StreamCompare();
            var areEqual = await comparer.AreEqualAsync(source, toBeCompared).ConfigureAwait(false);
            return areEqual;
        }

        private static int FindAspectRatioCore(int width, int height)
        {
            if (width < height)
            {
                var temp = width;
                width = height;
                height = temp;
            }

            while (true)
            {
                var remainder = width % height;
                if (remainder == 0)
                    return height;
                width = height;
                height = remainder;
            }
        }

        public static AspectRatio FindAspectRatio(int width, int height)
        {
            var ar = FindAspectRatioCore(width, height);
            return new AspectRatio(width / ar, height / ar);
        }

        private static void SaveAsWatermarked(this Image sourceImage, string watermarkUrl, Action<Image> action)
        {
            using var watermark = LoadWatermark(watermarkUrl, sourceImage.Width, sourceImage.Height);
            using var outputImage = sourceImage.Watermarkize(watermark);
            action.Invoke(outputImage);

            watermark.Dispose();
            outputImage.Dispose();
        }

        private static async Task SaveAsWatermarkedAsync(this Image sourceImage, string watermarkUrl, Action<Image> action, CancellationToken cancellationToken = default)
        {
            using var watermark = await LoadWatermarkAsync(watermarkUrl, sourceImage.Width, sourceImage.Height, cancellationToken);
            using var outputImage = sourceImage.Watermarkize(watermark);
            action.Invoke(outputImage);

            watermark.Dispose();
            outputImage.Dispose();
        }

        public static async Task<Image> LoadWatermarkAsync(Stream stream, int sourceImageWidth, int sourceImageHeight)
        {
            using var watermark = await Image.LoadAsync(stream);
            watermark.ProcessWatermark(sourceImageWidth, sourceImageHeight);
            return watermark;
        }

        public static async Task<Image> LoadWatermarkAsync(Stream stream, int sourceImageWidth, int sourceImageHeight, IImageDecoder imageDecoder)
        {
            using var watermark = await Image.LoadAsync(stream, imageDecoder);
            watermark.ProcessWatermark(sourceImageWidth, sourceImageHeight);
            return watermark;
        }

        public static Image LoadWatermark(Stream stream, int sourceImageWidth, int sourceImageHeight)
        {
            using var watermark = Image.Load(stream);
            watermark.ProcessWatermark(sourceImageWidth, sourceImageHeight);
            return watermark;
        }

        public static Image LoadWatermark(Stream stream, int sourceImageWidth, int sourceImageHeight, IImageDecoder imageDecoder)
        {
            using var watermark = Image.Load(stream, imageDecoder);
            watermark.ProcessWatermark(sourceImageWidth, sourceImageHeight);
            return watermark;
        }

        private static void ProcessWatermark(this Image watermark, int sourceImageWidth, int sourceImageHeight)
        {
            watermark.Mutate(o => o.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Pad,
                Size = new Size(sourceImageWidth, sourceImageHeight),
                Sampler = new CubicResampler(3f, 1f, 2f),
                Position = AnchorPositionMode.Center,
                CenterCoordinates = new PointF(sourceImageWidth / 2, sourceImageHeight / 2)
            }));
        }

        public static Image LoadWatermark(string url, int sourceImageWidth, int sourceImageHeight)
        {
            using var watermark = Image.Load(url);
            watermark.ProcessWatermark(sourceImageWidth, sourceImageHeight);
            return watermark;
        }

        public static Image LoadWatermark(string url, int sourceImageWidth, int sourceImageHeight, IImageDecoder imageDecoder)
        {
            using var watermark = Image.Load(url, imageDecoder);
            watermark.ProcessWatermark(sourceImageWidth, sourceImageHeight);
            return watermark;
        }

        public static async Task<Image> LoadWatermarkAsync(string url, int sourceImageWidth, int sourceImageHeight, CancellationToken cancellationToken = default)
        {
            using var watermark = await Image.LoadAsync(url, cancellationToken).ConfigureAwait(false);
            watermark.ProcessWatermark(sourceImageWidth, sourceImageHeight);
            return watermark;
        }

        public static async Task<Image> LoadWatermarkAsync(string url, int sourceImageWidth, int sourceImageHeight, IImageDecoder imageDecoder)
        {
            using var watermark = await Image.LoadAsync(url, imageDecoder).ConfigureAwait(false);
            watermark.ProcessWatermark(sourceImageWidth, sourceImageHeight);
            return watermark;
        }

        public static Image Watermarkize(this Image source, Image watermark)
        {
            using var outputImage = new Image<Rgba32>(source.Width, source.Height);
            outputImage.Mutate(o => o
                .DrawImage(source, 1f)
                .DrawImage(watermark, 1f)
            );
            return outputImage;
        }

        private static void SaveImage(this Image img, Stream targetStream, IImageEncoder encoder)
        {
            switch (encoder)
            {
                case JpegEncoder jpeg:
                    img.SaveAsJpeg(targetStream, jpeg);
                    break;

                case PngEncoder png:
                    img.SaveAsPng(targetStream, png);
                    break;

                case BmpEncoder bmp:
                    img.SaveAsBmp(targetStream, bmp);
                    break;

                case GifEncoder gif:
                    img.SaveAsGif(targetStream, gif);
                    break;

                case TgaEncoder tga:
                    img.SaveAsTga(targetStream, tga);
                    break;

                case null:
                default:
                    throw new ArgumentOutOfRangeException($"Unable to recognize {nameof(encoder)} encoder of type {typeof(IImageEncoder)}");
            }
        }

        private static async Task SaveImageAsync(this Image img, Stream targetStream, IImageEncoder encoder, CancellationToken cancellationToken = default)
        {
            switch (encoder)
            {
                case JpegEncoder jpeg:
                    await img.SaveAsJpegAsync(targetStream, jpeg, cancellationToken: cancellationToken).ConfigureAwait(false);
                    break;

                case PngEncoder png:
                    await img.SaveAsPngAsync(targetStream, png, cancellationToken: cancellationToken).ConfigureAwait(false);
                    break;

                case BmpEncoder bmp:
                    await img.SaveAsBmpAsync(targetStream, bmp, cancellationToken: cancellationToken).ConfigureAwait(false);
                    break;

                case GifEncoder gif:
                    await img.SaveAsGifAsync(targetStream, gif, cancellationToken: cancellationToken).ConfigureAwait(false);
                    break;

                case TgaEncoder tga:
                    await img.SaveAsTgaAsync(targetStream, tga, cancellationToken: cancellationToken).ConfigureAwait(false);
                    break;

                case null:
                default:
                    throw new ArgumentOutOfRangeException($"Unable to recognize {nameof(encoder)} encoder of type {typeof(IImageEncoder)}");
            }
        }

        public static void SaveAsWatermarked(this Image sourceImage, Stream targetStream,
            string watermarkUrl, IImageEncoder encoder)
        {
            using (sourceImage)
            {
                sourceImage.SaveAsWatermarked(watermarkUrl, img =>
                {
                    img.SaveImage(targetStream, encoder);
                });
            }
        }

        public static async Task SaveAsWatermarkedAsync(this Image sourceImage, Stream targetStream,
           string watermarkUrl, IImageEncoder encoder)
        {
            using (sourceImage)
            {
                await sourceImage.SaveAsWatermarkedAsync(watermarkUrl, async img =>
                {
                    await img.SaveAsync(targetStream, encoder);
                });
            }
        }

        public static async Task<bool> IsImageAsync(this Stream stream)
        {
            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);
            if (stream.Length == 0) return false;

            bool isImage;
            try
            {
                using var img = await Image.LoadAsync(stream);
                isImage = img.Width > 0;
                img.Dispose();
            }
            catch
            {
                isImage = false;
            }

            return isImage;
        }

        public static bool IsImage(this Stream stream)
        {
            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);
            if (stream.Length == 0) return false;

            bool isImage;
            try
            {
                using var img = Image.Load(stream);
                isImage = img.Width > 0;
                img.Dispose();
            }
            catch
            {
                isImage = false;
            }

            return isImage;
        }

        public static async Task<MemoryStream> OpenFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            try
            {
                var ms = new MemoryStream();
                await using var fileStream = new FileStream(filePath, FileMode.Open);
                await fileStream.CopyToAsync(ms).ConfigureAwait(false);
                await fileStream.DisposeAsync().ConfigureAwait(false);
                await fileStream.FlushAsync().ConfigureAwait(false);

                return ms;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static MemoryStream OpenFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            try
            {
                var ms = new MemoryStream();
                using var fileStream = new FileStream(filePath, FileMode.Open);
                fileStream.CopyTo(ms);
                fileStream.Dispose();
                fileStream.Flush();

                return ms;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task<Stream> PdfToImageAsync(this Stream pdfStream, IImageEncoder encoder, string ghostScriptPath)
        {
            await using var inputStream = pdfStream.PdfToImageCore(encoder, ghostScriptPath);
            var outputStream = new MemoryStream();
            using var outputImage = await Image.LoadAsync(inputStream);
            await outputImage.SaveAsync(outputStream, encoder);
            await inputStream.DisposeAsync().ConfigureAwait(false);
            await inputStream.FlushAsync().ConfigureAwait(false);
            return outputStream;
        }

        private static Stream PdfToImageCore(this Stream pdfStream, IImageEncoder encoder, string ghostScriptPath)
        {
            if (pdfStream.Length == 0)
                return null;

            pdfStream.Position = 0;
            pdfStream.Seek(0, SeekOrigin.Begin);

            const int dpi = 300;

            var currentAssembly = Assembly.GetEntryAssembly();
            if (currentAssembly == null)
                return null;

            var appPath = Path.GetDirectoryName(currentAssembly.Location);
            if (string.IsNullOrEmpty(appPath))
                return null;

            // var ghostScriptPath = Path.Combine(appPath, "gsdll64.dll");
            var versionInfo = new GhostscriptVersionInfo(ghostScriptPath);

            using var inputStream = new MemoryStream();
            using (var rasterizer = new GhostscriptRasterizer())
            {
                rasterizer.Open(pdfStream, versionInfo, true);
                var inputImage = rasterizer.GetPage(dpi, dpi, 1);

                ImageFormat imageFormat;
                switch (encoder)
                {
                    case PngEncoder png:
                        imageFormat = ImageFormat.Png;
                        break;

                    case BmpEncoder bmp:
                        imageFormat = ImageFormat.Bmp;
                        break;

                    case GifEncoder gif:
                        imageFormat = ImageFormat.Gif;
                        break;

                    default:
                    case JpegEncoder jpg:
                        imageFormat = ImageFormat.Jpeg;
                        break;
                }
                inputImage.Save(inputStream, imageFormat);
            }

            inputStream.Position = 0;
            inputStream.Seek(0, SeekOrigin.Begin);

            return inputStream;
        }

        public static Stream PdfToImage(this Stream pdfStream, IImageEncoder encoder, string ghostScriptPath)
        {
            using var inputStream = pdfStream.PdfToImageCore(encoder, ghostScriptPath);
            var outputStream = new MemoryStream();
            using var outputImage = Image.Load(inputStream);
            outputImage.Save(outputStream, encoder);
            inputStream.Dispose();
            inputStream.Flush();
            return outputStream;
        }

        public static void ResizeImage(this Image image)
        {
            if (image == null)
                return;

            if (image.Height <= 0 || image.Width <= 0)
                return;

            var sourceWidth = image.Width;
            var sourceHeight = image.Height;

            var widthRatio = (float)MaxImageSize / sourceWidth;
            var heightRatio = (float)MaxImageSize / sourceHeight;
            var ratio = heightRatio < widthRatio ? heightRatio : widthRatio;

            var destWidth = (int)(sourceWidth * ratio);
            var destHeight = (int)(sourceHeight * ratio);

            image.Mutate(processor => processor.Resize(destWidth, destHeight));
        }

        public static void GetOrCreateDirectory(string serverRootPath, string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException(path);

            var dirArrays = path.Split("\\");
            for (var i = 0; i < dirArrays.Length; i++)
            {
                var inHostPath = string.Join("\\", dirArrays.Take(i + 1));
                var thisPath = Path.Combine(serverRootPath, inHostPath);
                var directory = new DirectoryInfo(thisPath);
                if (!directory.Exists)
                    directory.Create();
            }
        }

        public static async Task<MyThumbnailFile> SaveImageAsync(this Image image, string rootPath,
            string absolutePath, Action<MyFileConfiguration> config)
        {
            var thisConfig = new MyFileConfiguration();
            config?.Invoke(thisConfig);
            image.ResizeImage();

            var outputStream = new MemoryStream();
            if (thisConfig.WatermarkOnImages)
            {
                var watermarkUrl = rootPath + thisConfig.WatermarkPath;
                await image.SaveAsWatermarkedAsync(outputStream, watermarkUrl, thisConfig.ImageEncoder);
            }
            else
            {
                await image.SaveAsync(outputStream, thisConfig.ImageEncoder);
            }

            image.Dispose();

            var getExtensionIndex = absolutePath.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase);
            if (getExtensionIndex >= 0)
            {
                string extension;
                try
                {
                    extension = Path.GetExtension(absolutePath);
                }
                catch
                {
                    extension = null;
                }

                var strExt = absolutePath[new Range(getExtensionIndex, ^0)];
                if (!string.IsNullOrEmpty(extension))
                {
                    if (strExt.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
                        absolutePath = absolutePath.Substring(0, getExtensionIndex);
                    else
                        return null;
                }
                else
                {
                    var extArr = FileTypes.Image.GetFileExtensions().Select(x => x.ToLower());
                    if (extArr.Contains(strExt.ToLower()))
                        absolutePath = absolutePath.Substring(0, getExtensionIndex);
                    else
                        return null;
                }
            }

            var outputImage = await outputStream.SaveFileAsync(rootPath, $"{absolutePath}.jpg").ConfigureAwait(false);
            return new MyThumbnailFile(outputImage);
        }

        public static async Task<bool> IsPdfAsync(this Stream stream)
        {
            if (stream == null)
                return false;

            stream.Seek(0, SeekOrigin.Begin);
            stream.Position = 0;

            const string pdfString = "%PDF-";
            var pdfBytes = Encoding.ASCII.GetBytes(pdfString);
            var len = pdfBytes.Length;
            var buf = new byte[len];
            var remaining = len;
            var pos = 0;
            while (remaining > 0)
            {
                var amtRead = await stream.ReadAsync(buf, pos, remaining);
                if (amtRead == 0) return false;
                remaining -= amtRead;
                pos += amtRead;
            }

            return pdfBytes.SequenceEqual(buf);
        }

        public static bool IsPdf(this Stream stream)
        {
            if (stream == null)
                return false;

            stream.Seek(0, SeekOrigin.Begin);
            stream.Position = 0;

            const string pdfString = "%PDF-";
            var pdfBytes = Encoding.ASCII.GetBytes(pdfString);
            var len = pdfBytes.Length;
            var buf = new byte[len];
            var remaining = len;
            var pos = 0;
            while (remaining > 0)
            {
                var amtRead = stream.Read(buf, pos, remaining);
                if (amtRead == 0) return false;
                remaining -= amtRead;
                pos += amtRead;
            }

            return pdfBytes.SequenceEqual(buf);
        }

        public static MyFile SaveFile(this Stream stream, string path)
        {
            if (stream.Length == 0)
                return null;

            if (!stream.CanRead || !stream.CanSeek)
                return null;

            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                using var fileStream = new FileStream(path, FileMode.Create);
                stream.CopyTo(fileStream);
                fileStream.Flush();

                var result = new MyFile(path);
                return result;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<MyFile> SaveFileAsync(this Stream stream, string path)
        {
            if (stream.Length == 0)
                return null;

            if (!stream.CanRead || !stream.CanSeek)
                return null;

            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                await using var fileStream = new FileStream(path, FileMode.Create);
                await stream.CopyToAsync(fileStream).ConfigureAwait(false);
                await fileStream.FlushAsync().ConfigureAwait(false);

                var result = new MyFile(path);
                return result;
            }
            catch
            {
                return null;
            }
        }

        public static MyFile SaveFile(this Stream stream, string rootPath,
           string absolutePath)
        {
            if (stream.Length == 0)
                return null;

            if (!stream.CanRead || !stream.CanSeek)
                return null;

            try
            {
                var url = Path.Combine(rootPath, absolutePath);
                stream.Seek(0, SeekOrigin.Begin);
                using var fileStream = new FileStream(url, FileMode.Create);
                stream.CopyTo(fileStream);
                fileStream.Flush();

                var result = new MyFile(absolutePath);
                return result;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<MyFile> SaveFileAsync(this Stream stream, string rootPath,
            string absolutePath)
        {
            if (stream.Length == 0)
                return null;

            if (!stream.CanRead || !stream.CanSeek)
                return null;

            try
            {
                var url = Path.Combine(rootPath, absolutePath);
                stream.Seek(0, SeekOrigin.Begin);
                await using var fileStream = new FileStream(url, FileMode.Create);
                await stream.CopyToAsync(fileStream).ConfigureAwait(false);
                await fileStream.FlushAsync().ConfigureAwait(false);

                var result = new MyFile(absolutePath);
                return result;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<MyThumbnailFile> SaveImageAsync(this Stream stream, string rootPath,
            string absolutePath, Action<MyFileConfiguration> config)
        {
            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);

            using var image = await Image.LoadAsync(stream);

            var result = await image.SaveImageAsync(rootPath, absolutePath, config);
            return result;
        }

        public static FileTypes? GetFileType(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            var fields = typeof(FileTypes)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .ToList();
            if (fields.Count == 0)
                return null;

            FileTypes? type = null;
            foreach (var field in fields)
            {
                var displayAttribute = field.GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute == null)
                    continue;

                var allowedExtensions = displayAttribute.Name.Split("|");
                if (allowedExtensions.Length == 0)
                    continue;

                var fileExtension = Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(fileExtension))
                    continue;

                if (fileExtension.StartsWith("."))
                    fileExtension = fileExtension.Split(".")[1];

                var contain = allowedExtensions.Contains(fileExtension);
                if (!contain)
                    continue;

                type = (FileTypes)Enum.Parse(typeof(FileTypes), field.Name, true);
                break;
            }

            return type;
        }
    }
}