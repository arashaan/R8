using System.Globalization;
using Newtonsoft.Json;
using R8.Lib.Enums;
using R8.Lib.Localization;
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
            Assert.False(container1.Equals(container2));
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
            Assert.False(container1.HasValue);
        }

        [Fact]
        public void CallHasValue()
        {
            // Assets
            var container1 = new LocalizerContainer();
            container1.Set("Arash");

            // Arranges
            Assert.True(container1.HasValue);
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
        public void CallGetLocale_Text()
        {
            // Assets
            var turkishCulture = CultureInfo.GetCultureInfo("tr");
            var model = new LocalizerContainer { [turkishCulture] = "Her türlü kopya siber suçlar Yasası'na tabidir ve yasal kovuşturmaya yol açacaktır. Tüm hakları Saklıdır." };

            // Arranges
            Assert.Equal(LocalizerValueType.Text, model.ValueType);
        }

        [Fact]
        public void CallGetLocale_FormattableType()
        {
            // Assets
            var turkishCulture = CultureInfo.GetCultureInfo("tr");
            var model = new LocalizerContainer { [turkishCulture] = "Telif Hakkı © {0} EKOHOS Kurumsal" };

            // Arranges
            Assert.Equal(LocalizerValueType.FormattableText, model.ValueType);
        }

        [Fact]
        public void CallGetLocale_HtmlType()
        {
            // Assets
            var turkishCulture = CultureInfo.GetCultureInfo("tr");
            var model = new LocalizerContainer { [turkishCulture] = "Daha iyi bir kullanıcı deneyimi sağlamak, site trafiğini analiz etmek ve hedefli reklamlar sunmak için çerezleri kullanıyoruz. Bu web sitesini kullanmaya devam ederek, <0></0> uygun olarak çerezlerin kullanılmasına izin vermiş olursunuz." };

            // Arranges
            Assert.Equal(LocalizerValueType.Html, model.ValueType);
        }

        [Fact]
        public void CallGetLocale_HtmlType_Complicated()
        {
            // Assets
            var turkishCulture = CultureInfo.GetCultureInfo("tr");
            var model = new LocalizerContainer { [turkishCulture] = "<0>Emlak Reklam Kuralları</0><1><2>ECOHOS'un reklam yayınlama sürecini kolaylaştırmak ve hızlandırmak için bazı durumlarda reklam metninizi veya ayrıntılarınızı kurallara göre ayrıntılı olarak değiştirebileceğini İzin veriyorum.</2><2>Reklamın sahibi, kaydettiği reklamın içeriğinin güncel ve doğru bilgiler içermesini ve söz konusu içeriğin reklamın hüküm ve koşulları için geçerli olmasını sağlamalıdır. Reklamın içeriğiyle ilgili tüm sorumluluklar reklamın sahibine aittir. </2><2>Reklam sahibinin, yabancı müşterinin iç ve dış özelliklerini ve mülkün çevresindeki ortamı daha iyi anlamak için diğer ayrıntıları doğru ve eksiksiz olarak kaydetmesi gerekir. Bunu yapmak, reklamın emlak arama sayfasında % 80'e kadar daha iyi görüntülenme olasılığını kesinlikle artıracaktır. </2><2>Reklam fotoğrafları yalnızca en az 600 x 600 piksel boyutunda yuklemeniz gerekmektedir. Logo, telefon numarası, kartvizit resmi, resimlerde gorunmesi kesinlikle yasaktir. Diger web sitesine yüklenen fotoğraflarda ot...nen ve güncellenen www.ecohos.com'a bir reklam gönderilmesiyle takip edileceğini kabul ve beyan edir. </2><2>Satılan veya eski reklamlar, sahibinin tarafından arşivlenmelidir. Ayrıca, reklamın yayınlanmasından 30 gün sonra, söz konusu mülkün satışa hazır olduğunu belirten, web sitesinin mülk bilgilerini güncellemek için reklamın sahibine bir e-posta gönderilecektir. </2><2>Bu kurallara uymayan reklam verenlerin reklam içeriğinin bir kısmı veya tamamı kaldırılmış ve kara listeye alınmış olabilecektir ve üyelik sözleşmelerinin ECOHOS tarafından tek taraflı olarak feshedilmesi mümkündür.</2></1><0>Kara listeye girmemeye dikkat edin:</0><3>Reklam yayınlama kurallarına uymayan kullanıcılar uyarı olarak kara listeye alınabilir. Kara listedeki kullanıcıların reklamları yayınlanmaya devam eder, ancak reklam arama öncelik listesindeki en düşük puanla gösterilir. ECOHOS, kendi takdirine bağlı olarak, ilgili reklamları Web Sitesinden kaldırabilir veya geçici olarak listeden askıya alabilir.</3>" };

            // Arranges
            Assert.Equal(LocalizerValueType.Html, model.ValueType);
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