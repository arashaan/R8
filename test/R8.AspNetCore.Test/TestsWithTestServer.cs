using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using R8.AspNetCore.FileHandlers;
using R8.AspNetCore.Localization;
using R8.AspNetCore.Test.ILocalizer;
using R8.AspNetCore.Test.TestServerSimulation;
using R8.Test.Constants;

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
            _localizer =
                serviceProvider.GetService(typeof(Lib.Localization.ILocalizer)) as
                    Lib.Localization.ILocalizer;
            _localizer.InitializeLocalizer();

            serviceProvider.UseResponse();
            serviceProvider.UseFileHandlers();
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
        public async Task CallUploadFile()
        {
            // Assets
            var formFile = FakeCore.MockFile(Constants.ValidImageFile);
            //Act
            var file = await formFile.UploadAsync();

            Assert.NotNull(file);
            Assert.InRange(file.FileSize, 1, 99999999999);
            Assert.Equal($"/uploads/{DateTime.UtcNow.Year}/{DateTime.UtcNow.Month:00}/{DateTime.UtcNow.Day:00}/valid.png", file.FilePath);
        }
    }
}