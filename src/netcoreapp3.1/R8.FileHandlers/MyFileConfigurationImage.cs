using SixLabors.ImageSharp.Formats;

namespace R8.FileHandlers
{
    /// <summary>
    /// Initializes an instance of <see cref="MyFileConfigurationImage"/>
    /// </summary>
    public class MyFileConfigurationImage : MyFileConfiguration, IMyFileConfigurationImageBase
    {
        /// <summary>
        /// Gets or sets An <see cref="IImageEncoder"/> interface that representing Encoder for saving images. If left null, will be set to <c>JpegEncoder</c>
        /// </summary>
        /// <remarks>default: <c>new JpegEncoder{ Quality = 80 }</c></remarks>
        public IImageEncoder? ImageEncoder { get; set; }

        public int? ResizeToSize { get; set; }

        public string? WatermarkPath { get; set; }
    }
}