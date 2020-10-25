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

namespace R8.Lib
{
    /// <summary>
    /// Returns User-defined dictionary value based on Database
    /// </summary>
    public interface ILocalizer
    {
        /// <summary>
        /// Refreshes internal dictionary
        /// </summary>
        /// <returns></returns>
        Task RefreshAsync();

        bool TryGetValue(string key, out string localized);

        /// <summary>
        /// Gets default culture
        /// </summary>
        CultureInfo DefaultCulture { get; }

        /// <summary>
        /// Gets list of supported cultures
        /// </summary>
        List<CultureInfo> SupportedCultures { get; }

        string this[string text] { get; }

        /// <summary>
        /// Gets value from internal dictionary
        /// </summary>
        /// <param name="key">A key to find in internal dictionary</param>
        /// <returns></returns>
        string this[Expression<Func<string>> key] { get; }
    }

    /// <summary>
    /// Returns User-defined dictionary value based on Database
    /// </summary>
    public class Localizer : ILocalizer
    {
        private readonly LocalizerConfiguration _configuration;

        /// <summary>
        /// Gets or sets list of supported cultures
        /// </summary>
        public List<CultureInfo> SupportedCultures { get; set; }

        /// <summary>
        /// Gets or sets default culture
        /// </summary>
        public CultureInfo DefaultCulture { get; set; }

        public Localizer(LocalizerConfiguration configuration)
        {
            _configuration = configuration;
            _dictionary = new Dictionary<string, Dictionary<string, string>>();
        }

        private readonly Dictionary<string, Dictionary<string, string>> _dictionary;

        /// <summary>
        /// Refreshes internal dictionary
        /// </summary>
        /// <returns></returns>
        public async Task RefreshAsync()
        {
            if (SupportedCultures == null || !SupportedCultures.Any())
                throw new NullReferenceException($"'{nameof(SupportedCultures)}' expected to be filled");

            var folder = _configuration.Folder ??
                         throw new ArgumentNullException($"Missing {nameof(_configuration.Folder)}");

            var fileName = _configuration.FileName ??
                           throw new ArgumentNullException($"Missing {nameof(_configuration.FileName)}");

            _dictionary.Clear();
            foreach (var supportedCulture in SupportedCultures)
                await HandleLanguageAsync(supportedCulture).ConfigureAwait(false);
        }

        public async Task HandleLanguageAsync(CultureInfo culture)
        {
            var language = culture.GetTwoLetterCulture();

            var jsonFile = $"{_configuration.FileName}.{language}.json";
            var jsonPath = Path.Combine(_configuration.Folder, jsonFile);

            using var sr = new StreamReader(jsonPath, Encoding.UTF8);
            var jsonString = await sr.ReadToEndAsync().ConfigureAwait(false);
            var dic = HandleDictionary(jsonString);
            _dictionary.Add(language, dic);

            sr.Dispose();
        }

        public static Dictionary<string, string> HandleDictionary(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
                throw new Exception($"JSON file is not in an expected format");

            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
            var dic = new Dictionary<string, string>();
            foreach (var (key, value) in json)
                dic.Add(key, HttpUtility.HtmlEncode(value));

            return dic;
        }

        public static string GetKey(Expression<Func<string>> key)
        {
            var myKey = key.Body switch
            {
                MethodCallExpression call => key.Compile().Invoke(),
                MemberExpression member => member.Member.Name,
                ConstantExpression constant => constant.Value.ToString(),
                _ => throw new ArgumentException(
                    $"{nameof(key)} must be an '{nameof(MemberExpression)}' type expression")
            };

            return myKey;
        }

        /// <summary>
        /// Gets value from internal dictionary
        /// </summary>
        /// <param name="text">A key to find in internal dictionary</param>
        /// <param name="culture">Culture-specific translation</param>
        public string this[string text, CultureInfo culture]
        {
            get
            {
                if (string.IsNullOrEmpty(text))
                    return null;

                var hasLocalization = TryGetValue(culture, text, out var localized);
                return hasLocalization
                    ? localized
                    : text;
            }
        }

        /// <summary>
        /// Gets value from internal dictionary
        /// </summary>
        /// <param name="text">A key to find in internal dictionary</param>
        public string this[string text] => this[text, CultureInfo.CurrentCulture];

        /// <summary>
        /// Gets value from internal dictionary
        /// </summary>
        /// <param name="key">A key to find in internal dictionary</param>
        public string this[Expression<Func<string>> key] => this[key, CultureInfo.CurrentCulture];

        /// <summary>
        /// Gets value from internal dictionary
        /// </summary>
        /// <param name="key">A key to find in internal dictionary</param>
        /// <param name="culture">Culture-specific translation</param>
        public string this[Expression<Func<string>> key, CultureInfo culture]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                var myKey = GetKey(key);
                return this[myKey, culture];
            }
        }

        public bool TryGetValue(CultureInfo culture, string key, out string localized)
        {
            var mean = string.Empty;
            var currentCulture = culture.GetTwoLetterCulture();
            var (dictionaryCulture, dictionary) = _dictionary.FirstOrDefault(x => x.Key == currentCulture);
            if (dictionary == null)
            {
                localized = key;
                return false;
            }

            if (dictionary.TryGetValue(key, out var value))
            {
                localized = HttpUtility.HtmlDecode(value);
                return true;
            }

            localized = key;
            return false;
        }

        public bool TryGetValue(string key, out string localized)
        {
            return TryGetValue(CultureInfo.CurrentCulture, key, out localized);
        }
    }

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