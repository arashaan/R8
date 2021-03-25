using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.DependencyInjection;

using R8.AspNetCore.Localization;
using R8.AspNetCore.TagBuilders;
using R8.AspNetCore.Test.TestServerSimulation;
using R8.Lib.Localization;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace R8.AspNetCore.Test.ILocalizer
{
    public class TestsWithTestServer : TestWebServer
    {
        private readonly ITestOutputHelper _output;
        private readonly Lib.Localization.ILocalizer _localizer;

        public TestsWithTestServer(TestServerFixture fixture, ITestOutputHelper output) : base(fixture, output)
        {
            _output = output;
            using var scope = Fixture.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            _localizer = serviceProvider.GetService<Lib.Localization.ILocalizer>();
            _localizer.RefreshAsync().GetAwaiter().GetResult();
            serviceProvider.UseResponse();
        }

        [Fact]
        public void CallResponse_ToString()
        {
            // Assets
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");

            // Act
            var response = new FakeResponse(Flags.Success);
            response.SetLocalizer(_localizer);
            const string expected = "عملیات به موفقیت انجام شد";

            // Arrange
            Assert.Equal(expected, response.Message);
        }

        [Fact]
        public async Task CallTryGetValue_CorrectLanguageButCouldntFindKey()
        {
            // Assets
            var key = "AppName2";
            var culture = CultureInfo.GetCultureInfo("tr");

            // Act
            await _localizer.RefreshAsync();
            var localized = _localizer[key, culture];

            // Arrange
            Assert.Equal(key, localized);
        }

        [Fact]
        public async Task CallGetter_ShouldNotBeEqualToKey()
        {
            // Assets
            var key = "AppName";

            // Act
            await _localizer.RefreshAsync();
            var translation = _localizer[key][Constants.DefaultCulture, false];

            // Arrange
            Assert.NotEqual(translation, key);
        }

        [Fact]
        public async Task CallGetter_SpecificCultureDefault()
        {
            // Assets
            var key = "AppName";

            // Act
            await _localizer.RefreshAsync();
            var translation = _localizer[key][Constants.DefaultCulture, false];

            // Arrange
            Assert.Equal("EKOHOS", translation);
        }

        [Fact]
        public async Task CallGetter_SpecificCultureFarsi2()
        {
            // Assets
            var key = "AppName";
            var culture = CultureInfo.GetCultureInfo("fa");

            // Act
            await _localizer.RefreshAsync();
            var translation = _localizer[key, culture];

            // Arrange
            Assert.Equal("هلدینگ اکوهوس", translation);
        }

        [Fact]
        public async Task CallGetter_SpecificCultureFarsi()
        {
            // Assets
            var key = "AppName";
            var culture = CultureInfo.GetCultureInfo("fa");

            // Act
            await _localizer.RefreshAsync();
            var translation = _localizer[key][culture, false];

            // Arrange
            Assert.Equal("هلدینگ اکوهوس", translation);
        }

        [Theory]
        [InlineData("fa")]
        [InlineData("en")]
        [InlineData("tr")]
        public async Task CallGetter(string lang)
        {
            // Assets
            var key = "AppName";
            var culture = CultureInfo.GetCultureInfo(lang);

            // Act
            await _localizer.RefreshAsync();
            var translation = _localizer[key][culture, false];

            // Arrange
            Assert.NotNull(translation);
        }

        [Fact]
        public async Task CallGetter_NullArg()
        {
            // Act
            await _localizer.RefreshAsync();
            var translation = _localizer[(string)null];

            // Arrange
            Assert.Null(translation);
        }

        [Fact]
        public async Task CallGetter_WithCulture_NullKey()
        {
            // Assets
            var key = (string)null;

            // Act
            await _localizer.RefreshAsync();
            var translation = _localizer[key, Constants.DefaultCulture];

            // Arrange
            Assert.Null(translation);
        }

        [Fact]
        public async Task CallGetter_SpecificCultureEnglish()
        {
            // Assets
            var key = "AppName";

            // Act
            await _localizer.RefreshAsync();
            var translation = _localizer[key, CultureInfo.GetCultureInfo("en")];

            // Arrange
            Assert.Equal("ECOHOS Holding", translation);
        }

        [Fact]
        public async Task CallGetter_WithDefaultCulture()
        {
            // Assets
            var key = "AppName";

            // Act
            await _localizer.RefreshAsync();
            var translation = _localizer[key, Constants.DefaultCulture];

            // Arrange
            Assert.Equal("EKOHOS", translation);
        }

        [Fact]
        public async Task CallTryGetValue_SpecificCulture()
        {
            // Assets
            var key = "AppName";
            var culture = CultureInfo.GetCultureInfo("tr");

            // Act
            await _localizer.RefreshAsync();
            var localized = _localizer[key, culture];

            // Arrange
            Assert.NotNull(localized);
        }

        [Fact]
        public async Task CallLocalizerHtml()
        {
            // Assets
            var key = "CookieBanner";
            var tags = new List<Func<string, IHtmlContent>>();
            tags.Add(str => new HtmlString("<a class=\"inline-link\" asp-page=\"typeof(PrivacyModel).GetPagePath()\">" +
                                           _localizer["PrivacyPolicy"] + "</a>"));

            // Acts
            CultureInfo.CurrentCulture = Constants.DefaultCulture;
            await _localizer.RefreshAsync();
            var act = _localizer.Html(key, tags.ToArray());
            var html = act.GetString();

            var expected =
                "Daha iyi bir kullanıcı deneyimi sağlamak, site trafiğini analiz etmek ve hedefli reklamlar sunmak için çerezleri kullanıyoruz. Bu web sitesini kullanmaya devam ederek, <a asp-page='typeof(PrivacyModel).GetPagePath()' class='inline-link'>Gizlilik Politikası</a> uygun olarak çerezlerin kullanılmasına izin vermiş olursunuz.";

            // Arrange
            Assert.Equal(expected, html);
        }

        [Fact]
        public async Task CallLocalizerFormat_NullTags2()
        {
            // Assets
            var key = "Copyright";

            // Acts
            CultureInfo.CurrentCulture = Constants.DefaultCulture;
            await _localizer.RefreshAsync();

            // Arrange
            Assert.Throws<NullReferenceException>(() => _localizer.Format(key, (List<string>)null));
        }

        [Fact]
        public async Task CallLocalizerFormat_NullKey()
        {
            // Assets
            var key = (string)null;
            var tags = new List<string>();
            var year = DateTime.Now.Year.ToString();
            tags.Add(year);

            // Acts
            CultureInfo.CurrentCulture = Constants.DefaultCulture;
            await _localizer.RefreshAsync();

            // Arrange
            Assert.Throws<ArgumentNullException>(() => _localizer.Format(key, tags.ToArray()));
        }

        [Fact]
        public void CallLocalizerFormat_NullLocalizer()
        {
            // Arrange
            Assert.Throws<ArgumentNullException>(() => LocalizerExtensions.Format(null, null, (List<string>)null));
        }

        [Fact]
        public async Task CallLocalizerFormat()
        {
            // Assets
            var key = "Copyright";
            var tags = new List<string>();
            var year = DateTime.Now.Year.ToString();
            tags.Add(year);

            // Acts
            CultureInfo.CurrentCulture = Constants.DefaultCulture;
            await _localizer.RefreshAsync();
            var act = _localizer.Format(key, tags.ToArray());
            var html = act.GetString();

            var expected = $"Telif Hakkı © {DateTime.Now.Year} EKOHOS Kurumsal";

            // Arrange
            Assert.Equal(expected, html);
        }

        [Fact]
        public void CallLocalizerFormat_Html_NullKey()
        {
            // Assets
            var key = (string)null;
            var tags = new List<Func<string, IHtmlContent>>();
            var year = DateTime.Now.Year.ToString();
            tags.Add(str => new HtmlString(year));

            // Arrange
            Assert.Throws<ArgumentNullException>(() => _localizer.Format(key, tags.ToArray()));
        }

        [Fact]
        public void CallLocalizerFormat_Html_NullLocalizer()
        {
            // Assets
            var key = (string)null;
            var tags = new List<Func<string, IHtmlContent>>();
            var year = DateTime.Now.Year.ToString();
            tags.Add(str => new HtmlString(year));

            // Arrange
            Assert.Throws<ArgumentNullException>(() => LocalizerExtensions.Format(null, key, tags.ToArray()));
        }

        [Fact]
        public async Task CallLocalizerFormat_Html()
        {
            // Assets
            const string key = "Copyright";
            var tags = new List<Func<string, IHtmlContent>>();
            var year = DateTime.Now.Year.ToString();
            tags.Add(str => new HtmlString(year));

            // Acts
            CultureInfo.CurrentCulture = Constants.DefaultCulture;
            await _localizer.RefreshAsync();
            var act = _localizer.Format(key, tags.ToArray()).GetString();

            var expected = $"Telif Hakkı © {DateTime.Now.Year} EKOHOS Kurumsal";

            // Arrange
            Assert.Equal(expected, act);
        }

        [Fact]
        public void CallLocalizerHtml_NullTags()
        {
            // Assets
            var key = "CookieBanner";

            // Arranges
            Assert.Throws<ArgumentNullException>(() => _localizer.Html(key));
        }

        [Fact]
        public void CallLocalizerHtml_NullKey()
        {
            // Assets
            string key = null;

            // Arranges
            Assert.Throws<ArgumentNullException>(() => _localizer.Html(key));
        }

        [Fact]
        public void CallLocalizerHtml_NullLocalizer()
        {
            // Assets
            string key = null;

            // Arranges
            Assert.Throws<ArgumentNullException>(() => LocalizerExtensions.Html(null, key));
        }

        [Fact]
        public async Task CallGetter_WithCulture()
        {
            // Assets
            var key = "AppName";
            var culture = CultureInfo.GetCultureInfo("fa");

            // Act
            await _localizer.RefreshAsync();
            var translation = _localizer[key, culture];

            // Arrange
            Assert.Equal("هلدینگ اکوهوس", translation);
        }

        [Fact]
        public void CallRefreshAsync_ConfigurationNull()
        {
            // Arrange
            Assert.Throws<ArgumentNullException>(() => new Localizer(null, null));
        }
    }
}