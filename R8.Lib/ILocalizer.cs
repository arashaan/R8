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

        string this[string text, bool titleize = false] { get; }

        /// <summary>
        /// Gets value from internal dictionary
        /// </summary>
        /// <param name="key">A key to find in internal dictionary</param>
        /// <param name="ignoreNormalize">Ignores normalize text like ThisIsFake to This Is Fake</param>
        /// <returns></returns>
        string this[Expression<Func<string>> key, bool ignoreNormalize = false] { get; }
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
        }

        private const string CacheKey = "JsonLocalizer";

        private static string GetCultureKey(CultureInfo culture) => $"{CacheKey}_{culture.GetTwoLetterCulture()}";

        private Dictionary<string, string> _dictionary;

        /// <summary>
        /// Refreshes internal dictionary
        /// </summary>
        /// <returns></returns>
        public async Task RefreshAsync()
        {
            if (SupportedCultures == null || !SupportedCultures.Any())
                throw new NullReferenceException($"'{nameof(SupportedCultures)}' expected to be filled");

            foreach (var path in from culture in SupportedCultures
                                 let language = culture.GetTwoLetterCulture()
                                 let currentCacheKey = GetCultureKey(culture)
                                 select $"{_configuration.FileName}.{language}.json"
                into fileName
                                 select !string.IsNullOrEmpty(_configuration.Folder)
                                     ? Path.Combine(Directory.GetCurrentDirectory(), _configuration.Folder, fileName)
                                     : Path.Combine(Directory.GetCurrentDirectory(), fileName))
            {
                var jsonString = await File.ReadAllTextAsync(path, Encoding.UTF8).ConfigureAwait(false);
                if (string.IsNullOrEmpty(jsonString))
                    return;

                var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
                if (json == null)
                    return;

                foreach (var (key, value) in json)
                    _dictionary.Add(key, HttpUtility.HtmlEncode(value));
            }
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
        /// <param name="titleize">Ignores normalize text like ThisIsFake to This Is Fake</param>
        public string this[string text, bool titleize = false]
        {
            get
            {
                // if (text == null)
                // throw new ArgumentNullException(nameof(text));

                if (string.IsNullOrEmpty(text))
                    return string.Empty;

                var tempKey = text.Replace(" ", "");
                var hasLocalization = TryGetValue(tempKey, out var localized);
                if (!hasLocalization)
                    return text;

                return localized;
            }
        }

        /// <summary>
        /// Gets value from internal dictionary
        /// </summary>
        /// <param name="key">A key to find in internal dictionary</param>
        /// <param name="ignoreNormalize">Ignores normalize text like ThisIsFake to This Is Fake</param>
        public string this[Expression<Func<string>> key, bool ignoreNormalize = false]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                var myKey = GetKey(key);
                var localized = this[myKey, !ignoreNormalize];
                return localized;
            }
        }

        public bool TryGetValue(string key, out string localized)
        {
            var currentCulture = CultureInfo.CurrentCulture;

            var mean = string.Empty;
            var currentCacheKey = GetCultureKey(CultureInfo.CurrentCulture);
            if (_dictionary.TryGetValue(currentCacheKey, out var value))
            {
                try
                {
                    mean = HttpUtility.HtmlDecode(value);
                }
                catch
                {
                }
            }
            //if (currentCulture.Equals(new CultureInfo("tr")) || currentCulture.Equals(new CultureInfo("fa")))
            //{
            //}
            //else
            //{
            //    if (_configuration.StaticResourceType == null)
            //        throw new NullReferenceException(nameof(_configuration.StaticResourceType) + " must be filled with any *.resx");

            //    var resManager = new ResourceManager(_configuration.StaticResourceType);
            //    try
            //    {
            //        var result = resManager.GetString(key);
            //        if (result != null)
            //        {
            //            mean = result;
            //        }
            //    }
            //    catch
            //    {
            //    }
            //}

            localized = !string.IsNullOrEmpty(mean)
                    ? mean
                    : key;

            return !string.IsNullOrEmpty(mean);
        }
    }

    public class LocalizerConfiguration
    {
        public Type StaticResourceType { get; set; }
        public TimeSpan CacheControl { get; set; }

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