using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml;

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
        public async Task CallGetIpAddressAsync2()
        {
            // Assets
            var ipAddress = IPAddress.Parse("162.158.93.83");

            // Acts
            var ip = await ipAddress.GetIpAddressAsync();
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
            var ip = await ipAddress.GetIpAddressAsync();

            // Assert
            Assert.NotNull(ip);
            Assert.Equal("Germany", ip.Country);
            Assert.Equal("Euro", ip.Currency.Name);
            Assert.Equal("Europe", ip.Continent);
            Assert.Equal("Berlin", ip.Capital);
            Assert.Equal("+49", ip.CountryPhoneCode);
            Assert.Equal("Hesse", ip.Region);
            Assert.Equal("Frankfurt", ip.City);
            Assert.Equal(timeZone, ip.TimeZone);
        }

        [Theory]
        [InlineData("109.108.160.241")]
        [InlineData("162.158.93.83")]
        public async Task CallGetIpAddressAsyncString(string ipAddress)
        {
            // Assets

            // Acts
            var ip = await IPProcess.Extensions.GetIpAddressAsync(ipAddress);

            // Assert
            Assert.NotNull(ip);
        }
    }
}