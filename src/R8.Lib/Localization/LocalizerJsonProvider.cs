using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R8.Lib.Localization
{
    /// <summary>
    /// A provider to being used as json provider in type of <see cref="ILocalizerProvider"/>.
    /// </summary>
    public class LocalizerJsonProvider : ILocalizerProvider, ILocalizerRefresher
    {
        /// <summary>
        /// A provider to being used as json provider in type of <see cref="ILocalizerProvider"/>.
        /// </summary>
        public LocalizerJsonProvider()
        {
        }

        /// <summary>
        /// Gets or sets full path that containing dictionary json files.
        /// </summary>
        public virtual string Folder { get; set; }

        /// <summary>
        /// Gets or sets filename without culture and extension. MAYBE CHANGED IN FUTURE VERSIONS.
        /// </summary>
        /// <remarks>dictionary => to be "dictionary.tr.json"</remarks>
        public virtual string FileName { get; set; }

        private void CheckRequirements()
        {
            if (string.IsNullOrEmpty(Folder))
                throw new NullReferenceException($"Missing {nameof(Folder)}");

            if (string.IsNullOrEmpty(FileName))
                throw new NullReferenceException($"Missing {nameof(FileName)}");
        }

        private string GetJsonPath(CultureInfo culture)
        {
            var language = culture.TwoLetterISOLanguageName;

            var jsonFile = $"{FileName}.{language}.json";
            var jsonPath = Path.Combine(Folder, jsonFile);
            return jsonPath;
        }

        public virtual async Task<Dictionary<string, LocalizerContainer>> RefreshAsync(IEnumerable<CultureInfo> cultures)
        {
            CheckRequirements();

            var dictionary = new Dictionary<string, LocalizerContainer>();
            foreach (var culture in cultures)
            {
                var jsonPath = GetJsonPath(culture);

                using var sr = new StreamReader(jsonPath, Encoding.UTF8);
                var jsonString = await sr.ReadToEndAsync().ConfigureAwait(false);
                HandleDic(ref dictionary, jsonString, culture);

                sr.Dispose();
            }

            return dictionary;
        }

        private static void HandleDic(ref Dictionary<string, LocalizerContainer> dictionary, string jsonString, CultureInfo culture)
        {
            var dic = Localizer.HandleDictionary(jsonString);
            foreach (var (key, value) in dic)
            {
                if (dic.Count > 0)
                {
                    var (_, localizerContainer) = dictionary.FirstOrDefault(pair => pair.Key.Equals(key, StringComparison.InvariantCulture));
                    if (localizerContainer != null)
                    {
                        var check = localizerContainer[culture];
                        if (string.IsNullOrEmpty(check) || !check.Equals(value, StringComparison.InvariantCulture))
                            dictionary[key].Set(culture, value);

                        continue;
                    }
                }

                dictionary.Add(key, new LocalizerContainer(culture, value));
            }
        }

        public virtual Dictionary<string, LocalizerContainer> Refresh(IEnumerable<CultureInfo> cultures)
        {
            CheckRequirements();

            var dictionary = new Dictionary<string, LocalizerContainer>();
            foreach (var culture in cultures)
            {
                var jsonPath = GetJsonPath(culture);

                using var sr = new StreamReader(jsonPath, Encoding.UTF8);
                var jsonString = sr.ReadToEnd();
                HandleDic(ref dictionary, jsonString, culture);

                sr.Dispose();
            }

            return dictionary;
        }
    }
}