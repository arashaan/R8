namespace R8.Lib.FileHandlers
{
    /// <summary>
    /// Initializes an instance of <see cref="MyFileConfigurationWatermark"/>
    /// </summary>
    public class MyFileConfigurationWatermark
    {
        /// <summary>
        /// Initializes an instance of <see cref="MyFileConfigurationWatermark"/>
        /// </summary>
        public MyFileConfigurationWatermark()
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="MyFileConfigurationWatermark"/>
        /// </summary>
        /// <param name="watermarkPath">An <see cref="string"/> value that representing watermark image absolute path</param>
        public MyFileConfigurationWatermark(string watermarkPath)
        {
            Path = watermarkPath;
        }

        /// <summary>
        /// Gets or sets watermark file path
        /// </summary>
        public string Path { get; set; }

        public override string ToString()
        {
            return Path;
        }
    }
}