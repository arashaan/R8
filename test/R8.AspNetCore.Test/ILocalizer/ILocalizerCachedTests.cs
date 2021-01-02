using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using R8.Lib.Localization;
using R8.Test.Constants;

using Xunit;

namespace R8.AspNetCore.Test.ILocalizer
{
    public class ILocalizerCachedTests : FakeCore
    {
        private readonly Localizer _localizer;

        private static List<CultureInfo> SupportedCultures => new List<CultureInfo>
        {
            CultureInfo.GetCultureInfo("tr"),
            CultureInfo.GetCultureInfo("en"),
            CultureInfo.GetCultureInfo("fa"),
        };

        public ILocalizerCachedTests()
        {
            var configuration = new LocalizerConfiguration
            {
                SupportedCultures = SupportedCultures,
                UseMemoryCache = true,
                Provider = new LocalizerJsonProvider
                {
                    Folder = Constants.FolderPath,
                    FileName = Constants.JsonFileName,
                }
            };
            _localizer = new Localizer(configuration, MemoryCache);
        }

        [Fact]
        public void CallLocalizerCached_NotRegisteredIMemoryCache()
        {
            // Assets
            var configuration = new LocalizerConfiguration
            {
                SupportedCultures = SupportedCultures,
                UseMemoryCache = true,
                Provider = new LocalizerJsonProvider
                {
                    Folder = Constants.FolderPath,
                    FileName = Constants.JsonFileName,
                }
            };

            // Act
            var ctor = new Localizer(configuration, null);

            // Arrange
            Assert.Throws<NullReferenceException>(() => ctor.Refresh());
        }

        [Fact]
        public async Task CallTryGetValue_SpecificCulture()
        {
            // Assets
            var key = "AppName";
            var culture = CultureInfo.GetCultureInfo("tr");

            // Act
            await _localizer.RefreshAsync();
            var localized = _localizer.GetValue(culture, key);

            // Arrange
            Assert.NotNull(localized);
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
        public async Task CallGetter_NullArg()
        {
            // Act
            await _localizer.RefreshAsync();
            var translation = _localizer[(string)null];

            // Arrange
            Assert.Null(translation);
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
        public async Task CallTryGetValue_CorrectLanguageButCouldntFindKey()
        {
            // Assets
            var key = "AppName2";
            var culture = CultureInfo.GetCultureInfo("tr");

            // Act
            await _localizer.RefreshAsync();
            var localized = _localizer.GetValue(culture, key);

            // Arrange
            Assert.Equal(key, localized);
        }

        [Fact]
        public void CallRefreshAsync_ConfigurationNull()
        {
            // Arrange
            Assert.Throws<ArgumentNullException>(() => new Localizer(null, null));
        }

        [Fact]
        public async Task CallRefreshAsync_FileNameNull()
        {
            // Assets
            var configuration = new LocalizerConfiguration
            {
                SupportedCultures = SupportedCultures,
                Provider = new LocalizerJsonProvider
                {
                    Folder = Constants.FolderPath,
                }
            };

            // Act
            var localizer = new Localizer(configuration, MemoryCache);

            // Arrange
            await Assert.ThrowsAsync<NullReferenceException>(() => localizer.RefreshAsync());
        }

        [Fact]
        public async Task CallRefreshAsync_FolderNull()
        {
            // Assets
            var configuration = new LocalizerConfiguration
            {
                SupportedCultures = SupportedCultures,
                Provider = new LocalizerJsonProvider()
            };

            // Act
            var localizer = new Localizer(configuration, MemoryCache);

            // Arrange
            await Assert.ThrowsAsync<NullReferenceException>(() => localizer.RefreshAsync());
        }

        [Fact]
        public async Task CallRefreshAsync_SupportedNull()
        {
            // Act
            var localizer = new Localizer(new LocalizerConfiguration(), MemoryCache);

            // Arrange
            await Assert.ThrowsAsync<NullReferenceException>(() => localizer.RefreshAsync());
        }
    }
}