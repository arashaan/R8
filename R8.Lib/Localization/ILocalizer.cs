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
    /// Returns User-defined dictionary value based on Database
    /// </summary>
    public interface ILocalizer
    {
        /// <summary>
        /// Refreshes internal dictionary
        /// </summary>
        /// <returns></returns>
        Task RefreshAsync();

        /// <summary>
        /// Gets default culture
        /// </summary>
        CultureInfo DefaultCulture { get; }

        /// <summary>
        /// Gets list of supported cultures
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
        /// <returns></returns>
        LocalizerContainer this[Expression<Func<string>> key] { get; }
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
            _dictionary = new Dictionary<string, LocalizerContainer>();
        }

        private readonly Dictionary<string, LocalizerContainer> _dictionary;

        /// <summary>
        /// Refreshes internal dictionary
        /// </summary>
        /// <returns></returns>
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
        /// <param name="key">A key to find in internal dictionary</param>
        /// <param name="culture">Specific culture to search in</param>
        public string this[string key, CultureInfo culture]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    return null;

                var localized = TryGetValue(culture, key).Get(culture, false, true);
                return localized;
            }
        }

        /// <summary>
        /// Gets value from internal dictionary
        /// </summary>
        /// <param name="key">A key to find in internal dictionary</param>
        public LocalizerContainer this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    return null;

                var localized = TryGetValue(CultureInfo.CurrentCulture, key);
                return localized;
            }
        }

        /// <summary>
        /// Gets value from internal dictionary
        /// </summary>
        /// <param name="key">A key to find in internal dictionary</param>
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

        public LocalizerContainer TryGetValue(CultureInfo culture, string key)
        {
            var (_, container) = _dictionary.FirstOrDefault(x => x.Key.Equals(key));
            if (container == null)
                return new LocalizerContainer(culture, key);

            foreach (var containerCulture in container.Cultures)
                containerCulture.Value = HttpUtility.HtmlDecode(containerCulture.Value);

            return container;
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