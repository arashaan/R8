using System.Collections.Generic;
using System.Globalization;

namespace R8.Lib.Localization
{
    /// <summary>
    /// Initializes an instance of <see cref="ILocalizerProvider"/>.
    /// </summary>
    public class LocalizerJsonProvider : ILocalizerProvider
    {
        /// <summary>
        /// Absolute path under project root
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// Filename without culture and extension : dictionary => to be "dictionary.tr.json"
        /// </summary>
        public string FileName { get; set; }

        public List<CultureInfo> SupportedCultures { get; set; }
        public CultureInfo DefaultCulture { get; set; }
    }
}