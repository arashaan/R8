using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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

        public virtual async Task<Dictionary<string, LocalizerContainer>> RefreshAsync(IEnumerable<CultureInfo> cultures)
        {
            if (string.IsNullOrEmpty(Folder))
                throw new NullReferenceException($"Missing {nameof(Folder)}");

            if (string.IsNullOrEmpty(FileName))
                throw new NullReferenceException($"Missing {nameof(FileName)}");

            var dictionary = new Dictionary<string, LocalizerContainer>();
            foreach (var culture in cultures)
            {
                var language = culture.TwoLetterISOLanguageName;

                var jsonFile = $"{FileName}.{language}.json";
                var path = Path.Combine(Folder, jsonFile);

                using var streamReader = new StreamReader(path, Encoding.UTF8);
                var json = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                if (string.IsNullOrEmpty(json))
                    throw new ArgumentNullException($"{json} expected to being in JSON format.");

                var dictionaryFromJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                foreach (var (key, value) in dictionaryFromJson)
                {
                    var encodedValue = HttpUtility.HtmlEncode(value);
                    var (_, container) = dictionary.FirstOrDefault(pair => pair.Key.Equals(key, StringComparison.InvariantCulture));
                    if (container != null)
                    {
                        if (string.IsNullOrEmpty(container[culture]) || !container[culture].Equals(value, StringComparison.InvariantCulture))
                            dictionary[key].Set(culture, encodedValue);

                        continue;
                    }

                    dictionary.Add(key, new LocalizerContainer(culture, encodedValue));
                }

                streamReader.Dispose();
            }

            return dictionary;
        }
    }
}