using System.Collections.Generic;
using System.Globalization;

namespace R8.Lib.Localization
{
    public interface ILocalizerCultureProvider
    {
        /// <summary>
        /// Gets or sets a collection of supported cultures for instance.
        /// </summary>
        public List<CultureInfo> SupportedCultures { get; set; }

        /// <summary>
        /// Gets or sets Default culture for instance.
        /// </summary>
        public CultureInfo DefaultCulture { get; set; }
    }
}