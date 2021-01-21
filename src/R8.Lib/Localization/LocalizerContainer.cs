using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using R8.Lib.Enums;
using R8.Lib.JsonExtensions;

namespace R8.Lib.Localization
{
    /// <summary>
    /// Initializes a <see cref="LocalizerContainer"/> to store a word in many languages.
    /// </summary>
    [JsonConverter(typeof(ContainerJsonConverter))]
    public class LocalizerContainer : ICloneable
    {
        /// <summary>
        /// Initializes a <see cref="LocalizerContainer"/> to store a word in many languages.
        /// </summary>
        /// <param name="culture">A <see cref="CultureInfo"/> to store localized text in.</param>
        /// <param name="localized">A <see cref="string"/> value that representing equivalent translation for given culture.</param>
        public LocalizerContainer(CultureInfo culture, string localized) : this()
        {
            Set(culture, localized);
        }

        /// <summary>
        /// Initializes a <see cref="LocalizerContainer"/> to store a word in many languages.
        /// </summary>
        /// <param name="localized">A <see cref="string"/> value that representing equivalent translation for current culture.</param>
        public LocalizerContainer(string localized) : this(CultureInfo.CurrentCulture, localized)
        {
        }

        /// <summary>
        /// Initializes a <see cref="LocalizerContainer"/> to store a word in many languages.
        /// </summary>
        /// <param name="language">A enumerator constant value to store localized text in.</param>
        /// <param name="localized">A <see cref="string"/> value that representing equivalent translation for given culture.</param>
        public LocalizerContainer(Languages language, string localized) : this(
            CultureInfo.GetCultureInfo(language.GetDisplayName()), localized)
        {
        }

        /// <summary>
        /// Initializes a <see cref="LocalizerContainer"/> to store a word in many languages.
        /// </summary>
        /// <param name="language">A <see cref="string"/> value to store localized text in.</param>
        /// <param name="localized">A <see cref="string"/> value that representing equivalent translation for given culture.</param>
        public LocalizerContainer(string language, string localized) : this(CultureInfo.GetCultureInfo(language),
            localized)
        {
        }

        /// <summary>
        /// Initializes a <see cref="LocalizerContainer"/> to store a word in many languages.
        /// </summary>
        public LocalizerContainer()
        {
            _cultures = new List<LocalizerCultureNode>();
        }

        /// <summary>
        /// Gets a collection of specified cultures in current instance.
        /// </summary>
        public IReadOnlyCollection<LocalizerCultureNode> Cultures => _cultures;

        private readonly List<LocalizerCultureNode> _cultures;

        /// <summary>
        /// Deserializes already-serialized JSON to new <see cref="LocalizerContainer"/> instance.
        /// </summary>
        /// <param name="json">A <see cref="string"/> json.</param>
        /// <returns>A <see cref="LocalizerContainer"/> instance.</returns>
        public static LocalizerContainer Deserialize(string json)
        {
            var output = (LocalizerContainer)null;
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    output = JsonConvert.DeserializeObject<LocalizerContainer>(json);
                }
                catch
                {
                }
            }

            if (output == null || !output.HasValue)
                output = new LocalizerContainer(json);

            return output;
        }

        /// <summary>
        /// Represents an indicator that if container has cultures.
        /// </summary>
        public bool HasValue => _cultures.Any(x => !string.IsNullOrEmpty(x.Value));

        /// <summary>
        /// Represents an enumerator constant that shows container value type.
        /// </summary>
        public LocalizerValueType ValueType
        {
            get
            {
                if (Cultures == null || Cultures.Count == 0)
                    return LocalizerValueType.Unknown;

                var node = this.Cultures.FirstOrDefault(x => !string.IsNullOrEmpty(x.Value));
                if (node == null)
                    return LocalizerValueType.Unknown;

                var value = HttpUtility.HtmlDecode(node.Value);
                var matchHtml = Regex.Match(value, @"(<[\w\d]{1,2}>)|(<\/[\w\d]{1,2}>)");
                if (matchHtml.Success)
                    return LocalizerValueType.Html;

                var matchFormat = Regex.Match(value, @"{[\w\d]{1,2}}");
                if (matchFormat.Success)
                    return LocalizerValueType.FormattableText;

                return LocalizerValueType.Text;
            }
        }

        /// <summary>
        /// Serializes current instance to JSON string in specific format.
        /// </summary>
        /// <returns>A <see cref="string"/> json value.</returns>
        public string Serialize()
        {
            var settings = CustomJsonSerializerSettings.Settings;
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            return !HasValue
                ? null
                : JsonConvert.SerializeObject(this, settings);
        }

        public static implicit operator string(LocalizerContainer obj)
        {
            return obj.ToString();
        }

        public static explicit operator LocalizerContainer(string str)
        {
            return Deserialize(str);
        }

        internal class ContainerJsonConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var jObject = new JObject();
                var val = (LocalizerContainer)value;

                if (val.Cultures?.Any() == true)
                    foreach (var culture in val.Cultures)
                        jObject.Add(culture.Culture.TwoLetterISOLanguageName, culture.Value);

                jObject.WriteTo(writer);
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null)
                    return new LocalizerContainer();

                var obj = new LocalizerContainer();
                var jObject = JObject.Load(reader);
                var dictionary = jObject.ToObject<Dictionary<string, string>>();
                if (dictionary == null || !dictionary.Any())
                    return new LocalizerContainer();

                foreach (var (key, mean) in dictionary)
                    obj.Set(key, mean);

                return obj;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(LocalizerContainer);
            }
        }

        /// <summary>
        /// Sets equivalent translation for current language.
        /// </summary>
        /// <param name="localized">An <see cref="string"/> value that represents equivalent translation.</param>
        public void Set(string localized)
        {
            Set(CultureInfo.CurrentCulture, localized);
        }

        /// <summary>
        /// Sets equivalent translation for given culture.
        /// </summary>
        /// <param name="culture">An <see cref="CultureInfo"/> that representing specific culture.</param>
        /// <param name="localized">An <see cref="string"/> value that represents equivalent translation.</param>
        public void Set(CultureInfo culture, string localized)
        {
            var desiredCulture = this.Cultures.FirstOrDefault(x => x.Culture.Equals(culture));
            if (desiredCulture == null)
            {
                this._cultures.Add(new LocalizerCultureNode
                {
                    Culture = culture,
                    Value = localized
                });
            }
            else
            {
                desiredCulture.Value = localized;
            }
        }

        /// <summary>
        /// Sets equivalent translation for given culture.
        /// </summary>
        /// <param name="culture">An <see cref="string"/> that representing specific culture.</param>
        /// <param name="localized">An <see cref="string"/> value that represents equivalent translation.</param>
        public void Set(string culture, string localized)
        {
            var _culture = CultureInfo.GetCultureInfo(culture);
            Set(_culture, localized);
        }

        /// <summary>
        /// Sets equivalent translation for given language.
        /// </summary>
        /// <param name="language">An enumerator constant that representing specific language.</param>
        /// <param name="localized">An <see cref="string"/> value that represents equivalent translation.</param>
        public void Set(Languages language, string localized)
        {
            var description = language.GetDescription();
            Set(description, localized);
        }

        /// <summary>
        /// Returns equivalent translation for given culture.
        /// </summary>
        /// <param name="language">An <see cref="string"/> that represents specified language to get translation value from.</param>
        /// <returns>A <see cref="string"/> value that being contained in given culture.</returns>
        public string this[string language]
        {
            get => Get(language);
            set => Set(language, value);
        }

        /// <summary>
        /// Returns equivalent translation for given culture.
        /// </summary>
        /// <param name="culture">A <see cref="CultureInfo"/> object that should get translation value from.</param>
        /// <returns>A <see cref="string"/> value that being contained in given culture.</returns>
        public string this[CultureInfo culture]
        {
            get => Get(culture);
            set => Set(culture, value);
        }

        /// <summary>
        /// Returns equivalent translation for given culture.
        /// </summary>
        /// <param name="language">An <see cref="string"/> that represents specified language to get translation value from.</param>
        /// <param name="useFallback">A <see cref="bool"/> value that represents return fallback value from other cultures, if current culture doesn't have value.</param>
        /// <returns>A <see cref="string"/> value that being contained in given culture.</returns>
        public string this[string language, bool useFallback]
        {
            get => Get(language, useFallback);
            set => Set(language, value);
        }

        /// <summary>
        /// Returns equivalent translation for given culture.
        /// </summary>
        /// <param name="language">An enumerator constant that represents specified language to get translation value from.</param>
        /// <returns>A <see cref="string"/> value that being contained in given culture.</returns>
        public string this[Languages language]
        {
            get => Get(language);
            set => Set(language, value);
        }

        /// <summary>
        /// Returns equivalent translation for given culture.
        /// </summary>
        /// <param name="language">An enumerator constant that represents specified language to get translation value from.</param>
        /// <param name="useFallback">A <see cref="bool"/> value that represents return fallback value from other cultures, if current culture doesn't have value.</param>
        /// <returns>A <see cref="string"/> value that being contained in given culture.</returns>
        public string this[Languages language, bool useFallback]
        {
            get => Get(language, useFallback);
            set => Set(language, value);
        }

        /// <summary>
        /// Returns equivalent translation for given culture.
        /// </summary>
        /// <param name="culture">A <see cref="CultureInfo"/> object that should get translation value from.</param>
        /// <param name="useFallback">A <see cref="bool"/> value that represents return fallback value from other cultures, if current culture doesn't have value.</param>
        /// <returns>A <see cref="string"/> value that being contained in given culture.</returns>
        public string this[CultureInfo culture, bool useFallback]
        {
            get => Get(culture, useFallback);
            set => Set(culture, value);
        }

        /// <summary>
        /// Returns equivalent translation for given culture.
        /// </summary>
        /// <param name="returnNullIfEmpty">A <see cref="bool"/> value that represents <c>N/A</c> if set true, otherwise make output to be null. * This parameter will be applicable, if <c>useFallback</c> parameter set to false. *</param>
        /// <returns>A <see cref="string"/> value that being contained in given culture.</returns>
        /// <remarks>If <c>CurrentCulture</c> doesn't have value, Fallback will not be shown.</remarks>
        public string ToString(bool returnNullIfEmpty)
        {
            return Get(CultureInfo.CurrentCulture, false, returnNullIfEmpty);
        }

        public override string ToString()
        {
            return Get(CultureInfo.CurrentCulture, true, false);
        }

        /// <summary>
        /// Returns equivalent translation for given culture.
        /// </summary>
        /// <param name="culture">A <see cref="CultureInfo"/> object that should get translation value from.</param>
        /// <param name="useFallback">A <see cref="bool"/> value that represents return fallback value from other cultures, if current culture doesn't have value.</param>
        /// <returns>A <see cref="string"/> value that being contained in given culture.</returns>
        public string Get(string culture, bool useFallback = true)
        {
            var locale = CultureInfo.GetCultureInfo(culture);
            return this.Get(locale, useFallback);
        }

        /// <summary>
        /// Returns equivalent translation for given culture.
        /// </summary>
        /// <param name="language">An enumerator constant that should get translation value from.</param>
        /// <param name="useFallback">A <see cref="bool"/> value that represents return fallback value from other cultures, if current culture doesn't have value.</param>
        /// <returns>A <see cref="string"/> value that being contained in given culture.</returns>
        public string Get(Languages language, bool useFallback = true)
        {
            var locale = language.GetDescription();
            return this.Get(locale, useFallback);
        }

        /// <summary>
        /// Returns equivalent translation for given culture.
        /// </summary>
        /// <param name="culture">A <see cref="CultureInfo"/> object that should get translation value from.</param>
        /// <param name="useFallback">A <see cref="bool"/> value that represents return fallback value from other cultures, if current culture doesn't have value.</param>
        /// <param name="returnNullIfEmpty">A <see cref="bool"/> value that represents <c>N/A</c> if set true, otherwise make output to be null. * This parameter will be applicable, if <c>useFallback</c> parameter set to false. *</param>
        /// <returns>A <see cref="string"/> value that being contained in given culture.</returns>
        public string Get(CultureInfo culture, bool useFallback = true, bool returnNullIfEmpty = true)
        {
            var desiredCulture = this._cultures.Find(x => x.Culture.Equals(culture));
            if (desiredCulture != null)
            {
                var result = desiredCulture.Value;
                if (!string.IsNullOrEmpty(result))
                    return result;
            }

            if (!useFallback)
                return !returnNullIfEmpty ? "N/A" : null;

            var thisCulture = this._cultures.Find(x => x.Culture.Equals(culture));
            var list = this._cultures
                .Except(thisCulture)
                .ToList();

            list = list
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .ToList();
            if (list.Count == 0)
                return "N/A";

            var fallback = list.Count == 1
                ? list[0]
                : list.SelectRandom();
            return fallback.Value;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (!(obj is LocalizerContainer container))
                return false;

            LocalizerContainer source;
            LocalizerContainer destination;
            if (this._cultures.Count >= container._cultures.Count)
            {
                source = this;
                destination = container;
            }
            else
            {
                source = container;
                destination = this;
            }

            var sourceCultures = source._cultures.ToDictionary(x => x.Culture.EnglishName, x => x.Value);
            var destinationCultures = destination._cultures.ToDictionary(x => x.Culture.EnglishName, x => x.Value);
            foreach (var (key, value) in destinationCultures)
            {
                var sourceValue = sourceCultures.First(x => x.Key.Equals(key)).Value;
                if (!string.IsNullOrEmpty(sourceValue))
                {
                    if (string.IsNullOrEmpty(value))
                        return false;

                    if (!sourceValue.Equals(value, StringComparison.InvariantCulture))
                        return false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(value))
                        return false;
                }
            }

            return true;
        }

        public object Clone()
        {
            var newObj = new LocalizerContainer();
            this._cultures.ForEach(localizerCulture => newObj.Set(localizerCulture.Culture, localizerCulture.Value));
            return newObj;
        }

        /// <summary>
        /// Creates a new <see cref="LocalizerContainer"/> that is a copy of the current instance.
        /// </summary>
        /// <param name="container">A <see cref="LocalizerContainer"/> instance to being copied.</param>
        /// <returns>A new <see cref="LocalizerContainer"/> that is a copy of this instance.</returns>
        public static LocalizerContainer Clone(LocalizerContainer container) =>
            (LocalizerContainer)container.Clone();

        /// <summary>
        /// Creates a new <see cref="LocalizerContainer"/> that is a copy of the current instance.
        /// </summary>
        /// <param name="container">A <see cref="LocalizerContainer"/> instance to being copied.</param>
        /// <param name="localized">A <see cref="string"/> value that should be added to current instance for <c>CurrentCulture</c>.</param>
        /// <returns>A new <see cref="LocalizerContainer"/> that is a copy of this instance.</returns>
        public static LocalizerContainer Clone(LocalizerContainer container, string localized) =>
            Clone(container, CultureInfo.CurrentCulture, localized);

        /// <summary>
        /// Creates a new <see cref="LocalizerContainer"/> that is a copy of the current instance.
        /// </summary>
        /// <param name="container">A <see cref="LocalizerContainer"/> instance to being copied.</param>
        /// <param name="culture">A <see cref="CultureInfo"/> object that <c>localized</c> will be added to.</param>
        /// <param name="localized">A <see cref="string"/> value that should be added to current instance for given culture.</param>
        /// <returns>A new <see cref="LocalizerContainer"/> that is a copy of this instance.</returns>
        public static LocalizerContainer Clone(LocalizerContainer container, CultureInfo culture, string localized)
        {
            container ??= new LocalizerContainer();
            var clone = (LocalizerContainer)container.Clone();
            clone.Set(culture, localized);
            return clone;
        }

        public override int GetHashCode()
        {
            var source = Cultures.Select(x => x.Value).ToList();
            return source.Aggregate(0, (current, src) => current ^ src.GetHashCode());
        }
    }
}