namespace R8.FileHandlers
{
    /// <summary>
    /// Represents a instance of <see cref="MyFile"/>
    /// </summary>
    internal class MyFile : IMyFile
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

        public string FilePath { get; set; }

        public long FileSize { get; set; }

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