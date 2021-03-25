using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using R8.AspNetCore.Test;
using R8.FileHandlers.AspNetCore.Test.FakeObjects;
using R8.FileHandlers.AspNetCore.Test.TestServerSimulation;

using System;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace R8.FileHandlers.AspNetCore.Test
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
            _localizer = serviceProvider.GetService<Lib.Localization.ILocalizer>();
            _localizer.RefreshAsync().GetAwaiter().GetResult();
            serviceProvider.UseFileHandlers();
        }

        [Fact]
        public async Task CallUploadFile()
        {
            // Assets
            var formFile = FakeObjects.Extensions.GetFormFile(Constants.GetValidImageFile());
            //Act
            var file = await formFile.UploadAsync();

            Assert.NotNull(file);
            Assert.InRange(file.FileSize, 1, 99999999999);
            Assert.Equal($"/uploads/{DateTime.UtcNow.Year}/{DateTime.UtcNow.Month:00}/{DateTime.UtcNow.Day:00}/valid.png", file.FilePath);
        }

        [Fact]
        public void CallValidateFile()
        {
            // Assets
            var formFile = FakeObjects.Extensions.GetFormFile(Constants.GetValidImageFile());
            var model = new FakeValidatableFile
            {
                File = formFile
            };

            //Act
            var file = model.Validate();

            Assert.True(file);
        }

        [Fact]
        public void CallValidateFile4()
        {
            // Assets
            var formFile = FakeObjects.Extensions.GetFormFile(Constants.GetValidImageFile());
            var model = new FakeValidatableFile
            {
                File3 = formFile
            };

            //Act
            var file = model.Validate();

            Assert.True(file);
        }

        [Fact]
        public void CallValidateFile10()
        {
            // Assets
            var formFile = FakeObjects.Extensions.GetFormFile(Constants.GetValidImageFile());

            //Act
            var valid = WebFileValidator.TryValidateFile<FakeValidatableFile>(nameof(FakeValidatableFile.File),
                formFile, out var errors);

            Assert.True(valid);
        }

        [Fact]
        public void CallValidateFile11()
        {
            // Assets
            var formFile = FakeObjects.Extensions.GetFormFile(Constants.GetValidPdfFile());

            //Act
            var valid = WebFileValidator.TryValidateFile<FakeValidatableFile>(nameof(FakeValidatableFile.File),
                formFile, out var errors);

            Assert.False(valid);
            Assert.NotNull(errors);
            Assert.NotEmpty(errors);
        }

        [Fact]
        public void CallValidateFile12()
        {
            // Assets
            var formFile = FakeObjects.Extensions.GetFormFile(Constants.GetValidImageFile());
            var formFile2 = FakeObjects.Extensions.GetFormFile(Constants.GetValidPdfFile());
            var files = new FormFileCollection();
            files.Add(formFile);
            files.Add(formFile2);

            //Act
            var valid = WebFileValidator.TryValidateFile<FakeValidatableFile>(nameof(FakeValidatableFile.Files),
                files, out var errors);

            Assert.False(valid);
        }

        [Fact]
        public void CallValidateFile13()
        {
            // Assets
            var formFile = FakeObjects.Extensions.GetFormFile(Constants.GetValidImageFile());
            var formFile2 = FakeObjects.Extensions.GetFormFile(Constants.GetValidImageFile());
            var files = new FormFileCollection();
            files.Add(formFile);
            files.Add(formFile2);

            //Act
            var valid = WebFileValidator.TryValidateFile<FakeValidatableFile>(nameof(FakeValidatableFile.Files),
                files, out var errors);

            Assert.True(valid);
        }

        [Fact]
        public void CallValidateFile5()
        {
            // Assets
            var formFile = FakeObjects.Extensions.GetFormFile(Constants.GetValidPdfFile());
            var model = new FakeValidatableFile
            {
                File3 = formFile
            };

            //Act
            var file = model.Validate();

            Assert.False(file);
        }

        [Fact]
        public void CallValidateFile2()
        {
            // Assets
            var formFile = FakeObjects.Extensions.GetFormFile(Constants.GetValidPdfFile());
            var model = new FakeValidatableFile
            {
                File = formFile
            };

            // Act
            var file = model.Validate();

            // Arrange
            Assert.False(file);
        }

        [Fact]
        public void CallValidateFile3()
        {
            // Assets
            var formFile = FakeObjects.Extensions.GetFormFile(Constants.GetValidPdfFile());
            var model = new FakeValidatableFile3
            {
                File2 = formFile
            };

            // Arrange
            Assert.Throws<ArgumentNullException>(() => model.Validate());
        }

        [Fact]
        public void CallValidateImage()
        {
            // Assets
            var formFile = FakeObjects.Extensions.GetFormFile(Constants.GetValidImageFile());
            var model = new FakeValidateImageSize
            {
                File = formFile
            };

            // Act
            var file = model.Validate();

            // Arrange
            Assert.False(file);
        }

        [Fact]
        public void CallValidateImage2()
        {
            // Assets
            var formFile = FakeObjects.Extensions.GetFormFile(Constants.GetValidImageFile());
            var model = new FakeValidateImageSize
            {
                File2 = formFile
            };

            // Act
            var file = model.Validate();

            // Arrange
            Assert.True(file);
        }
    }
}