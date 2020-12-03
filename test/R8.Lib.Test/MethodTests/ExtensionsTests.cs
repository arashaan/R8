using R8.Lib.Localization;

using System.Collections.Generic;
using System.Globalization;

namespace R8.Lib.Test.MethodTests
{
    public class ExtensionsTests
    {
        private readonly Localizer _localizer;

        private static CultureInfo DefaultCulture => CultureInfo.GetCultureInfo("tr");
        private static string FolderPath => "E:\\Work\\Develope\\Asp\\Ecohos\\Ecohos.Presentation\\Dictionary";
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

        //[Fact]
        //public async Task CallGetMessage_NotShowable()
        //{
        //    // Assets
        //    var response = new Response(Flags.ParamIsNull);
        //    await _localizer.RefreshAsync();
        //    response.Localizer = _localizer;

        //    // Act
        //    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");
        //    var message = response.GetMessage();

        //    var expected = "خطا " + (int)Flags.ParamIsNull;

        //    // Arrange
        //    Assert.Equal(expected, message);
        //}

        //[Fact]
        //public void CallGetMessage_LocalizerNull()
        //{
        //    // Assets
        //    var response = new Response(Flags.Success);

        //    // Act
        //    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");
        //    var message = response.GetMessage();

        //    var expected = "Success";

        //    // Arrange
        //    Assert.Equal(expected, message);
        //}

        //[Fact]
        //public async Task CallGetMessage()
        //{
        //    // Assets
        //    var response = new Response(Flags.Success);
        //    await _localizer.RefreshAsync();
        //    response.Localizer = _localizer;

        //    // Act
        //    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");
        //    var message = response.GetMessage();

        //    var expected = "عملیات به موفقیت انجام شد";

        //    // Arrange
        //    Assert.Equal(expected, message);
        //}
    }
}