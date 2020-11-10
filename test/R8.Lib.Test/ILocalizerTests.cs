using R8.Lib.Localization;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Xunit;

namespace R8.Lib.Test
{
    public class ILocalizerTests
    {
        private readonly Localizer _localizer;

        private static CultureInfo DefaultCulture => CultureInfo.GetCultureInfo("tr");
        private static string FolderPath => Path.Combine(Directory.GetCurrentDirectory(), "Dictionary");
        private static string JsonFileName => "dic";

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
                Folder = FolderPath,
                FileName = JsonFileName,
                DefaultCulture = DefaultCulture,
                SupportedCultures = SupportedCultures
            };
            _localizer = new Localizer(configuration);
        }

        [Fact]
        public void CallHandleDictionary_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Localizer.HandleDictionary(null));
        }

        [Fact]
        public void CallHandleDictionary_Empty()
        {
            // Assets
            var jsonString = "{}";

            // Acts
            var dic = Localizer.HandleDictionary(jsonString);

            // Arrange
            Assert.NotNull(dic);
            Assert.Empty(dic);
        }

        [Fact]
        public async Task CallHandleLanguage_FileNotFound()
        {
            // Assets
            var culture = CultureInfo.GetCultureInfo("tr");
            var configuration = new LocalizerConfiguration
            {
                Folder = FolderPath,
                FileName = "fa",
                SupportedCultures = SupportedCultures
            };
            var localizer = new Localizer(configuration);

            // Arrange
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                localizer.HandleLanguageAsync(culture));
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

        // [Fact]
        // public async Task CallTryGetValue()
        // {
        //     // Assets
        //     var key = "AppName";
        //
        //     // Act
        //     await _localizer.RefreshAsync();
        //     var getter = _localizer.GetValue(key, out var localized);
        //
        //     // Arrange
        //     Assert.True(getter);
        //     Assert.NotNull(localized);
        // }

        //[Fact]
        //public void CallGetter_NullText()
        //{
        //    // Assets
        //    var key = string.Empty;

        //    // Act
        //    var translation = _localizer[key];

        //    // Arrange
        //    Assert.Null(translation[DefaultCulture]);
        //}

        [Fact]
        public async Task CallGetter_NullExpressionKey()
        {
            // Assets
            Expression<Func<string>> key = null;

            // Act
            await _localizer.RefreshAsync();
            Assert.Throws<ArgumentNullException>(() => _localizer[key]);
        }

        [Fact]
        public async Task CallGetter3()
        {
            // Assets
            Expression<Func<string>> key = () => "AppName";

            // Act
            await _localizer.RefreshAsync();
            var translation = _localizer[key][DefaultCulture, false];

            // Arrange
            Assert.Equal("EKOHOS", translation);
        }

        [Fact]
        public async Task CallGetter2()
        {
            // Assets
            Expression<Func<string>> key = () => "AppName";

            // Act
            await _localizer.RefreshAsync();
            var translation = _localizer[key]["en", false];

            // Arrange
            Assert.Equal("ECOHOS Holding", translation);
        }

        [Fact]
        public async Task CallGetter_WithCulture()
        {
            // Assets
            var key = "AppName";

            // Act
            await _localizer.RefreshAsync();
            var translation = _localizer[key, DefaultCulture];

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
            var translation = _localizer[key, DefaultCulture];

            // Arrange
            Assert.Null(translation);
        }

        [Fact]
        public async Task CallGetter4()
        {
            // Assets
            Expression<Func<string>> key = () => "AppName";

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
            var translation = _localizer[key][DefaultCulture, false];

            // Arrange
            Assert.Equal("EKOHOS", translation);
        }

        [Fact]
        public async Task CallGetter_ShouldntBeEqualToKey()
        {
            // Assets
            var key = "AppName";

            // Act
            await _localizer.RefreshAsync();
            var translation = _localizer[key][DefaultCulture, false];

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
        public async System.Threading.Tasks.Task CallRefreshAsync_ConfigurationNull()
        {
            // Act
            var localizer = new Localizer(null);

            // Arrange
            await Assert.ThrowsAsync<NullReferenceException>(() => localizer.RefreshAsync());
        }

        [Fact]
        public async System.Threading.Tasks.Task CallRefreshAsync_FileNameNull()
        {
            // Assets
            var configuration = new LocalizerConfiguration
            {
                Folder = FolderPath,
                SupportedCultures = SupportedCultures
            };

            // Act
            var localizer = new Localizer(configuration);

            // Arrange
            await Assert.ThrowsAsync<ArgumentNullException>(() => localizer.RefreshAsync());
        }

        [Fact]
        public async System.Threading.Tasks.Task CallRefreshAsync_FolderNull()
        {
            // Assets
            var configuration = new LocalizerConfiguration
            {
                SupportedCultures = SupportedCultures
            };

            // Act
            var localizer = new Localizer(configuration);

            // Arrange
            await Assert.ThrowsAsync<ArgumentNullException>(() => localizer.RefreshAsync());
        }

        [Fact]
        public async System.Threading.Tasks.Task CallRefreshAsync_SupportedNull()
        {
            // Act
            var localizer = new Localizer(new LocalizerConfiguration());

            // Arrange
            await Assert.ThrowsAsync<NullReferenceException>(() => localizer.RefreshAsync());
        }

        [Fact]
        public void CallHandleDictionary_IncludeItem()
        {
            // Assets
            var jsonString = "{\"AppName\": \"ECOHOS\"}";

            // Acts
            var dic = Localizer.HandleDictionary(jsonString);

            // Arrange
            Assert.NotNull(dic);
            Assert.NotEmpty(dic);
        }
    }
}