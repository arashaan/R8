using Newtonsoft.Json;

using R8.Lib.Enums;
using R8.Lib.Localization;

using System;
using System.Globalization;

using Xunit;

namespace R8.Lib.Test
{
    public class LocalizerContainerTests
    {
        [Fact]
        public void CallGetLocale()
        {
            // Assets
            var culture = CultureInfo.GetCultureInfo("fa");
            var model = new LocalizerContainer { [culture] = "آرش" };

            // Acts
            var locale = model.Get(culture);

            // Arranges
            Assert.Equal("آرش", locale);
        }

        [Fact]
        public void CallGetLocale4()
        {
            // Assets
            var culture = CultureInfo.GetCultureInfo("fa");
            var model = new LocalizerContainer { [culture] = "آرش" };

            // Acts
            var locale = model["fa"];

            // Arranges
            Assert.Equal("آرش", locale);
        }

        [Fact]
        public void CallSetLocale()
        {
            // Assets
            var model = new LocalizerContainer();

            // Acts
            model[Languages.Persian] = "آرش";

            // Arranges
            Assert.Equal("آرش", model[Languages.Persian]);
        }

        [Fact]
        public void CallGetLocale5()
        {
            // Assets
            var culture = CultureInfo.GetCultureInfo("fa");
            var model = new LocalizerContainer { [culture] = "آرش" };

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
            var model = new LocalizerContainer { [culture] = "آرش" };

            // Acts
            var locale = model.Get("fa");

            // Arranges
            Assert.Equal("آرش", locale);
        }

        [Fact]
        public void CallGetLocale_NullValueWithFallback()
        {
            // Assets
            var persianCulture = CultureInfo.GetCultureInfo("fa");
            var turkishCulture = CultureInfo.GetCultureInfo("tr");
            var model = new LocalizerContainer { [persianCulture] = "آرش" };

            // Acts
            var locale = model.Get(turkishCulture);

            // Arranges
            Assert.Equal("آرش", locale);
        }

        [Fact]
        public void CallGetLocale_Empty()
        {
            // Assets
            var persianCulture = CultureInfo.GetCultureInfo("fa");
            var turkishCulture = CultureInfo.GetCultureInfo("tr");
            var model = new LocalizerContainer();

            // Acts
            var locale = model.Get(turkishCulture);

            // Arranges
            Assert.Equal("N/A", locale);
        }

        [Fact]
        public void CallGetLocale_NullValueWithoutFallback1()
        {
            // Assets
            var persianCulture = CultureInfo.GetCultureInfo("fa");
            var turkishCulture = CultureInfo.GetCultureInfo("tr");
            var model = new LocalizerContainer { [persianCulture] = "آرش" };

            // Acts
            var locale = model.Get(turkishCulture, false, true);

            // Arranges
            Assert.Equal((string)null, locale);
        }

        [Fact]
        public void CallClone()
        {
            // Assets
            var englishCulture = CultureInfo.GetCultureInfo("en");
            var container1 = new LocalizerContainer("en", "Arash");
            var container2 = new LocalizerContainer(englishCulture, "Arash2");

            // Acts
            var cloned = LocalizerContainer.Clone(container1, englishCulture, "Arash2");

            // Arranges
            Assert.Equal(container2, cloned);
        }

        [Fact]
        public void CallClone2()
        {
            // Assets
            var englishCulture = CultureInfo.GetCultureInfo("en");
            var container1 = new LocalizerContainer("en", "Arash");
            var container2 = new LocalizerContainer(englishCulture, "Arash");

            // Acts
            var cloned = LocalizerContainer.Clone(container1, "Arash");

            // Arranges
            Assert.Equal(container2, cloned);
        }

        [Fact]
        public void CallEquals()
        {
            // Assets
            var container1 = new LocalizerContainer("en", "Arash");
            var container2 = new LocalizerContainer("en", "Arash");

            // Acts
            var equality = container1.Equals(container2);

            // Arranges
            Assert.True(equality);
        }

        [Fact]
        public void CallEquals_MismatchInfo3()
        {
            // Assets
            var container1 = new LocalizerContainer("en", "");
            var container2 = new LocalizerContainer("en", "Arash");

            // Acts
            var equality = container1.Equals(container2);

            // Arranges
            Assert.False(equality);
        }

        [Fact]
        public void CallEquals_MismatchInfo2()
        {
            // Assets
            var container1 = new LocalizerContainer("en", "Arash");
            var container2 = new LocalizerContainer("en", "");

            // Acts
            var equality = container1.Equals(container2);

            // Arranges
            Assert.False(equality);
        }

        [Fact]
        public void CallEquals_MismatchInfo1()
        {
            // Assets
            var container1 = new LocalizerContainer("en", "Arash");
            var container2 = new LocalizerContainer("en", "Arash2");

            // Acts
            var equality = container1.Equals(container2);

            // Arranges
            Assert.False(equality);
        }

        [Fact]
        public void CallEqual_NullEquivalent()
        {
            // Assets
            var container1 = new LocalizerContainer("en", "Arash");
            var container2 = (LocalizerContainer)null;

            // Arranges
            Assert.NotEqual(container1, container2);
        }

        [Fact]
        public void CallEqual_NullEquivalent2()
        {
            // Assets
            var container1 = new LocalizerContainer("en", "Arash");
            var container2 = (LocalizerContainer)null;

            // Arranges
            Assert.False(container1.Equals(container2));
        }

        [Fact]
        public void CallEqual_MismatchType()
        {
            // Assets
            var container1 = new LocalizerContainer("en", "Arash");
            var container2 = true;

            // Arranges
            Assert.Throws<Exception>(() => container1.Equals(container2));
        }

        [Fact]
        public void CallSerialize_Null()
        {
            // Assets
            var container1 = new LocalizerContainer();

            // Acts
            var serialized = container1.Serialize();

            // Arranges
            Assert.Null(serialized);
        }

        [Fact]
        public void CallDeserialize_Null()
        {
            // Assets
            var container1 = new LocalizerContainer();
            var json = (string)null;

            // Acts
            var deSerialized = LocalizerContainer.Deserialize(json);

            // Arranges
            Assert.Equal(deSerialized, container1);
        }

        [Fact]
        public void CallDeserialize_DirectCast()
        {
            // Assets
            var container1 = new LocalizerContainer
            {
                ["en"] = "Arash",
                ["fa"] = "آرش"
            };
            var json = container1.Serialize();

            // Acts
            var deSerialized = (LocalizerContainer)json;

            // Arranges
            Assert.Equal(deSerialized, container1);
        }

        [Fact]
        public void CallSerialize_DirectCast()
        {
            // Assets
            var container1 = new LocalizerContainer
            {
                ["en"] = "Arash",
                ["fa"] = "آرش"
            };
            var shouldBeSerialized = container1.ToString();

            // Acts
            var serialized = (string)container1;

            // Arranges
            Assert.Equal(shouldBeSerialized, serialized);
        }

        [Fact]
        public void CallSerialize()
        {
            // Assets
            var container1 = new LocalizerContainer
            {
                ["en"] = "Arash",
                ["fa"] = "آرش"
            };
            var shouldBeSerialized = JsonConvert.SerializeObject(container1);

            // Acts
            var serialized = container1.Serialize();

            // Arranges
            Assert.Equal(shouldBeSerialized, serialized);
        }

        [Fact]
        public void CallSerialize_PlainText()
        {
            // Assets
            var container1 = new LocalizerContainer
            {
                ["en"] = "Arash",
            };
            var shouldBeSerialized = "{\"en\":\"Arash\"}";

            // Acts
            var serialized = container1.Serialize();

            // Arranges
            Assert.Equal(shouldBeSerialized, serialized);
        }

        [Fact]
        public void CallSerialize_PlainText2()
        {
            // Assets
            var container1 = new LocalizerContainer
            {
                ["en"] = "Arash",
                ["fa"] = "آرش"
            };
            var shouldBeSerialized = "{\"en\":\"Arash\",\"fa\":\"آرش\"}";

            // Acts
            var serialized = container1.Serialize();

            // Arranges
            Assert.Equal(shouldBeSerialized, serialized);
        }

        [Fact]
        public void CallDeserialize()
        {
            // Assets
            var serialized = @"{""fa"":""خدمات خانوادگی"",""tr"":""Aile Hizmetleri"",""en"":""Family Services""}";
            var shouldBe = new LocalizerContainer
            {
                ["en"] = "Family Services",
                ["fa"] = "خدمات خانوادگی",
                ["tr"] = "Aile Hizmetleri"
            };

            // Acts
            var deserialized = LocalizerContainer.Deserialize(serialized);

            // Arranges
            Assert.Equal(shouldBe, deserialized);
        }

        [Fact]
        public void CallSet2()
        {
            // Assets
            var container1 = new LocalizerContainer { ["en"] = "Arash" };

            // Arranges
            Assert.Equal("Arash", container1[Languages.English]);
        }

        [Fact]
        public void CallSet()
        {
            // Assets
            var container1 = new LocalizerContainer { ["en"] = "Arash" };

            // Arranges
            Assert.Equal("Arash", container1["en"]);
        }

        [Fact]
        public void CallCtor()
        {
            // Assets
            var container1 = new LocalizerContainer("Arash");

            // Arranges
            Assert.Equal("Arash", container1[CultureInfo.CurrentCulture]);
        }

        [Fact]
        public void CallEqual_Set2()
        {
            // Assets
            var container1 = new LocalizerContainer();
            container1.Set("Arash");

            // Arranges
            Assert.Equal("Arash", container1[CultureInfo.CurrentCulture]);
        }

        [Fact]
        public void CallHasValue_Null()
        {
            // Assets
            var container1 = new LocalizerContainer();

            // Arranges
            Assert.False(container1.HasValue());
        }

        [Fact]
        public void CallHasValue()
        {
            // Assets
            var container1 = new LocalizerContainer();
            container1.Set("Arash");

            // Arranges
            Assert.True(container1.HasValue());
        }

        [Fact]
        public void CallGetLocale2()
        {
            // Assets
            var persianCulture = CultureInfo.GetCultureInfo("fa");
            var turkishCulture = CultureInfo.GetCultureInfo("tr");
            var model = new LocalizerContainer
            {
                [persianCulture] = "آرش",
                [turkishCulture] = "arash"
            };

            // Acts
            var locale = model.Get(turkishCulture, false, false);

            // Arranges
            Assert.Equal("arash", locale);
        }

        [Fact]
        public void CallGetLocale_NullValueWithoutFallback2()
        {
            // Assets
            var persianCulture = CultureInfo.GetCultureInfo("fa");
            var turkishCulture = CultureInfo.GetCultureInfo("tr");
            var model = new LocalizerContainer { [persianCulture] = "آرش" };

            // Acts
            var locale = model.Get(turkishCulture, false, false);

            // Arranges
            Assert.Equal("N/A", locale);
        }
    }
}