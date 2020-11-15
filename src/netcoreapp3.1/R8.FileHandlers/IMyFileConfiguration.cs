using System;

namespace R8.FileHandlers
{
    /// <summary>
    /// An <see cref="IMyFileConfiguration"/> interface.
    /// </summary>
    public interface IMyFileConfiguration
    {
        /// <summary>
        /// Gets or sets an <see cref="bool"/> value that avoid to save file into disk. For testing.
        /// </summary>
        /// <remarks>This is an internal API, and may be removed in future versions.</remarks>
        bool TestDevelopment { get; set; }

        /// <summary>
        /// Returns final file path
        /// </summary>
        /// <param name="currentFileName">An <see cref="string"/> value that representing real file name</param>
        /// <param name="fileExtension"></param>
        /// <returns>An <see cref="string"/> value that representing file path</returns>
        /// <remarks>If you don't want to Get Real File Path, you can leave <c>currentFileName</c> null and fill <c>fileExtension</c>, and otherwise if you do need for real file name, you have to fill <c>currentFileName</c>.</remarks>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        string GetFilePath(string? currentFileName, string? fileExtension);

        /// <summary>
        /// Returns final path
        /// </summary>
        /// <returns>An <see cref="string"/> value that representing path</returns>
        /// <exception cref="NullReferenceException"></exception>
        string GetPath();
    }
}