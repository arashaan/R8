using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using R8.Lib.Localization;
using R8.Lib.Test.Enums;

using Xunit;

namespace R8.Lib.Test.MethodTests
{
    public class ExtensionsTests
    {
        private readonly Localizer _localizer;

        private static CultureInfo DefaultCulture => CultureInfo.GetCultureInfo("tr");
        private static string FolderPath => "E:\\Work\\Develope\\Ecohos\\Ecohos.Presentation\\Dictionary";
        private static string JsonFileName => "dic";

        private static List<CultureInfo> SupportedCultures => new List<CultureInfo>
        {
            CultureInfo.GetCultureInfo("tr"),
            CultureInfo.GetCultureInfo("en"),
            CultureInfo.GetCultureInfo("fa"),
        };

        public ExtensionsTests()
        {
            var configuration = new LocalizerConfiguration
            {
                SupportedCultures = SupportedCultures,
                Provider = new LocalizerJsonProvider
                {
                    Folder = FolderPath,
                    FileName = JsonFileName,
                }
            };

            _localizer = new Localizer(configuration, null);
        }

        [Fact]
        public void CallGetMessage_LocalizerNull()
        {
            // Assets
            var response = new FakeResponse(Flags.Success);

            // Act
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");
            var message = response.Message;

            var expected = "Success";

            // Arrange
            Assert.Equal(expected, message);
        }

        [Fact]
        public async Task CallGetMessage()
        {
            // Assets
            var response = new FakeResponse(Flags.Success);
            await _localizer.RefreshAsync();
            response.SetLocalizer(_localizer);

            // Act
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");
            var message = response.Message;

            var expected = "عملیات به موفقیت انجام شد";

            // Arrange
            Assert.Equal(expected, message);
        }
    }
}