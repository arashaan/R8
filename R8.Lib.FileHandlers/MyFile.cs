namespace R8.Lib.FileHandlers
{
    public class MyFile
    {
        private string _filePath;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public MyFile(string filePath)
        {
            FilePath = filePath;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public MyFile()
        {
        }

        public string FilePath
        {
            get => _filePath;
            set => _filePath = value.StartsWith("/") ? value : $"/{value}";
        }

        public static implicit operator string(MyFile file)
        {
            return file?.ToString();
        }

        public static explicit operator MyFile(string path)
        {
            return new MyFile(path);
        }

        #region Overrides of Object

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return FilePath;
        }

        #endregion Overrides of Object
    }
}