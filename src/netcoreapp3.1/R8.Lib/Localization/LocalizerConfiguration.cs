using System.Collections.Generic;
using System.Globalization;

namespace R8.Lib.Localization
{
    public class LocalizerConfiguration
    {
        /// <summary>
        /// A collection of supported cultures.
        /// </summary>
        public List<CultureInfo> SupportedCultures { get; set; }

        /// <summary>
        /// Default culture.
        /// </summary>
        public CultureInfo DefaultCulture { get; set; }

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