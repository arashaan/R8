namespace R8.Lib.Localization
{
    public class LocalizerConfiguration
    {
        /// <summary>
        /// Absolute path under project root
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// Filename without culture and extension : dictionary => to be "dictionary.tr.json"
        /// </summary>
        public string FileName { get; set; }
    }
}