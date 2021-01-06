namespace R8.FileHandlers
{
    /// <summary>
    /// An <see cref="IMyFileConfigurationPdfBase"/> interface.
    /// </summary>
    public interface IMyFileConfigurationPdfBase : IMyFileRuntime
    {
        /// <summary>
        /// Gets or sets An <see cref="string"/> path for <c>gsdll64.dll</c> fill path.
        /// </summary>
        public string GhostScriptDllPath { get; set; }

        /// <summary>
        /// Gets or sets An <see cref="int"/> value that representing Pdf thumbnail resolution.
        /// </summary>
        public int? ResolutionDpi { get; set; }

        /// <summary>
        /// Gets or sets An <see cref="int"/> value that representing Preview image quality for <c>JpegEncoder</c>.
        /// </summary>
        public int? ImageQuality { get; set; }
    }
}