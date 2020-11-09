namespace R8.Lib.FileHandlers
{
    /// <summary>
    /// An interface to present saved file information.
    /// </summary>
    public interface IMyFile
    {
        /// <summary>
        /// Gets or sets file's absolute path
        /// </summary>
        string FilePath { get; set; }

        /// <summary>
        /// Gets or sets file' length
        /// </summary>
        long FileSize { get; set; }

        /// <summary>
        /// Gets or sets Thumbnail for image file's absolute path
        /// </summary>
        /// <remarks>Only for PDF preview, so far</remarks>
        string ThumbnailPath { get; set; }
    }
}