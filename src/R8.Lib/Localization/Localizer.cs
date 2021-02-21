using Microsoft.Extensions.Caching.Memory;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace R8.Lib.Localization
{
    /// <summary>
    /// Returns User-defined dictionary value based on Database
    /// </summary>
    public class Localizer : ILocalizer
    {
        private readonly IMemoryCache _cacheStorage;

        /// <summary>
        /// Returns User-defined dictionary value based on Database
        /// </summary>
        /// <param name="configuration">A <see cref="LocalizerConfiguration"/> object that represents initializing data.</param>
        /// <param name="cacheStorage"></param>
        public Localizer(LocalizerConfiguration configuration, IMemoryCache cacheStorage)
        {
            Configuration = configuration ??
                            throw new ArgumentNullException($"{nameof(configuration)} expected to be not null.");

            _cacheStorage = cacheStorage;
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

        private const string CacheKey = "ILocalizerCacheStorage";
        private const long CacheSize = 1000;

        public async Task RefreshAsync(bool forceToUpdate = false)
        {
            var provider = (ILocalizerRefresher)Configuration.Provider;

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
                if (_cacheStorage == null)
                    throw new NullReferenceException($"Unable to find registered {nameof(IMemoryCache)} service.");

                var hasCache = _cacheStorage.TryGetValue(CacheKey, out dictionary);
                dictionary ??= new Dictionary<string, LocalizerContainer>();
                if (forceToUpdate || !hasCache || dictionary.Count == 0)
                {
                    if (hasCache) _cacheStorage.Remove(CacheKey);
                    dictionary = await provider.RefreshAsync(SupportedCultures).ConfigureAwait(false);
                    _cacheStorage.Set(CacheKey, dictionary, new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = slidingExpiration,
                        Size = CacheSize
                    });

                    if (forceToUpdate)
                        Dictionary.Clear();
                }
            }
            else
            {
                dictionary = await provider.RefreshAsync(SupportedCultures).ConfigureAwait(false);
            }

            Dictionary = dictionary;
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

            if (Configuration.UsageCounter)
                container.SetCounter(container.GetCounter() + 1);

            return container;
        }
    }
}