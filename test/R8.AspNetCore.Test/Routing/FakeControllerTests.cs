using Microsoft.AspNetCore.Mvc;

using R8.AspNetCore.Test.FakeObjects;
using R8.AspNetCore.WebTest.Controller;

using System.Linq;

using Xunit;

namespace R8.AspNetCore.Test.Routing
{
    public class FakeControllerTests : FakeCore
    {
        [Fact]
        public void CallIndex()
        {
            var httpContextAccessor = this.GetHttpContextAccessor();
            httpContextAccessor.HttpContext.Request.RouteValues.Add("page", "/Index");

            var controller = new HomeController
            {
                ControllerContext = new ControllerContext { HttpContext = httpContextAccessor.HttpContext },
                Url = this.GetUrlHelper()
            };

            // Act
            var result = controller.Index();
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Contains("propId", ((RedirectToPageResult)result).RouteValues.Select(x => x.Key).ToList());
            // Assert.Contains(((int)Flags.UnexpectedError).ToString(), ((RedirectToPageResult)result).RouteValues["status"].ToString());
        }

        [Fact]
        public void CallIndex2()
        {
            var httpContextAccessor = this.GetHttpContextAccessor();
            httpContextAccessor.HttpContext.Request.RouteValues.Add("page", "/Index");

            var controller = new HomeController
            {
                ControllerContext = new ControllerContext { HttpContext = httpContextAccessor.HttpContext },
                Url = this.GetUrlHelper()
            };

            // Act
            var result = controller.Index2();
            Assert.IsType<RedirectToPageResult>(result);
        }
    }
}