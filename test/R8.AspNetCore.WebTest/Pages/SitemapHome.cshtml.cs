using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using R8.AspNetCore.Sitemap;

namespace R8.AspNetCore.WebTest.Pages
{
    [SitemapIndex]
    public class SitemapHomeModel : PageModel
    {
        public IActionResult OnGet()
        {
            var nameSpace = this.GetType().Namespace;
            return new SitemapResult();
            // return new SitemapResult(nameSpace);
        }
    }
}