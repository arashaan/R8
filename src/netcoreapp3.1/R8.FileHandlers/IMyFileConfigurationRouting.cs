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
        /// <remarks>If you are using a windows framework, fill this property with full path. and otherwise, if you have an specific folder under wwwroot to save files, fill with that, else leave it blank.</remarks>
        string Path { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="bool"/> that representing hierarchically directory names from root to file.
        /// If true hierarchically folders, otherwise right in <see cref="Path"/>.
        /// example: <example><c>/<see cref="Path"/>/2020/10/03/file.xyz</c></example>.
        /// </summary>
        bool? HierarchicallyDateFolders { get; set; }

        ///// <summary>
        ///// Gets or sets an <see cref="string"/> value that representing application root folder path.
        ///// </summary>
        ///// <remarks>If you using a non-web project, leave this blank.</remarks>
        //public string BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="bool"/> value that representing if file should be named as real name or <c>Guid.NewGuid</c>.
        /// </summary>
        public bool? SaveAsRealName { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="bool"/> value that overwrite on existing file or create new file with incremented number
        /// </summary>
        public bool? OverwriteExistingFile { get; set; }
    }
}