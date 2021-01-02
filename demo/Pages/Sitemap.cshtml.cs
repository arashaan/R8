using Microsoft.AspNetCore.Mvc;

using R8.AspNetCore.Routing;
using R8.AspNetCore.Sitemap;

namespace R8.AspNetCore.Demo.Pages
{
    public class SitemapModel : PageModelBase
    {
        public IActionResult OnGet()
        {
            return new SitemapIndexResult();
        }
    }
}