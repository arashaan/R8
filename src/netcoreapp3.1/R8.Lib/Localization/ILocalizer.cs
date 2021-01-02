using Microsoft.Extensions.Caching.Memory;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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
        /// Refreshes internal dictionary based on last update.
        /// </summary>
        /// <param name="forceToUpdate">Asks if dictionary should be enforced to update from supplier, or skip it if not null.</param>
        /// <remarks>Force Update is useful when <see cref="IMemoryCache"/> is not null. otherwise doesn't matter <c>forceToUpdate</c> be neither true or false.</remarks>
        void Refresh(bool forceToUpdate = false);

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
        string this[string key, CultureInfo culture] { get; }

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
        LocalizerContainer this[string key] { get; }
    }

    /// <summary>
    /// Returns User-defined dictionary value based on Database
    /// </summary>
    public class Localizer : ILocalizer
    {
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Returns User-defined dictionary value based on Database
        /// </summary>
        /// <param name="configuration">A <see cref="LocalizerConfiguration"/> object that represents initializing data.</param>
        /// <param name="memoryCache"></param>
        public Localizer(LocalizerConfiguration configuration, IMemoryCache memoryCache)
        {
            Configuration = configuration ??
                            throw new ArgumentNullException($"{nameof(configuration)} expected to be not null.");

            _memoryCache = memoryCache;
            Dictionary = new Dictionary<string, LocalizerContainer>();
        }

        public Dictionary<string, LocalizerContainer> Dictionary { get; private set; }

        public LocalizerConfiguration Configuration { get; internal set; }
        public List<CultureInfo> SupportedCultures => Configuration.SupportedCultures;

        public CultureInfo DefaultCulture
        {
            get
            {
                if (Configuration.SupportedCultures == null || !Configuration.SupportedCultures.Any())
                    throw new NullReferenceException($"{nameof(Configuration.SupportedCultures)} does not have any items.");

                return Configuration.SupportedCultures[0];
            }
        }

        public void Refresh(bool forceToUpdate = false)
        {
            var provider = (ILocalizerRefresher)Configuration.Provider;
            RefreshCore(forceToUpdate, () => Task.FromResult(provider.Refresh(SupportedCultures))).GetAwaiter().GetResult();
        }

        private const string CacheKey = "ILocalizerCacheStorage";
        private const long CacheSize = 1000;

        private async Task RefreshCore(bool forceToUpdate, Func<Task<Dictionary<string, LocalizerContainer>>> func)
        {
            if (Configuration == null)
                throw new NullReferenceException($"'{nameof(Configuration)}' expected to be filled");

            if (SupportedCultures == null || !SupportedCultures.Any())
                throw new NullReferenceException($"'{nameof(SupportedCultures)}' expected to be filled");

            if (Configuration.Provider == null)
                throw new NullReferenceException($"{nameof(Configuration)} must be implemented.");

            var slidingExpiration = Configuration.CacheSlidingExpiration ?? TimeSpan.FromDays(1);
            Dictionary<string, LocalizerContainer> dictionary;

            if (Configuration.UseMemoryCache)
            {
                if (_memoryCache == null)
                    throw new NullReferenceException($"Unable to find registered {nameof(IMemoryCache)} service.");

                var hasCache = _memoryCache.TryGetValue(CacheKey, out dictionary);
                dictionary ??= new Dictionary<string, LocalizerContainer>();
                if (forceToUpdate || !hasCache || dictionary.Count == 0)
                {
                    _memoryCache.Remove(CacheKey);
                    var tempDictionary = await func.Invoke().ConfigureAwait(false);
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = slidingExpiration,
                        Size = CacheSize
                    };
                    _memoryCache.Set(CacheKey, tempDictionary, cacheEntryOptions);
                    dictionary = tempDictionary;
                }
            }
            else
            {
                Dictionary.Clear();
                dictionary = await func.Invoke().ConfigureAwait(false);
            }

            Dictionary = dictionary;
        }

        public async Task RefreshAsync(bool forceToUpdate = false)
        {
            var provider = (ILocalizerRefresher)Configuration.Provider;
            await RefreshCore(forceToUpdate, () => provider.RefreshAsync(SupportedCultures));
        }

        /// <summary>
        /// Represents a <see cref="Dictionary{TKey,TValue}"/> object from JSON data.
        /// </summary>
        /// <param name="jsonString">A <see cref="string"/> value that representing JSON data.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> object.</returns>
        internal static Dictionary<string, string> HandleDictionary(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
                throw new ArgumentNullException($"{jsonString} expected to being in JSON format.");

            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
            var dic = new Dictionary<string, string>();
            foreach (var (key, value) in json)
                dic.Add(key, HttpUtility.HtmlEncode(value));

            return dic;
        }

        public string this[string key, CultureInfo culture] =>
            !string.IsNullOrEmpty(key)
                ? GetValue(culture, key).Get(culture, false)
                : null;

        public LocalizerContainer this[string key] =>
            !string.IsNullOrEmpty(key)
                ? GetValue(CultureInfo.CurrentCulture, key)
                : null;

        /// <summary>
        /// Returns an equivalent translation for given key.
        /// </summary>
        /// <param name="culture">A <see cref="CultureInfo"/>.</param>
        /// <param name="key">A <see cref="string"/> value that should be checked for translation.</param>
        /// <returns>A <see cref="LocalizerContainer"/> object.</returns>
        public LocalizerContainer GetValue(CultureInfo culture, string key)
        {
            if (Dictionary == null)
                return new LocalizerContainer(culture, key);

            var (_, container) = Dictionary.FirstOrDefault(x => x.Key.Equals(key));
            if (container == null)
                return new LocalizerContainer(culture, key);

            foreach (var containerCulture in container.Cultures)
                containerCulture.Value = HttpUtility.HtmlDecode(containerCulture.Value);

            return container;
        }
    }
}