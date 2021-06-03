using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using R8.EntityFrameworkCore.Audits;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace R8.AspNetCore.Test.FakeObjects
{
    public class FakeCore
    {
        public readonly IWebHostEnvironment WebHostEnvironment;

        public readonly IMemoryCache MemoryCache;
        public readonly Mock<IServiceProvider> ServiceProviderMock;

        public FakeCore()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
              .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
              .Returns(Task.FromResult((object)null));

            ServiceProviderMock = new Mock<IServiceProvider>();
            ServiceProviderMock
              .Setup(_ => _.GetService(typeof(IAuthenticationService)))
              .Returns(authServiceMock.Object);

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(ServiceProviderMock.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
              .Setup(x => x.CreateScope())
              .Returns(serviceScope.Object);

            ServiceProviderMock
              .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
              .Returns(serviceScopeFactory.Object);

            var moqEnvironment = new Mock<IWebHostEnvironment>();
            moqEnvironment.Setup(m => m.EnvironmentName).Returns("Hosting:UnitTestEnvironment");
            WebHostEnvironment = moqEnvironment.Object;

            // var moqHttpContextAccessor = new Mock<IHttpContextAccessor>();
            // var httpContext = new DefaultHttpContext
            // {
            //     RequestServices = ServiceProviderMock.Object,
            //     User = new ClaimsPrincipal(new ClaimsIdentity(UserService.GetUserClaims(GetUser())))
            // };
            // moqHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);
            // HttpContextAccessor = moqHttpContextAccessor.Object;

            var memoryCache = Mock.Of<IMemoryCache>();
            var mockMemoryCache = Mock.Get(memoryCache);
            mockMemoryCache
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>());
            MemoryCache = memoryCache;
        }

        public AuditCollection GetAudits(Guid rowId)
        {
            return new AuditCollection(new List<Audit>()
            {
                new Audit
                {
                    RowId = rowId,
                    Id = Guid.NewGuid(),
                    DateTime = DateTime.UtcNow,
                    Flag = AuditFlags.Created
                }
            });
        }

        public IUrlHelper GetUrlHelper(string returnUrl = "http://localhost:8080/")
        {
            var mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Strict);
            mockUrlHelper
                .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns(returnUrl)
                .Verifiable();
            return mockUrlHelper.Object;
        }

        public IHttpContextAccessor GetHttpContextAccessor()
        {
            var httpContext = new DefaultHttpContext
            {
                //User = new ClaimsPrincipal(new ClaimsIdentity(UserService.GetUserClaims(GetUser()))),
                RequestServices = ServiceProviderMock.Object
            };
            httpContext.Request.Protocol = "HTTP/1.1";
            httpContext.Request.Path = "/";
            httpContext.Request.PathBase = new PathString();
            httpContext.Request.RouteValues.Add(R8.AspNetCore.Localization.Constraints.LanguageKey, "en");
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost");

            var moqHttpContextAccessor = new Mock<IHttpContextAccessor>();
            moqHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

            return moqHttpContextAccessor.Object;
        }
    }
}