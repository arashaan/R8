using Microsoft.AspNetCore.Mvc;
using R8.AspNetCore.Routing;
using R8.AspNetCore.Sitemap;

namespace R8.AspNetCore3_1.Demo.Pages
{
    public class SitemapModel : PageModel
    {
        public IActionResult OnGet()
        {
            return new SitemapIndexResult();
        }
    }
}