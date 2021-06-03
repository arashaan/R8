using Microsoft.AspNetCore.Mvc;

using R8.AspNetCore.Routing;

using Controller = R8.AspNetCore.Routing.Controller;

namespace R8.AspNetCore.Test.FakeObjects
{
    public class FakeController : Controller
    {
        public IActionResult Index()
        {
            return this.RedirectToPageLocalized("/Index", new { propId = "123" });
        }
    }
}