﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace R8.Lib.Localization
{
    /// <summary>
    /// An initializing configuration store for <see cref="ILocalizer"/>.
    /// </summary>
    public class LocalizerConfiguration
    {
        /// <summary>
        /// Gets or sets a list of supported cultures to be used.
        /// </summary>
        /// <remarks>First culture will be used as Default Culture.</remarks>
        public List<CultureInfo> SupportedCultures { get; set; }

        /// <summary>
        /// Gets or sets if dictionary should be saved in memory cache.
        /// </summary>
        /// <remarks>If false, Dictionary will be refreshed on every request and may cause performance issues.</remarks>
        public bool UseMemoryCache { get; set; }

        /// <summary>
        /// Gets or sets expiration limit for memory cache.
        /// </summary>
        /// <remarks>This property will be applicable when <see cref="UseMemoryCache"/> equals <c>true</c></remarks>
        public TimeSpan? CacheSlidingExpiration { get; set; }

        /// <summary>
        /// Gets or sets a provider of <see cref="ILocalizerProvider"/> type to refresh dictionary.
        /// </summary>
        public ILocalizerProvider Provider { get; set; }
    }
}