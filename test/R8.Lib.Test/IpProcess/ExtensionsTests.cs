using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using R8.Lib.IPProcess;

using Xunit;
using Xunit.Abstractions;

namespace R8.Lib.Test.IpProcess
{
    public class ExtensionsTests
    {
        private readonly ITestOutputHelper _output;

        public ExtensionsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task CallGetAsn()
        {
            // Assets
            var ipAddress = IPAddress.Parse("109.108.160.241");

            // Acts
            var ip = await ipAddress.GetIpAddressAsync().ConfigureAwait(false);
            await ip.Isp.GetInformationAsync().ConfigureAwait(false);

            // Assert
            Assert.NotNull(ip);
            Assert.NotNull(ip.Isp.Name);
            Assert.StartsWith("Mobin Net", ip.Isp.Name);
        }

        [Fact]
        public async Task CallGetIpAddressAsync()
        {
            // Assets
            var ipAddress = IPAddress.Parse("162.158.93.83");

            // Acts
            var ip = await ipAddress.GetIpAddressAsync();

            // Assert
            Assert.NotNull(ip);
            Assert.Equal("Germany", ip.Country);
            Assert.Equal("Euro", ip.Currency.Name);
        }

        [Theory]
        [InlineData("109.108.160.241")]
        [InlineData("162.158.93.83")]
        public async Task CallGetIpAddressAsyncString(string ipAddress)
        {
            // Assets

            // Acts
            var ip = await R8.Lib.IPProcess.Extensions.GetIpAddressAsync(ipAddress);

            // Assert
            Assert.NotNull(ip);
        }
    }
}