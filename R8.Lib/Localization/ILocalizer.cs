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
        private readonly LocalizerConfiguration _configuration;

        public List<CultureInfo> SupportedCultures { get; set; }

        public CultureInfo DefaultCulture { get; set; }

        /// <summary>
        /// Returns User-defined dictionary value based on Database
        /// </summary>
        /// <param name="configuration">A <see cref="LocalizerConfiguration"/> object that represents initializing data.</param>
        public Localizer(LocalizerConfiguration configuration)
        {
            _configuration = configuration;
            _dictionary = new Dictionary<string, LocalizerContainer>();
        }

        private readonly Dictionary<string, LocalizerContainer> _dictionary;

        public async Task RefreshAsync()
        {
            if (_configuration == null)
                throw new NullReferenceException($"'{nameof(_configuration)}' expected to be filled");

            if (SupportedCultures == null || !SupportedCultures.Any())
                throw new NullReferenceException($"'{nameof(SupportedCultures)}' expected to be filled");

            if (string.IsNullOrEmpty(_configuration.Folder))
                throw new ArgumentNullException($"Missing {nameof(_configuration.Folder)}");

            if (string.IsNullOrEmpty(_configuration.FileName))
                throw new ArgumentNullException($"Missing {nameof(_configuration.FileName)}");

            _dictionary.Clear();
            foreach (var supportedCulture in SupportedCultures)
                await HandleLanguageAsync(supportedCulture).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles dictionary based on given culture.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns>A <see cref="Task"/> that represents asynchronous operation.</returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task HandleLanguageAsync(CultureInfo culture)
        {
            var language = culture.GetTwoLetterCulture();

            var jsonFile = $"{_configuration.FileName}.{language}.json";
            var jsonPath = Path.Combine(_configuration.Folder, jsonFile);

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
                        _dictionary.Add(key, new LocalizerContainer(culture, value));
                    }
                    else
                    {
                        container.Set(culture, value);
                    }
                }
            }

            sr.Dispose();
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