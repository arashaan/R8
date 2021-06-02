using R8.AspNetCore.Test;
using R8.Lib.Localization;

using System.Collections.Generic;
using System.Globalization;

namespace R8.Lib.Test.MethodTests
{
    public class ResponseTests
    {
        private readonly Localizer _localizer;

        private static List<CultureInfo> SupportedCultures => new List<CultureInfo>
        {
            CultureInfo.GetCultureInfo("tr"),
            CultureInfo.GetCultureInfo("en"),
            CultureInfo.GetCultureInfo("fa"),
        };

        public ResponseTests()
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

        // [Fact]
        // public async Task CallResponse_Message()
        // {
        //     // Assets
        //     CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");
        //
        //     // Act
        //     var response = new FakeResponse(Flags.Success);
        //     response.SetLocalizer(_localizer);
        //
        //     var expected = "عملیات به موفقیت انجام شد";
        //
        //     // Arrange
        //     Assert.Equal(expected, response.Message);
        // }

        //[Fact]
        //public void CallResponseGeneric_DirectCast4()
        //{
        //    // Act
        //    var response = new FakeResponse<object>();
        //    var expected = new ResponseBaseCollection<Flags>();
        //    expected.Add(response);

        //    // Arrange
        //    Assert.Equal(expected, response);
        //}

        // [Fact]
        // public void CallResponse_CheckSuccess2()
        // {
        //     // Assets
        //     CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");
        //
        //     // Act
        //     var response = new FakeResponse(Flags.ParamIsNull);
        //
        //     // Arrange
        //     Assert.False(response.Success);
        // }

        // [Fact]
        // public void CallResponse_CheckSuccess()
        // {
        //     // Assets
        //     CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");
        //
        //     // Act
        //     var response = new FakeResponse(Flags.Success);
        //
        //     // Arrange
        //     Assert.True(response.Success);
        // }

        // [Fact]
        // public void CallResponse_SetStatus()
        // {
        //     // Assets
        //     CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");
        //
        //     // Act
        //     var response = new FakeResponse();
        //     response.SetStatus(Flags.ParamIsNull);
        //
        //     // Arrange
        //     Assert.Equal(Flags.ParamIsNull, response.Status);
        // }

        // [Fact]
        // public async Task CallResponse_ToString()
        // {
        //     // Assets
        //     CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");
        //
        //     // Act
        //     var response = new FakeResponse(Flags.Success);
        //     response.SetLocalizer(_localizer);
        //
        //     var expected = "عملیات به موفقیت انجام شد";
        //
        //     // Arrange
        //     Assert.Equal(expected, response.Message);
        // }

        // [Fact]
        // public void CallResponse_CheckLocalizer()
        // {
        //     // Assets
        //
        //     // Act
        //     var response = new FakeResponse();
        //     response.SetLocalizer(_localizer);
        //     var localizer = response.GetLocalizer();
        //
        //     // Arrange
        //     Assert.NotNull(localizer);
        // }
    }
}