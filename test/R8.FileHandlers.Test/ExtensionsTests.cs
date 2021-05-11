using R8.AspNetCore.Test;

using Xunit;
using Xunit.Abstractions;

namespace R8.FileHandlers.Test
{
    public class ExtensionsTests
    {
        private readonly ITestOutputHelper _output;

        public ExtensionsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CallGetAspectRatio()
        {
            const int x = 1920;
            const int y = 1080;

            var act = Extensions.GetAspectRatio(x, y);

            Assert.NotNull(act);
            Assert.Equal(16, act.X);
            Assert.Equal(9, act.Y);
        }

        [Fact]
        public void CallGetUniqueFileName()
        {
            var fileName = Constants.GetValidImageFile();
            var fileNameSanitized = fileName.Replace(".png", "").Replace("\\", "/");
            var act = Extensions.GetUniqueFileName(fileName);
            _output.WriteLine(fileName);

            Assert.NotNull(act);
            Assert.StartsWith(fileNameSanitized, act);
            Assert.Equal($"{fileNameSanitized}_2.png", act);
        }
    }
}