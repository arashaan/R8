using System;
using System.Globalization;

using Xunit;

namespace R8.Lib.Test
{
    public class GlobalizationContainerJsonTests
    {
        [Fact]
        public void CallGetLocale()
        {
            // Assets
            var culture = CultureInfo.GetCultureInfo("fa");
            var model = new GlobalizationCollectionJson { [culture] = "آرش" };

            // Acts
            var locale = model.GetLocale(culture);

            // Arranges
            Assert.Equal("آرش", locale);
        }

        [Fact]
        public void CallGetLocale4()
        {
            // Assets
            var culture = CultureInfo.GetCultureInfo("fa");
            var model = new GlobalizationCollectionJson { [culture] = "آرش" };

            // Acts
            var locale = model["fa"];

            // Arranges
            Assert.Equal("آرش", locale);
        }

        [Fact]
        public void CallGetLocale5()
        {
            // Assets
            var culture = CultureInfo.GetCultureInfo("fa");
            var model = new GlobalizationCollectionJson { [culture] = "آرش" };

            // Acts
            var locale = model[culture];

            // Arranges
            Assert.Equal("آرش", locale);
        }

        [Fact]
        public void CallGetLocale3()
        {
            // Assets
            var culture = CultureInfo.GetCultureInfo("fa");
            var model = new GlobalizationCollectionJson { [culture] = "آرش" };

            // Acts
            var locale = model.GetLocale("fa");

            // Arranges
            Assert.Equal("آرش", locale);
        }

        [Fact]
        public void CallGetLocale_NullValueWithFallback()
        {
            // Assets
            var persianCulture = CultureInfo.GetCultureInfo("fa");
            var turkishCulture = CultureInfo.GetCultureInfo("tr");
            var model = new GlobalizationCollectionJson { [persianCulture] = "آرش" };

            // Acts
            var locale = model.GetLocale(turkishCulture);

            // Arranges
            Assert.Equal("آرش", locale);
        }

        [Fact]
        public void CallGetLocale_Empty()
        {
            // Assets
            var persianCulture = CultureInfo.GetCultureInfo("fa");
            var turkishCulture = CultureInfo.GetCultureInfo("tr");
            var model = new GlobalizationCollectionJson();

            // Acts
            var locale = model.GetLocale(turkishCulture);

            // Arranges
            Assert.Equal("N/A", locale);
        }

        [Fact]
        public void CallGetLocale_NullValueWithoutFallback1()
        {
            // Assets
            var persianCulture = CultureInfo.GetCultureInfo("fa");
            var turkishCulture = CultureInfo.GetCultureInfo("tr");
            var model = new GlobalizationCollectionJson { [persianCulture] = "آرش" };

            // Acts
            var locale = model.GetLocale(turkishCulture, false, true);

            // Arranges
            Assert.Equal((string)null, locale);
        }

        [Fact]
        public void CallClone()
        {
            // Assets
            var englishCulture = CultureInfo.GetCultureInfo("en");
            var container1 = new GlobalizationCollectionJson("en", "Arash");
            var container2 = new GlobalizationCollectionJson(englishCulture, "Arash2");

            // Acts
            var cloned = GlobalizationCollectionJson.Clone(container1, englishCulture, "Arash2");

            // Arranges
            Assert.Equal(container2, cloned);
        }

        [Fact]
        public void CallClone2()
        {
            // Assets
            var englishCulture = CultureInfo.GetCultureInfo("en");
            var container1 = new GlobalizationCollectionJson("en", "Arash");
            var container2 = new GlobalizationCollectionJson(englishCulture, "Arash");

            // Acts
            var cloned = GlobalizationCollectionJson.Clone(container1, "Arash");

            // Arranges
            Assert.Equal(container2, cloned);
        }

        [Fact]
        public void CallEquals()
        {
            // Assets
            var container1 = new GlobalizationCollectionJson("en", "Arash");
            var container2 = new GlobalizationCollectionJson("en", "Arash");

            // Acts
            var equality = container1.Equals(container2);

            // Arranges
            Assert.True(equality);
        }

        [Fact]
        public void CallEquals_MismatchInfo3()
        {
            // Assets
            var container1 = new GlobalizationCollectionJson("en", "");
            var container2 = new GlobalizationCollectionJson("en", "Arash");

            // Acts
            var equality = container1.Equals(container2);

            // Arranges
            Assert.False(equality);
        }

        [Fact]
        public void CallEquals_MismatchInfo2()
        {
            // Assets
            var container1 = new GlobalizationCollectionJson("en", "Arash");
            var container2 = new GlobalizationCollectionJson("en", "");

            // Acts
            var equality = container1.Equals(container2);

            // Arranges
            Assert.False(equality);
        }

        [Fact]
        public void CallEquals_MismatchInfo1()
        {
            // Assets
            var container1 = new GlobalizationCollectionJson("en", "Arash");
            var container2 = new GlobalizationCollectionJson("en", "Arash2");

            // Acts
            var equality = container1.Equals(container2);

            // Arranges
            Assert.False(equality);
        }

        [Fact]
        public void CallEqual_MismatchType()
        {
            // Assets
            var englishCulture = CultureInfo.GetCultureInfo("en");
            var container1 = new GlobalizationCollectionJson("en", "Arash");
            var container2 = string.Empty;

            // Arranges
            Assert.Throws<Exception>(() => container1.Equals(container2));
        }

        [Fact]
        public void CallGetLocale2()
        {
            // Assets
            var persianCulture = CultureInfo.GetCultureInfo("fa");
            var turkishCulture = CultureInfo.GetCultureInfo("tr");
            var model = new GlobalizationCollectionJson
            {
                [persianCulture] = "آرش",
                [turkishCulture] = "arash"
            };

            // Acts
            var locale = model.GetLocale(turkishCulture, false, false);

            // Arranges
            Assert.Equal("arash", locale);
        }

        [Fact]
        public void CallGetLocale_NullValueWithoutFallback2()
        {
            // Assets
            var persianCulture = CultureInfo.GetCultureInfo("fa");
            var turkishCulture = CultureInfo.GetCultureInfo("tr");
            var model = new GlobalizationCollectionJson { [persianCulture] = "آرش" };

            // Acts
            var locale = model.GetLocale(turkishCulture, false, false);

            // Arranges
            Assert.Equal("N/A", locale);
        }
    }
}