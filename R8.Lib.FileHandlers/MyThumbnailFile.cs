namespace R8.Lib.FileHandlers
{
    public class MyThumbnailFile : MyFile
    {
        private string _thumbnailPath;

        public MyThumbnailFile()
        {
        }

        public MyThumbnailFile(MyFile file)
        {
            if (file != null)
                FilePath = file.FilePath;
        }

        public MyThumbnailFile(string filePath, string thumbnailPath)
        {
            FilePath = filePath;
            ThumbnailPath = thumbnailPath;
        }

        public MyThumbnailFile(string thumbnailPath)
        {
            ThumbnailPath = thumbnailPath;
        }

        public string ThumbnailPath
        {
            get => _thumbnailPath;
            set => _thumbnailPath = value.StartsWith("/") ? value : $"/{value}";
        }

        public void Deconstruct(out string path, out string thumbnail)
        {
            path = FilePath;
            thumbnail = ThumbnailPath;
        }

        #region Overrides of MyFile

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return FilePath;
        }

        #endregion Overrides of MyFile

        public static implicit operator string(MyThumbnailFile file)
        {
            return file?.ToString();
        }

        public static explicit operator MyThumbnailFile(string path)
        {
            return new MyThumbnailFile(path);
        }
    }
}