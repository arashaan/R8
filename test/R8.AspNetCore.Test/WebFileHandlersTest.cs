using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;

using R8.AspNetCore.Demo;
using R8.AspNetCore.FileHandlers;
using R8.AspNetCore.Test.FakeBack;

using Xunit;

namespace R8.AspNetCore.Test
{
    public class WebFileHandlersTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public WebFileHandlersTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            FileHandlersConnection.Services = _factory.Services;
        }

        [Fact]
        public async Task CallUploadFile()
        {
            // Assets
            var formFile = FakeCore.MockFile(Constants.ValidImageFile);

            //Act
            var file = await formFile.UploadAsync();

            Assert.NotNull(file);
            Assert.Equal($"/uploads/{DateTime.Now.Year}/{DateTime.Now.Month:00}/{DateTime.Now.Day:00}/valid.png", file.FilePath);
        }
    }
}