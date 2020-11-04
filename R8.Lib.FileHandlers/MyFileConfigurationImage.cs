using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace R8.Lib.FileHandlers
{
    /// <summary>
    /// Initializes an instance of <see cref="MyFileConfigurationImage"/>
    /// </summary>
    public class MyFileConfigurationImage : MyFileConfiguration
    {
        /// <summary>
        /// Gets or sets An <see cref="IImageEncoder"/> interface that representing Encoder for saving images. If left null, will be set to <c>JpegEncoder</c>
        /// </summary>
        /// <remarks>default: <c>new JpegEncoder{ Quality = 80 }</c></remarks>
        public IImageEncoder ImageEncoder { get; set; } = new JpegEncoder { Quality = 80 };

        /// <summary>
        /// Gets or sets and <see cref="int"/> that representing image max size. Leave it <c>null</c>, if no need to resize image.
        /// </summary>
        /// <remarks>default: <c>null</c></remarks>
        public int? ResizeToSize { get; set; } = null;

        /// <summary>
        /// Gets or sets an <see cref="MyFileConfigurationWatermark"/> instance that representing if pictures should have specific watermark.
        /// </summary>
        /// <remarks>default: <c>null</c></remarks>
        public MyFileConfigurationWatermark? Watermark { get; set; }
    }
}