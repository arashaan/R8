namespace R8.FileHandlers
{
    /// <summary>
    /// An <see cref="IMyFileConfigurationRouting"/> interface.
    /// </summary>
    public interface IMyFileConfigurationRouting
    {
        /// <summary>
        /// Gets or sets an <see cref="string"/> value that representing specific sub folder under root path for uploading files and can be null.
        /// </summary>
        string Folder { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="bool"/> that representing hierarchically directory names from root to file.
        /// If true hierarchically folders, otherwise right in <see cref="Folder"/>.
        /// example: <example><c>/<see cref="Folder"/>/2020/10/03/file.xyz</c></example>.
        /// </summary>
        bool? HierarchicallyFolderNameByDate { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="string"/> value that representing application root folder path.
        /// </summary>
        public string RootPath { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="bool"/> value that representing if file should be named as real name or <c>Guid.NewGuid</c>.
        /// </summary>
        public bool? RealFilename { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="bool"/> value that overwrite on existing file or create new file with incremented number
        /// </summary>
        public bool? OverwriteFile { get; set; }
    }
}