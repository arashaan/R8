using Microsoft.AspNetCore.Mvc;

using R8.AspNetCore.Routing;
using R8.AspNetCore3_1.Demo.Pages;

namespace R8.AspNetCore3_1.Demo.Controller
{
    public class HomeController : AspNetCore.Routing.Controller
    {
        public IActionResult Index()
        {
            return this.RedirectToPageLocalized(typeof(IndexModel).GetPagePath(), new { propId = 123 });
        }

        public IActionResult Index2()
        {
            return this.RedirectToPageLocalized(typeof(IndexModel).GetPagePath());
        }
    }
}