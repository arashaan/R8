using SixLabors.ImageSharp.Formats;

namespace R8.FileHandlers.AspNetCore
{
    public class FileHandlerImageRuntime : IMyFileConfigurationImageBase
    {
        /// <summary>
        /// Gets or sets An <see cref="IImageEncoder"/> interface that representing Encoder for saving images. If left null, will be set to <c>JpegEncoder</c>
        /// </summary>
        /// <remarks>default: <c>new JpegEncoder{ Quality = 80 }</c></remarks>
        public IImageEncoder? ImageEncoder { get; set; }

        /// <summary>
        /// Gets or sets and <see cref="int"/> that representing image max size. Leave it <c>null</c>, if no need to resize image.
        /// </summary>
        /// <remarks>default: <c>null</c></remarks>
        public int? ResizeToSize { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="string"/> value that representing a watermark image url.
        /// </summary>
        /// <remarks>default: <c>null</c></remarks>
        public string? WatermarkPath { get; set; }
    }
}