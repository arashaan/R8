using Microsoft.AspNetCore.Mvc;
using R8.AspNetCore.Routing;
using R8.AspNetCore.Sitemap;

namespace R8.AspNetCore.WebTest.Pages
{
    public class SitemapModel : PageModel
    {
        public IActionResult OnGet()
        {
            return new SitemapIndexResult();
        }
    }
}