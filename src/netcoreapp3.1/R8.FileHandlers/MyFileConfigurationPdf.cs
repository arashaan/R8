namespace R8.FileHandlers
{
    /// <summary>
    /// Initializes an instance of <see cref="MyFileConfigurationPdf"/>
    /// </summary>
    public class MyFileConfigurationPdf : MyFileConfiguration, IMyFileConfigurationPdfBase
    {
        /// <summary>
        /// Initializes an instance of <see cref="MyFileConfigurationPdf"/>
        /// </summary>
        public MyFileConfigurationPdf()
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="MyFileConfigurationPdf"/>
        /// </summary>
        /// <param name="ghostScriptDllPath">An <see cref="string"/> path for <c>gsdll64.dll</c> fill path.</param>
        public MyFileConfigurationPdf(string ghostScriptDllPath)
        {
            GhostScriptDllPath = ghostScriptDllPath;
        }

        /// <summary>
        /// Initializes an instance of <see cref="MyFileConfigurationPdf"/>
        /// </summary>
        /// <param name="ghostScriptDllPath">An <see cref="string"/> path for <c>gsdll64.dll</c> fill path.</param>
        /// <param name="resolutionDpi">An <see cref="int"/> value that representing Pdf thumbnail resolution.</param>
        public MyFileConfigurationPdf(string ghostScriptDllPath, int resolutionDpi)
        {
            GhostScriptDllPath = ghostScriptDllPath;
            ResolutionDpi = resolutionDpi;
        }

        /// <summary>
        /// Initializes an instance of <see cref="MyFileConfigurationPdf"/>
        /// </summary>
        /// <param name="ghostScriptDllPath">An <see cref="string"/> path for <c>gsdll64.dll</c> fill path.</param>
        /// <param name="resolutionDpi">An <see cref="int"/> value that representing Pdf thumbnail resolution.</param>
        /// <param name="imageQuality">An <see cref="int"/> value that representing Preview image quality for <c>JpegEncoder</c>.</param>
        public MyFileConfigurationPdf(string ghostScriptDllPath, int resolutionDpi, int imageQuality)
        {
            GhostScriptDllPath = ghostScriptDllPath;
            ResolutionDpi = resolutionDpi;
            ImageQuality = imageQuality;
        }

        /// <summary>
        /// Gets or sets An <see cref="string"/> path for <c>gsdll64.dll</c> fill path.
        /// </summary>
        public string GhostScriptDllPath { get; set; }

        /// <summary>
        /// Gets or sets An <see cref="int"/> value that representing Pdf thumbnail resolution.
        /// </summary>
        /// <remarks>default: <c>300</c></remarks>
        public int? ResolutionDpi { get; set; } 

        /// <summary>
        /// Gets or sets An <see cref="int"/> value that representing Preview image quality for <c>JpegEncoder</c>.
        /// </summary>
        /// <remarks>default: <c>80</c></remarks>
        public int? ImageQuality { get; set; } 
    }
}