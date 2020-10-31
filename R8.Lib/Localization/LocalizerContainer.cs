﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using R8.Lib.Enums;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace R8.Lib.Localization
{
    [JsonConverter(typeof(ContainerJsonConverter))]
    public class LocalizerContainer : ICloneable
    {
        public LocalizerContainer(CultureInfo culture, string localized) : this()
        {
            Set(culture, localized);
        }

        public LocalizerContainer(string localized) : this(CultureInfo.CurrentCulture, localized)
        {
        }

        public LocalizerContainer(string language, string localized) : this(CultureInfo.GetCultureInfo(language), localized)
        {
        }

        public LocalizerContainer()
        {
            _cultures = new List<LocalizerCulture>();
        }

        public IReadOnlyCollection<LocalizerCulture> Cultures => _cultures;
        private readonly List<LocalizerCulture> _cultures;

        public static LocalizerContainer Deserialize(string json)
        {
            return !string.IsNullOrEmpty(json)
                ? JsonConvert.DeserializeObject<LocalizerContainer>(json)
                : new LocalizerContainer();
        }

        public bool HasValue()
        {
            return _cultures.Any(x => !string.IsNullOrEmpty(x.Value));
        }

        public string Serialize()
        {
            var settings = JsonSettingsExtensions.JsonNetSettings;
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            return !HasValue()
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
                        jObject.Add(culture.Culture.GetTwoLetterCulture(), culture.Value);

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

        public void Set(string localized)
        {
            Set(CultureInfo.CurrentCulture, localized);
        }

        public void Set(CultureInfo culture, string localized)
        {
            var desiredCulture = this.Cultures.FirstOrDefault(x => x.Culture.Equals(culture));
            if (desiredCulture == null)
            {
                this._cultures.Add(new LocalizerCulture
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

        public void Set(string culture, string localized)
        {
            var _culture = CultureInfo.GetCultureInfo(culture);
            Set(_culture, localized);
        }

        public void Set(Languages language, string localized)
        {
            var description = language.GetDescription();
            Set(description, localized);
        }

        public string this[string language]
        {
            get => Get(language);
            set => Set(language, value);
        }

        public string this[CultureInfo culture]
        {
            get => Get(culture);
            set => Set(culture, value);
        }

        public string this[string language, bool useFallback]
        {
            get => Get(language, useFallback);
            set => Set(language, value);
        }

        public string this[Languages language]
        {
            get => Get(language);
            set => Set(language, value);
        }

        public string this[Languages language, bool useFallback]
        {
            get => Get(language, useFallback);
            set => Set(language, value);
        }

        public string this[CultureInfo culture, bool useFallback]
        {
            get => Get(culture, useFallback);
            set => Set(culture, value);
        }

        public object Clone()
        {
            var newObj = new LocalizerContainer();
            this._cultures.ForEach(localizerCulture => newObj.Set(localizerCulture.Culture, localizerCulture.Value));
            return newObj;
        }

        public static LocalizerContainer Clone(LocalizerContainer container, string localized)
        {
            return Clone(container, CultureInfo.CurrentCulture, localized);
        }

        public static LocalizerContainer Clone(LocalizerContainer container, CultureInfo culture, string localized)
        {
            container ??= new LocalizerContainer();
            var clone = (LocalizerContainer)container.Clone();
            clone.Set(culture, localized);
            return clone;
        }

        /// <summary>
        /// Returns a string that represents Final Meaning without Fallback
        /// </summary>
        /// <param name="returnNullIfEmpty">Determine return null of N/A</param>
        public string ToString(bool returnNullIfEmpty)
        {
            return Get(CultureInfo.CurrentCulture, false, returnNullIfEmpty);
        }

        /// <summary>
        /// Returns a string that represents Final Meaning with Fallback
        /// </summary>
        public override string ToString()
        {
            return Get(CultureInfo.CurrentCulture, true, false);
        }

        public string Get(string culture, bool useFallback = true)
        {
            var locale = CultureInfo.GetCultureInfo(culture);
            return this.Get(locale, useFallback);
        }

        public string Get(Languages language, bool useFallback = true)
        {
            var locale = language.GetDescription();
            return this.Get(locale, useFallback);
        }

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
                throw new Exception($"{nameof(obj)} must be in type of {nameof(LocalizerContainer)}");

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

        public override int GetHashCode()
        {
            var source = Cultures.Select(x => x.Value).ToList();
            return source.Aggregate(0, (current, src) => current ^ src.GetHashCode());
        }
    }
}