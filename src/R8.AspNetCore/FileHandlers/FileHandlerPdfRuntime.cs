using R8.FileHandlers;

namespace R8.AspNetCore.FileHandlers
{
    public class FileHandlerPdfRuntime : IMyFileConfigurationPdfBase
    {
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