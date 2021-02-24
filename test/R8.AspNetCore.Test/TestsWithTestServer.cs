using Microsoft.Extensions.DependencyInjection;

using R8.AspNetCore.Localization;
using R8.AspNetCore.Test.ILocalizer;
using R8.AspNetCore.Test.TestServerSimulation;
using System.Globalization;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace R8.AspNetCore.Test
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
    }
}