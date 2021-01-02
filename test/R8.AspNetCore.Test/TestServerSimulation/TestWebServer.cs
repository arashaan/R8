using System;
using System.Net.Http;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

using R8.AspNetCore.Demo;

using Xunit;
using Xunit.Abstractions;

namespace R8.AspNetCore.Test.TestServerSimulation
{
    public class TestWebServer : IClassFixture<TestServerFixture>, IDisposable
    {
        public readonly HttpClient Client;
        public readonly WebApplicationFactory<Startup> Fixture;
        public TestServer Server { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Fixture.Dispose();
            }
        }

        public TestWebServer(TestServerFixture fixture, ITestOutputHelper output)
        {
            Fixture = fixture.WithWebHostBuilder(builder => builder.UseStartup<Startup>().UseEnvironment("Test"));
            fixture.Output = output;
            Client = fixture.CreateClient();
            Server = fixture.Server;
        }
    }
}