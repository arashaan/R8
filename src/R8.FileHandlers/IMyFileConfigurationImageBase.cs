using SixLabors.ImageSharp.Formats;

namespace R8.FileHandlers
{
    /// <summary>
    /// An <see cref="IMyFileConfigurationImageBase"/> interface.
    /// </summary>
    public interface IMyFileConfigurationImageBase : IMyFileRuntime
    {
        /// <summary>
        /// Gets or sets An <see cref="IImageEncoder"/> interface that representing Encoder for saving images. If left null, will be set to <c>JpegEncoder</c>
        /// </summary>
        IImageEncoder ImageEncoder { get; set; }

        /// <summary>
        /// Gets or sets and <see cref="int"/> that representing image max size. Leave it <c>null</c>, if no need to resize image.
        /// </summary>
        int? ResizeToSize { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="string"/> value that representing a watermark image url.
        /// </summary>
        string? WatermarkPath { get; set; }
    }
}