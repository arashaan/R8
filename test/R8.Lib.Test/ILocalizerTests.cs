using R8.AspNetCore.Test;
using R8.Lib.Localization;
using R8.Lib.Test.Enums;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using Xunit;

namespace R8.Lib.Test
{
    public class ILocalizerTests
    {
        private readonly Localizer _localizer;

        private static List<CultureInfo> SupportedCultures => new List<CultureInfo>
        {
            CultureInfo.GetCultureInfo("tr"),
            CultureInfo.GetCultureInfo("en"),
            CultureInfo.GetCultureInfo("fa"),
        };

        public ILocalizerTests()
        {
            var configuration = new LocalizerConfiguration
            {
                SupportedCultures = SupportedCultures,
                Provider = new LocalizerJsonProvider
                {
                    Folder = Constants.GetLocalizerDictionaryPath(),
                    FileName = Constants.JsonFileName,
                }
            };
            _localizer = new Localizer(configuration, null);
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
        public async Task CallGetter_WithCulture2()
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
        public async Task CallGetter_WithCulture()
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

        [Fact]
        public async Task CallGetter_SpecificCulture()
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
        public async Task CallTryGetValue_Enum()
        {
            // Assets
            var culture = CultureInfo.GetCultureInfo("fa");

            // Act
            await _localizer.RefreshAsync();
            var localized = _localizer[Flags.Success, culture];

            // Arrange
            Assert.Equal("عملیات به موفقیت انجام شد", localized);
        }

        [Fact]
        public async Task CallTryGetValue_Enum2()
        {
            // Assets
            var culture = CultureInfo.GetCultureInfo("fa");

            // Act
            await _localizer.RefreshAsync();
            var localized = _localizer[Flags.Success].Get("fa");

            // Arrange
            Assert.Equal("عملیات به موفقیت انجام شد", localized);
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
        public async System.Threading.Tasks.Task CallRefreshAsync_FileNameNull()
        {
            // Assets
            var configuration = new LocalizerConfiguration
            {
                SupportedCultures = SupportedCultures,
                Provider = new LocalizerJsonProvider
                {
                    Folder = Constants.GetLocalizerDictionaryPath(),
                }
            };

            // Act
            var localizer = new Localizer(configuration, null);

            // Arrange
            await Assert.ThrowsAsync<NullReferenceException>(() => localizer.RefreshAsync());
        }

        [Fact]
        public async System.Threading.Tasks.Task CallRefreshAsync_FolderNull()
        {
            // Assets
            var configuration = new LocalizerConfiguration
            {
                SupportedCultures = SupportedCultures,
                Provider = new LocalizerJsonProvider()
            };

            // Act
            var localizer = new Localizer(configuration, null);

            // Arrange
            await Assert.ThrowsAsync<NullReferenceException>(() => localizer.RefreshAsync());
        }

        [Fact]
        public async System.Threading.Tasks.Task CallRefreshAsync_SupportedNull()
        {
            // Act
            var localizer = new Localizer(new LocalizerConfiguration(), null);

            // Arrange
            await Assert.ThrowsAsync<NullReferenceException>(() => localizer.RefreshAsync());
        }
    }
}