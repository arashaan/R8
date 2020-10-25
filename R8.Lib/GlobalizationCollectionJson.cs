using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace R8.Lib
{
    public class GlobalizationCollectionJson
    {
        [JsonProperty("fa")]
        [DefaultValue("")]
        public string Persian { get; set; }

        [JsonProperty("tr")]
        [DefaultValue("")]
        public string Turkish { get; set; }

        [JsonProperty("en")]
        [DefaultValue("")]
        public string English { get; set; }

        public GlobalizationCollectionJson(CultureInfo culture, string localized) : this()
        {
            Set(culture, localized);
        }

        public GlobalizationCollectionJson(string localized) : this(CultureInfo.CurrentCulture, localized)
        {
        }

        public GlobalizationCollectionJson(string language, string localized) : this(CultureInfo.GetCultureInfo(language), localized)
        {
        }

        public GlobalizationCollectionJson()
        {
        }

        public static GlobalizationCollectionJson Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                return Deserialize("{}");

            try
            {
                var obj = JsonConvert.DeserializeObject<GlobalizationCollectionJson>(json);
                return obj ?? new GlobalizationCollectionJson();
            }
            catch
            {
                return new GlobalizationCollectionJson();
            }
        }

        public bool HasValue()
        {
            return GetCultures().Any(x => !string.IsNullOrEmpty(x.GetValue(this)?.ToString()));
        }

        private static JsonSerializerSettings JsonSettings
        {
            get
            {
                var settings = JsonSettingsExtensions.JsonNetSettings;
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                return settings;
            }
        }

        public string Serialize()
        {
            if (!HasValue())
                return null;

            try
            {
                var json = JsonConvert.SerializeObject(this, JsonSettings);
                return json;
            }
            catch
            {
                return null;
            }
        }

        public static implicit operator string(GlobalizationCollectionJson obj)
        {
            return obj.Serialize();
        }

        public static explicit operator GlobalizationCollectionJson(string str)
        {
            return Deserialize(str);
        }

        public void Set(string localized)
        {
            Set(CultureInfo.CurrentCulture, localized);
        }

        public void Set(CultureInfo culture, string localized)
        {
            var desiredCulture = this.FirstOrDefault(x => x.Equals(culture));
            desiredCulture.SetValue(this, localized);
        }

        public void Set(string language, string localized)
        {
            var culture = CultureInfo.GetCultureInfo(language);
            Set(culture, localized);
        }

        public string this[string language, bool useFallback = true]
        {
            get => GetLocale(language, useFallback);
            set => Set(language, value);
        }

        public string this[CultureInfo culture, bool useFallback = true]
        {
            get => GetLocale(culture, useFallback);
            set => Set(culture, value);
        }

        public string GetLocale(string culture, bool useFallback = true)
        {
            var locale = CultureInfo.GetCultureInfo(culture);
            return this.GetLocale(locale, useFallback);
        }

        public override string ToString()
        {
            var localized = GetLocale(CultureInfo.CurrentCulture, true, false);
            return localized;
        }

        public static GlobalizationCollectionJson Clone(GlobalizationCollectionJson container, string localized)
        {
            return Clone(container, CultureInfo.CurrentCulture, localized);
        }

        public static GlobalizationCollectionJson Clone(GlobalizationCollectionJson container, CultureInfo culture, string localized)
        {
            container ??= new GlobalizationCollectionJson();
            var newObj = new GlobalizationCollectionJson();
            foreach (var propertyInfo in container.GetCultures())
            {
                propertyInfo.SetValue(newObj,
                    culture.Equals(FindCultureFromProperty(propertyInfo))
                        ? localized
                        : propertyInfo.GetValue(container));
            }

            return newObj;
        }

        public string ToString(bool returnNullIfEmpty)
        {
            var localized = GetLocale(CultureInfo.CurrentCulture, false, returnNullIfEmpty);
            return localized;
        }

        private List<PropertyInfo> GetCultures()
        {
            var props = this.GetType().GetPublicProperties()
                .Where(x => x.GetCustomAttribute<JsonPropertyAttribute>() != null).ToList();
            return props;
        }

        private static CultureInfo FindCultureFromProperty(MemberInfo propertyInfo)
        {
            var lang = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? string.Empty;
            return string.IsNullOrEmpty(lang)
                ? null
                : CultureInfo.GetCultureInfo(lang);
        }

        private PropertyInfo FirstOrDefault(Func<CultureInfo, bool> predicate)
        {
            var cultures = GetCultures();
            var culture = cultures.Find(x => predicate.Invoke(FindCultureFromProperty(x)));
            return culture;
        }

        private PropertyInfo Get(string cultureName)
        {
            return this.FirstOrDefault(x =>
                x.GetTwoLetterCulture().Equals(cultureName, StringComparison.InvariantCultureIgnoreCase));
        }

        public string Get(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                return null;

            var obj = propertyInfo.GetValue(this);
            return obj?.ToString();
        }

        public string GetLocale(CultureInfo culture, bool useFallback = true, bool returnNullIfEmpty = true)
        {
            var desiredCulture = this.FirstOrDefault(x => x.Equals(culture));
            var result = Get(desiredCulture);
            if (!string.IsNullOrEmpty(result))
                return result;

            if (!useFallback)
                return !returnNullIfEmpty ? "N/A" : null;

            var thisCulture = Get(culture.EnglishName);
            var list = this.GetCultures()
                .Except(thisCulture)
                .ToList();

            list = list
                .Where(x => !string.IsNullOrEmpty(x.GetValue(this)?.ToString()))
                .ToList();
            if (list.Count == 0)
                return "N/A";

            var fallback = list.Count == 1
                ? list[0]
                : list.SelectRandom();
            return Get(fallback);
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is GlobalizationCollectionJson container))
                throw new Exception($"{nameof(obj)} must be in type of {nameof(GlobalizationCollectionJson)}");

            var source = GetCultures().ToDictionary(x => x.Name, x => x.GetValue(this)?.ToString());
            var destination = container.GetCultures().ToDictionary(x => x.Name, x => x.GetValue(container)?.ToString());
            foreach (var (key, value) in destination)
            {
                var sourceValue = source.First(x => x.Key.Equals(key)).Value;
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
            var source = GetCultures().Select(x => x.GetValue(this)?.ToString()).ToList();
            var hash = source.Aggregate(0, (current, src) => current ^ src.GetHashCode());
            return hash;
        }
    }
}