using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;

namespace R8.FileHandlers
{
    public static class Tools
    {
        ///// <summary>
        ///// Represents an instance of <see cref="Image"/> with stuck watermark on it
        ///// </summary>
        ///// <param name="source">Source <see cref="Image"/></param>
        ///// <param name="watermark">Watermark <see cref="Image"/></param>
        ///// <returns>Returns an Image with source and watermark on it</returns>
        ///// <exception cref="ArgumentNullException"></exception>
        ///// <exception cref="ObjectDisposedException"></exception>
        //public static Image DrawWatermark(this Image source, Image watermark)
        //{
        //    if (source == null)
        //        throw new ArgumentNullException(nameof(source));
        //    if (watermark == null)
        //        throw new ArgumentNullException(nameof(watermark));

        //    using var outputImage = new Image<Rgba32>(source.Width, source.Height);
        //    outputImage.Mutate(o => o
        //        .DrawImage(source, 1f)
        //        .DrawImage(watermark, 1f)
        //    );
        //    return outputImage;
        //}

        /// <summary>
        /// Resizes <see cref="Image"/> based on specific width and height.
        /// </summary>
        /// <param name="image">Watermark <see cref="Stream"/> as <see cref="Image"/></param>
        /// <param name="width">Source Image's width</param>
        /// <param name="height">Source Image's height</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void ResizeScaleByDimensions(this Image image, int width, int height)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            image.Mutate(o => o.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Pad,
                Size = new Size(width, height),
                Sampler = new CubicResampler(3f, 1f, 2f),
                Position = AnchorPositionMode.Center,
                CenterCoordinates = new PointF(width / 2, height / 2)
            }));
        }

        ///// <summary>
        ///// Represents comparison between two <see cref="Stream"/>
        ///// </summary>
        ///// <returns>An <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        ///// <exception cref="ArgumentNullException"></exception>
        ///// <exception cref="ObjectDisposedException"></exception>
        //public static async Task<bool> CompareAsync(this Stream stream1, Stream stream2)
        //{
        //    if (stream1 == null)
        //        throw new ArgumentNullException(nameof(stream1));
        //    if (stream2 == null)
        //        throw new ArgumentNullException(nameof(stream2));

        //    var comparer = new StreamCompare();
        //    var areEqual = await comparer
        //        .AreEqualAsync(stream1, stream2)
        //        .ConfigureAwait(false);

        //    return areEqual;
        //}
        /// <summary>
        /// Represents a instance of <see cref="Stream"/> contains PDF Preview
        /// </summary>
        /// <param name="stream">An <see cref="Stream"/> contains PDF data</param>
        /// <param name="ghostScriptPath">An <see cref="string"/> path for <c>gsdll64.dll</c> fill path</param>
        /// <param name="outputStreamHandler">An <see cref="Action{TResult}"/> to do whatever need to be done when still output stream scope is open.</param>
        /// <param name="jpgQuality">An <see cref="int"/> value that representing Preview image quality for <c>JpegEncoder</c>.</param>
        /// <param name="dpi">An <see cref="int"/> value that representing Pdf thumbnail resolution</param>
        /// <returns>An <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static async Task<Stream> PdfToImageAsync(this Stream stream, string ghostScriptPath, int jpgQuality = 80, int dpi = 300)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (ghostScriptPath == null)
                throw new ArgumentNullException(nameof(ghostScriptPath));

            var versionInfo = new GhostscriptVersionInfo(ghostScriptPath);
            if (stream.Length == 0)
                throw new ArgumentNullException(nameof(stream.Length));

            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);

            using var rasterizer = new GhostscriptRasterizer();
            rasterizer.Open(stream, versionInfo, true);

            await using var inputStream = new MemoryStream();
            using var inputImage = rasterizer.GetPage(dpi, dpi, 1);
            inputImage.Save(inputStream, ImageFormat.Jpeg);
            inputImage.Dispose();
            rasterizer.Dispose();

            inputStream.Position = 0;
            inputStream.Seek(0, SeekOrigin.Begin);
            var outputStream = new MemoryStream();
            using var outputImage = await Image.LoadAsync(inputStream);
            await outputImage.SaveAsync(outputStream, new JpegEncoder { Quality = jpgQuality });

            outputStream.Position = 0;
            outputStream.Seek(0, SeekOrigin.Begin);

            outputImage.Dispose();
            await inputStream.DisposeAsync();

            return outputStream;
        }

        /// <summary>
        /// Represents a instance of <see cref="Stream"/> contains PDF Preview
        /// </summary>
        /// <param name="stream">An <see cref="Stream"/> contains PDF data</param>
        /// <param name="ghostScriptPath">An <see cref="string"/> path for <c>gsdll64.dll</c> fill path</param>
        /// <param name="outputStreamHandler">An <see cref="Action{TResult}"/> to do whatever need to be done when still output stream scope is open.</param>
        /// <param name="jpgQuality">An <see cref="int"/> value that representing Preview image quality for <c>JpegEncoder</c>.</param>
        /// <param name="dpi">An <see cref="int"/> value that representing Pdf thumbnail resolution</param>
        /// <returns>An <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static async Task PdfToImageAsync(this Stream stream, string ghostScriptPath, Action<Stream> outputStreamHandler, int jpgQuality = 80, int dpi = 300)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (ghostScriptPath == null)
                throw new ArgumentNullException(nameof(ghostScriptPath));

            var versionInfo = new GhostscriptVersionInfo(ghostScriptPath);
            if (stream.Length == 0)
                throw new ArgumentNullException(nameof(stream.Length));

            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);

            await using var inputStream = new MemoryStream();
            {
                using (var rasterizer = new GhostscriptRasterizer())
                {
                    rasterizer.Open(stream, versionInfo, true);
                    using var inputImage = rasterizer.GetPage(dpi, dpi, 1);
                    {
                        inputImage.Save(inputStream, ImageFormat.Jpeg);
                        inputImage.Dispose();
                    }
                }

                inputStream.Position = 0;
                inputStream.Seek(0, SeekOrigin.Begin);
                using var outputImage = await Image.LoadAsync(inputStream).ConfigureAwait(false);
                {
                    await inputStream.DisposeAsync().ConfigureAwait(false);
                    await using var outputStream = new MemoryStream();
                    {
                        await outputImage.SaveAsync(outputStream, new JpegEncoder { Quality = jpgQuality }).ConfigureAwait(false);

                        outputStream.Position = 0;
                        outputStream.Seek(0, SeekOrigin.Begin);

                        outputStreamHandler?.Invoke(outputStream);

                        await outputStream.FlushAsync().ConfigureAwait(false);
                        await outputStream.DisposeAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        /// <summary>
        /// Represents a instance of <see cref="Stream"/> contains PDF Preview
        /// </summary>
        /// <param name="stream">An <see cref="Stream"/> contains PDF data</param>
        /// <param name="ghostScriptPath">An <see cref="string"/> path for <c>gsdll64.dll</c> fill path</param>
        /// <param name="outputStreamHandler">An <see cref="Action{TResult}"/> to do whatever need to be done when still output stream scope is open.</param>
        /// <param name="jpgQuality">An <see cref="int"/> value that representing Preview image quality for <c>JpegEncoder</c>.</param>
        /// <param name="dpi">An <see cref="int"/> value that representing Pdf thumbnail resolution</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void PdfToImage(this Stream stream, string ghostScriptPath, Action<Stream> outputStreamHandler, int jpgQuality = 80, int dpi = 300) =>
            stream.PdfToImageAsync(ghostScriptPath, outputStreamHandler, jpgQuality, dpi).GetAwaiter().GetResult();

        /// <summary>
        /// Resizes width and height of An <see cref="Image"/> instance
        /// </summary>
        /// <param name="image">An <see cref="Image"/> instance contains Image</param>
        /// <param name="maxImageSize">An <see cref="float"/> value contains max width and height value</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void Resize(this Image image, float maxImageSize = 1500)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            if (image.Height <= 0 || image.Width <= 0)
                return;

            var sourceWidth = image.Width;
            var sourceHeight = image.Height;

            var widthRatio = maxImageSize / sourceWidth;
            var heightRatio = maxImageSize / sourceHeight;
            var ratio = heightRatio < widthRatio ? heightRatio : widthRatio;

            var destWidth = (int)(sourceWidth * ratio);
            var destHeight = (int)(sourceHeight * ratio);

            image.Mutate(processor => processor.Resize(destWidth, destHeight));
        }

        /// <summary>
        /// Loads watermark by file path into instance of <see cref="Image"/>.
        /// </summary>
        /// <param name="url">Watermark file url</param>
        /// <param name="sourceImageWidth">Source Image's width</param>
        /// <param name="sourceImageHeight">Source Image's height</param>
        /// <param name="cancellationToken"></param>
        /// <returns>An <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UnknownImageFormatException"></exception>
        /// <exception cref="InvalidImageContentException"></exception>
        public static async Task<Image> LoadWatermarkAsync(string url, int sourceImageWidth, int sourceImageHeight, CancellationToken cancellationToken = default)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            using var watermark = await Image.LoadAsync(url, cancellationToken).ConfigureAwait(false);
            watermark.ResizeScaleByDimensions(sourceImageWidth, sourceImageHeight);
            return watermark;
        }

        /// <summary>
        /// Loads watermark by file path into instance of <see cref="Image"/>
        /// </summary>
        /// <param name="url">Watermark file url</param>
        /// <param name="sourceImageWidth">Source Image's width</param>
        /// <param name="sourceImageHeight">Source Image's height</param>
        /// <param name="imageDecoder"><see cref="IImageDecoder"/> to be used for processing</param>
        /// <returns>An <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UnknownImageFormatException"></exception>
        /// <exception cref="InvalidImageContentException"></exception>
        public static async Task<Image> LoadWatermarkAsync(string url, int sourceImageWidth, int sourceImageHeight, IImageDecoder imageDecoder)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            using var watermark = await Image.LoadAsync(url, imageDecoder).ConfigureAwait(false);
            watermark.ResizeScaleByDimensions(sourceImageWidth, sourceImageHeight);
            return watermark;
        }
    }
}