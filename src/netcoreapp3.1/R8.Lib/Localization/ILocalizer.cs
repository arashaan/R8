using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
        /// Refreshes internal dictionary to update to the latest data.
        /// </summary>
        /// <returns>A <see cref="Task"/> object for asynchronous operation.</returns>
        Task RefreshAsync();

        /// <summary>
        /// Returns a <see cref="Dictionary{TKey,TValue}"/> object that representing collection of words and translations.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, LocalizerContainer> GetDictionary();

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

        // /// <summary>
        // /// Gets an enumerator constant that representing current provider type.
        // /// </summary>
        // LocalizerProvider Provider { get; }

        /// <summary>
        /// Gets value from internal dictionary
        /// </summary>
        /// <param name="key">A key to find in internal dictionary</param>
        LocalizerContainer this[string key] { get; }

        /// <summary>
        /// Gets value from internal dictionary
        /// </summary>
        /// <param name="key">A key to find in internal dictionary</param>
        /// <returns>A <see cref="LocalizerContainer"/> component.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        LocalizerContainer this[Expression<Func<string>> key] { get; }
    }

    /// <summary>
    /// Returns User-defined dictionary value based on Database
    /// </summary>
    public class Localizer : ILocalizer
    {
        private readonly ILocalizerProvider _provider;

        public List<CultureInfo> SupportedCultures => _provider.SupportedCultures;

        public CultureInfo DefaultCulture => _provider.DefaultCulture;

        /// <summary>
        /// Returns User-defined dictionary value based on Database
        /// </summary>
        /// <param name="provider">A <see cref="ILocalizerCultureProvider"/> object that represents initializing data.</param>
        public Localizer(ILocalizerProvider provider)
        {
            _provider = provider;
            _dictionary = new Dictionary<string, LocalizerContainer>();
        }

        private readonly Dictionary<string, LocalizerContainer> _dictionary;

        public Dictionary<string, LocalizerContainer> GetDictionary() => _dictionary;

        public ILocalizerProvider GetProvider() => _provider;

        public async Task RefreshAsync()
        {
            if (_provider == null)
                throw new NullReferenceException($"'{nameof(_provider)}' expected to be filled");

            if (DefaultCulture == null)
                throw new NullReferenceException($"'{nameof(DefaultCulture)}' expected to be filled");

            if (SupportedCultures == null || !SupportedCultures.Any())
                throw new NullReferenceException($"'{nameof(SupportedCultures)}' expected to be filled");

            if (_provider == null)
                throw new NullReferenceException($"{nameof(_provider)} must be implemented.");

            if (_provider is LocalizerCustomProvider dbProvider)
            {
                var tempDic = dbProvider.Dictionary?.Compile().Invoke();
                if (tempDic == null || !tempDic.Any())
                    tempDic = await dbProvider.DictionaryAsync.Compile().Invoke();

                if (tempDic?.Any() == true)
                {
                    _dictionary.Clear();
                    foreach (var (key, localizerContainer) in tempDic)
                        _dictionary.Add(key, localizerContainer);
                }
            }
            else if (_provider is LocalizerJsonProvider jsonProvider)
            {
                if (string.IsNullOrEmpty(jsonProvider.Folder))
                    throw new ArgumentNullException($"Missing {nameof(jsonProvider.Folder)}");

                if (string.IsNullOrEmpty(jsonProvider.FileName))
                    throw new ArgumentNullException($"Missing {nameof(jsonProvider.FileName)}");

                _dictionary.Clear();
                foreach (var supportedCulture in SupportedCultures)
                {
                    var language = supportedCulture.GetTwoLetterCulture();

                    var jsonFile = $"{jsonProvider.FileName}.{language}.json";
                    var jsonPath = Path.Combine(jsonProvider.Folder, jsonFile);

                    using var sr = new StreamReader(jsonPath, Encoding.UTF8);
                    var jsonString = await sr.ReadToEndAsync().ConfigureAwait(false);
                    var dic = HandleDictionary(jsonString);
                    if (dic?.Any() == true)
                    {
                        foreach (var (key, value) in dic)
                        {
                            var (_, container) = _dictionary.FirstOrDefault(x => x.Key.Equals(key));
                            if (container == null)
                            {
                                _dictionary.Add(key, new LocalizerContainer(supportedCulture, value));
                            }
                            else
                            {
                                container.Set(supportedCulture, value);
                            }
                        }
                    }

                    sr.Dispose();
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Represents a <see cref="Dictionary{TKey,TValue}"/> object from JSON data.
        /// </summary>
        /// <param name="jsonString">A <see cref="string"/> value that representing JSON data.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> object.</returns>
        public static Dictionary<string, string> HandleDictionary(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
                throw new ArgumentNullException($"{jsonString} expected to being in JSON format.");

            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
            var dic = new Dictionary<string, string>();
            foreach (var (key, value) in json)
                dic.Add(key, HttpUtility.HtmlEncode(value));

            return dic;
        }

        /// <summary>
        /// Retrieves <see cref="string"/> key from given expression.
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>A <see cref="string"/> value.</returns>
        public static string GetKey(Expression<Func<string>> key)
        {
            return key.Body switch
            {
                MethodCallExpression call => key.Compile().Invoke(),
                MemberExpression member => member.Member.Name,
                ConstantExpression constant => constant.Value.ToString(),
                _ => throw new ArgumentException(
                    $"{nameof(key)} must be an '{nameof(MemberExpression)}' type expression")
            };
        }

        public string this[string key, CultureInfo culture] =>
            string.IsNullOrEmpty(key) ? null : GetValue(culture, key).Get(culture, false);

        public LocalizerContainer this[string key] =>
            string.IsNullOrEmpty(key) ? null : GetValue(CultureInfo.CurrentCulture, key);

        public LocalizerContainer this[Expression<Func<string>> key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                var myKey = GetKey(key);
                return this[myKey];
            }
        }

        /// <summary>
        /// Returns an equivalent translation for given key.
        /// </summary>
        /// <param name="culture">A <see cref="CultureInfo"/>.</param>
        /// <param name="key">A <see cref="string"/> value that should be checked for translation.</param>
        /// <returns>A <see cref="LocalizerContainer"/> object.</returns>
        public LocalizerContainer GetValue(CultureInfo culture, string key)
        {
            var (_, container) = _dictionary.FirstOrDefault(x => x.Key.Equals(key));
            if (container == null)
                return new LocalizerContainer(culture, key);

            foreach (var containerCulture in container.Cultures)
                containerCulture.Value = HttpUtility.HtmlDecode(containerCulture.Value);

            return container;
        }
    }
}