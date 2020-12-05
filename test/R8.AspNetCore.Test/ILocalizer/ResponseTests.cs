using System.Globalization;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using R8.AspNetCore.Demo;
using R8.AspNetCore.Localization;
using R8.Lib.MethodReturn;

using Xunit;

namespace R8.AspNetCore.Test.ILocalizer
{
    public class ResponseTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly Lib.Localization.ILocalizer _localizer;

        public ResponseTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;

            using var scope = factory.Services.CreateScope();
            _localizer = scope.ServiceProvider.GetService(typeof(Lib.Localization.ILocalizer)) as Lib.Localization.ILocalizer;
            _localizer.InitializeLocalizer();
            ResponseConnection.Services = _factory.Services;
        }

        [Fact]
        public void CallResponse_ToString()
        {
            // Assets
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");

            // Act
            var response = new FakeResponse(Flags.Success);
            var expected = "عملیات به موفقیت انجام شد";

            // Arrange
            Assert.Equal(expected, response.Message);
        }
    }
}