using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R8.Lib.Localization
{
    public class LocalizerJsonProvider : ILocalizerProvider
    {
        public virtual List<CultureInfo> SupportedCultures { get; set; }
        public virtual CultureInfo DefaultCulture { get; set; }

        /// <summary>
        /// Absolute path under project root
        /// </summary>
        public virtual string Folder { get; set; }

        /// <summary>
        /// Filename without culture and extension : dictionary => to be "dictionary.tr.json"
        /// </summary>
        public virtual string FileName { get; set; }

        public virtual async Task RefreshAsync(IServiceProvider serviceProvider, Dictionary<string, LocalizerContainer> dictionary)
        {
            if (string.IsNullOrEmpty(Folder))
                throw new ArgumentNullException($"Missing {nameof(Folder)}");

            if (string.IsNullOrEmpty(FileName))
                throw new ArgumentNullException($"Missing {nameof(FileName)}");

            dictionary.Clear();
            foreach (var culture in SupportedCultures)
            {
                var language = culture.GetTwoLetterCulture();

                var jsonFile = $"{FileName}.{language}.json";
                var jsonPath = Path.Combine(Folder, jsonFile);

                using var sr = new StreamReader(jsonPath, Encoding.UTF8);
                var jsonString = await sr.ReadToEndAsync().ConfigureAwait(false);
                var dic = Localizer.HandleDictionary(jsonString);
                if (dic?.Any() == true)
                {
                    foreach (var (key, value) in dic)
                    {
                        var (_, container) = dictionary.FirstOrDefault(x => x.Key.Equals(key));
                        if (container != null)
                        {
                            dictionary[key].Set(culture, value);
                        }
                        else
                        {
                            dictionary.Add(key, new LocalizerContainer(culture, value));
                        }
                    }
                }

                sr.Dispose();
            }
        }

        public virtual void Refresh(IServiceProvider serviceProvider, Dictionary<string, LocalizerContainer> dictionary)
        {
            if (string.IsNullOrEmpty(Folder))
                throw new ArgumentNullException($"Missing {nameof(Folder)}");

            if (string.IsNullOrEmpty(FileName))
                throw new ArgumentNullException($"Missing {nameof(FileName)}");

            dictionary.Clear();
            foreach (var supportedCulture in SupportedCultures)
            {
                var language = supportedCulture.GetTwoLetterCulture();

                var jsonFile = $"{FileName}.{language}.json";
                var jsonPath = Path.Combine(Folder, jsonFile);

                using var sr = new StreamReader(jsonPath, Encoding.UTF8);
                var jsonString = sr.ReadToEnd();
                var dic = Localizer.HandleDictionary(jsonString);
                if (dic?.Any() == true)
                    foreach (var (key, value) in dic)
                        dictionary.Add(key, new LocalizerContainer(supportedCulture, value));

                sr.Dispose();
            }
        }
    }
}