using System.Net;
using System.Threading.Tasks;
using R8.Lib.IPProcess;
using Xunit;

namespace R8.Lib.Test.IpProcess
{
    public class ExtensionsTests
    {
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
        }

        [Fact]
        public async Task CallGetIpAddressAsync()
        {
            // Assets
            var ipAddress = IPAddress.Parse("109.108.160.241");

            // Acts
            var ip = await IPProcess.Extensions.GetIpAddressAsync(ipAddress);

            // Assert
            Assert.NotNull(ip);
        }

        [Fact]
        public async Task CallGetIpAddressAsyncString()
        {
            // Assets
            const string ipAddress = "109.108.160.241";

            // Acts
            var ip = await IPProcess.Extensions.GetIpAddressAsync(ipAddress);

            // Assert
            Assert.NotNull(ip);
        }
    }
}