namespace R8.Lib.FileHandlers
{
    /// <summary>
    /// Represents a instance of <see cref="MyFile"/>
    /// </summary>
    public class MyFile
    {
        /// <summary>
        /// Represents a instance of <see cref="MyFile"/>
        /// </summary>
        /// <param name="filePath">An <see cref="string"/> value that representing file's absolute path</param>
        public MyFile(string filePath)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Represents a instance of <see cref="MyFile"/>
        /// </summary>
        /// <param name="filePath">An <see cref="string"/> value that representing file's absolute path</param>
        /// <param name="thumbnailPath">An <see cref="string"/> value that representing image file's preview</param>
        public MyFile(string filePath, string thumbnailPath) : this(filePath)
        {
            ThumbnailPath = thumbnailPath;
        }

        /// <summary>
        /// Represents a instance of <see cref="MyFile"/>
        /// </summary>
        /// <param name="filePath">An <see cref="string"/> value that representing file's absolute path</param>
        /// <param name="thumbnailPath">An <see cref="string"/> value that representing image file's preview</param>
        /// <param name="fileSize">A <see cref="long"/> value that representing file length</param>
        public MyFile(string filePath, string thumbnailPath, long fileSize) : this(filePath, thumbnailPath)
        {
            FileSize = fileSize;
        }

        /// <summary>
        /// Represents a instance of <see cref="MyFile"/>
        /// </summary>
        public MyFile()
        {
        }

        /// <summary>
        /// Gets or sets file's absolute path
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets file' length
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Gets or sets Thumbnail for image file's absolute path
        /// </summary>
        /// <remarks>Only for PDF preview, so far</remarks>
        public string ThumbnailPath { get; set; }

        public static implicit operator string(MyFile file)
        {
            return file?.ToString();
        }

        public void Deconstruct(out string path, out string thumbnail)
        {
            path = FilePath;
            thumbnail = ThumbnailPath;
        }

        public static explicit operator MyFile(string path)
        {
            return new MyFile(path);
        }

        public override string ToString()
        {
            return FilePath;
        }
    }
}