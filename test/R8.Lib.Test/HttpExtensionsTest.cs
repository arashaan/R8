using System;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

using Xunit;
using Xunit.Abstractions;

namespace R8.Lib.Test
{
    public class HttpExtensionsTest
    {
        private readonly ITestOutputHelper _output;

        public HttpExtensionsTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task CallGetAsn()
        {
            // Assets
            var ipAddress = IPAddress.Parse("109.108.160.241");

            // Acts
            var ip = await ipAddress.GetIPAddressInformationAsync().ConfigureAwait(false);

            // Assert
            Assert.NotNull(ip);
            Assert.NotNull(ip.ISPName);
            Assert.True(ip.ISPName.Equals("MCI", StringComparison.InvariantCultureIgnoreCase));
        }

        [Fact]
        public async Task CallGetIpAddressAsync2()
        {
            // Assets
            var ipAddress = IPAddress.Parse("162.158.93.83");

            // Acts
            var ip = await ipAddress.GetIPAddressInformationAsync();
            var flag = await ip.GetFlagAsync();

            using var xmlReader = XmlReader.Create(flag, new XmlReaderSettings() { Async = true });
            var svg = await xmlReader.MoveToContentAsync() == XmlNodeType.Element &&
                      "svg".Equals(xmlReader.Name, StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.InRange(flag.Length, 1, 9999999);
            Assert.True(svg);
        }

        [Fact]
        public async Task CallGetIpAddressAsync()
        {
            // Assets
            var ipAddress = IPAddress.Parse("162.158.93.83");
            var timeZone = Dates.GetNodaTimeZone("Europe/Berlin", false);

            // Acts
            var ip = await ipAddress.GetIPAddressInformationAsync();

            // Assert
            Assert.NotNull(ip);
            Assert.Equal("Germany", ip.Country);
            Assert.Equal("Euro", ip.Currency.Name);
            Assert.Equal("Europe", ip.Continent);
            Assert.Equal("Berlin", ip.Capital);
            Assert.Equal("+49", ip.CountryPhoneCode);
            Assert.Contains("Hesse", ip.Region);
            Assert.Contains("Frankfurt", ip.City);
            Assert.Equal(timeZone, ip.TimeZone);
        }

        [Theory]
        [InlineData("109.108.160.241")]
        [InlineData("162.158.93.83")]
        public async Task CallGetIpAddressAsyncString(string ipAddress)
        {
            // Assets

            // Acts
            var ip = await HttpExtensions.GetIPAddressInformationAsync(ipAddress);

            // Assert
            Assert.NotNull(ip);
        }

        [Fact]
        public void CallGetLocalIpAddress()
        {
            // Assets

            // Act
            var ip = HttpExtensions.GetLocalIPAddress();

            // Arrange
            Assert.NotNull(ip);
            Assert.NotEqual(IPAddress.None, ip);
        }

        [Fact]
        public async Task CallGetIpAddress()
        {
            // Assets

            // Act
            var ip = await HttpExtensions.GetIPAddressAsync();
            _output.WriteLine(ip.ToString());

            // Arrange
            Assert.NotNull(ip);
            Assert.NotEqual(IPAddress.None, ip);
        }
    }
}