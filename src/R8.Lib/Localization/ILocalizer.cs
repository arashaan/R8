using Microsoft.Extensions.Caching.Memory;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace R8.Lib.Localization
{
    /// <summary>
    /// An <see cref="ILocalizer"/> interface.
    /// </summary>
    public interface ILocalizer
    {
        /// <summary>
        /// Refreshes internal dictionary based on last update.
        /// </summary>
        /// <param name="forceToUpdate">Asks if dictionary should be enforced to update from supplier, or skip it if not null.</param>
        /// <returns>An <see cref="Task"/> object that representing asynchronous operation.</returns>
        /// <remarks>Force Update is useful when <see cref="IMemoryCache"/> is not null. otherwise doesn't matter <c>forceToUpdate</c> be neither true or false.</remarks>
        Task RefreshAsync(bool forceToUpdate = false);

        /// <summary>
        /// Gets default culture.
        /// </summary>
        CultureInfo DefaultCulture { get; }

        /// <summary>
        /// Gets a collection of supported cultures.
        /// </summary>
        List<CultureInfo> SupportedCultures { get; }

        /// <summary>
        /// Gets value from internal dictionary
        /// </summary>
        /// <param name="key">A key to find in internal dictionary</param>
        /// <param name="culture">Specific culture to search in</param>
        /// <param name="returnNullIfEmpty">Return null string if unable to find any translation.</param>
        string this[string key, CultureInfo culture, bool returnNullIfEmpty = true] { get; }

        /// <summary>
        /// Gets initializing configuration.
        /// </summary>
        LocalizerConfiguration Configuration { get; }

        /// <summary>
        /// Gets <see cref="Dictionary{TKey,TValue}"/> object that representing collection of words and translations.
        /// </summary>
        Dictionary<string, LocalizerContainer> Dictionary { get; }

        /// <summary>
        /// Gets value from internal dictionary
        /// </summary>
        /// <param name="key">A key to find in internal dictionary</param>
        /// <remarks><see cref="LocalizerContainer"/> has Direct Cast to <see cref="string"/>. So you don't need to use .ToString()</remarks>
        LocalizerContainer this[string key] { get; }

        /// <summary>
        /// Gets value from internal dictionary
        /// </summary>
        /// <param name="key">A key to find in internal dictionary</param>
        /// <param name="fullName">A <see cref="bool"/> that indicates if key should be like: <c>ENUM_KEY</c> ( ENUM: for type name, KEY: given enum key ), otherwise key will be like: <c>KEY</c>.</param>
        /// <remarks><see cref="LocalizerContainer"/> has Direct Cast to <see cref="string"/>. So you don't need to use .ToString()</remarks>
        LocalizerContainer this[Enum key, bool fullName = false] { get; }

        /// <summary>
        /// Gets value from internal dictionary
        /// </summary>
        /// <param name="key">A key to find in internal dictionary</param>
        /// <param name="culture">Specific culture to search in</param>
        /// <param name="fullName">A <see cref="bool"/> that indicates if key should be like: <c>ENUM_KEY</c> ( ENUM: for type name, KEY: given enum key ), otherwise key will be like: <c>KEY</c>.</param>
        string this[Enum key, CultureInfo culture, bool fullName = false] { get; }
    }
}