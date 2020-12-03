using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace R8.AspNetCore.Test.FakeBack
{
    public class FakeCore
    {
        public readonly IHttpContextAccessor HttpContextAccessor;
        public readonly IWebHostEnvironment WebHostEnvironment;
        public readonly IMemoryCache MemoryCache;

        public FakeCore(bool useInMemoryDb = true)
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
              .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
              .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
              .Setup(_ => _.GetService(typeof(IAuthenticationService)))
              .Returns(authServiceMock.Object);

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
              .Setup(x => x.CreateScope())
              .Returns(serviceScope.Object);

            serviceProviderMock
              .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
              .Returns(serviceScopeFactory.Object);

            // serviceProviderMock
            //   .Setup(_ => _.GetService(typeof(ResponseOptions)))
            //   .Returns(new ResponseOptions(typeof(Resources)));

            var moqEnvironment = new Mock<IWebHostEnvironment>();
            moqEnvironment.Setup(m => m.EnvironmentName).Returns("Hosting:UnitTestEnvironment");
            WebHostEnvironment = moqEnvironment.Object;

            var moqHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object,
            };
            moqHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);
            HttpContextAccessor = moqHttpContextAccessor.Object;

            var memoryCache = Mock.Of<IMemoryCache>();
            var mockMemoryCache = Mock.Get(memoryCache);
            mockMemoryCache
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>());
            MemoryCache = memoryCache;
        }

        public static IFormFile MockFile(string fileName)
        {
            var fileMock = new Mock<IFormFile>();
            var memoryStream = new MemoryStream();
            using var fileStream = new FileStream(fileName, FileMode.Open);
            fileStream.CopyTo(memoryStream);

            fileMock.Setup(_ => _.OpenReadStream()).Returns(memoryStream);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(memoryStream.Length);

            return fileMock.Object;
        }
    }
}