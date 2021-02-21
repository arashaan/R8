using R8.Lib.Localization;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using R8.AspNetCore.Test.FakeObjects;
using R8.Test.Shared;
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
            Assert.ThrowsAsync<NullReferenceException>(() => ctor.RefreshAsync());
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