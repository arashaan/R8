using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

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
        public void CallGetIpAddress()
        {
            // Assets

            // Act
            var ip = HttpExtensions.GetIPAddress();
            _output.WriteLine(ip.ToString());

            // Arrange
            Assert.NotNull(ip);
            Assert.NotEqual(IPAddress.None, ip);
        }
    }
}