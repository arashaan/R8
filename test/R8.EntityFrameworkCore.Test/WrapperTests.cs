using R8.AspNetCore.Test;
using R8.EntityFrameworkCore.Test.Enums;
using R8.Lib.Localization;

using System.Collections.Generic;
using System.Globalization;

using Xunit;

namespace R8.EntityFrameworkCore.Test
{
    public class WrapperTests
    {
        private readonly Localizer _localizer;

        private static List<CultureInfo> SupportedCultures => new List<CultureInfo>
        {
            CultureInfo.GetCultureInfo("tr"),
            CultureInfo.GetCultureInfo("en"),
            CultureInfo.GetCultureInfo("fa"),
        };

        public WrapperTests()
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
            _localizer.RefreshAsync().GetAwaiter().GetResult();
        }

        [Fact]
        public void CallWrapperGeneric_Message()
        {
            // Assets
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");

            // Act
            var response = new FakeWrapper<object>(Flags.Success);
            response.SetLocalizer(_localizer);

            const string expected = "عملیات به موفقیت انجام شد";

            // Arrange
            Assert.Equal(expected, response.Message);
        }

        [Fact]
        public void CallWrapperGeneric_CheckSuccess()
        {
            // Assets
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");

            // Act
            var response = new FakeWrapper<object>(Flags.Success);

            // Arrange
            Assert.True(response.Success);
        }

        //[Fact]
        //public void CallResponseGeneric_CheckSuccess2()
        //{
        //    // Assets
        //    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");

        //    // Act
        //    var response = new FakeWrapper<object>(Flags.Success);
        //    response.Save = DatabaseSaveState.SaveFailure;

        //    // Arrange
        //    Assert.False(response.Success);
        //}
    }
}