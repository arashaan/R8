using Microsoft.AspNetCore.Mvc;
using R8.AspNetCore.Routing;
using R8.AspNetCore.WebTest.Pages;

namespace R8.AspNetCore.WebTest.Controller
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