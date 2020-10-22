using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace R8.Lib
{
    public class GlobalizationCulture : CultureInfo
    {
        public int Order { get; set; }

        public GlobalizationCulture(int culture) : base(culture)
        {
        }

        public GlobalizationCulture(int culture, bool useUserOverride) : base(culture, useUserOverride)
        {
        }

        public GlobalizationCulture(string name) : base(name)
        {
        }

        public GlobalizationCulture(string name, bool useUserOverride) : base(name, useUserOverride)
        {
        }
    }

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

        //public HtmlString GetHtmlString(CultureInfo culture = null)
        //{
        //    var text = GetLocale(culture ?? CultureInfo.CurrentCulture, false);
        //    var html = new HtmlString(HttpUtility.HtmlDecode(text));
        //    return html;
        //}

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

        private string GetLocale(string culture, bool useFallback = true)
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
            var culture = cultures.FirstOrDefault(x => predicate.Invoke(FindCultureFromProperty(x)));
            return culture;
        }

        private PropertyInfo Get(string cultureName)
        {
            return this.FirstOrDefault(x =>
                x.GetTwoLetterCulture().Equals(cultureName, StringComparison.InvariantCultureIgnoreCase));
        }

        public string Get(PropertyInfo propertyInfo)
        {
            return (string)propertyInfo.GetValue(this);
        }

        private string GetLocale(CultureInfo culture, bool useFallback = true, bool returnNullIfEmpty = true)
        {
            var desiredCulture = this.FirstOrDefault(x => x.Equals(culture));
            var result = Get(desiredCulture);
            if (!string.IsNullOrEmpty(result))
                return result;

            if (!useFallback)
                return !returnNullIfEmpty ? "N/A" : null;

            var thisCulture = Get(culture.EnglishName);
            var list = this.GetCultures();
            if (thisCulture != null)
            {
                var exceptList = new List<PropertyInfo> { thisCulture };
                list = list.Except(exceptList).ToList();
            }

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
    }
}