using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using R8.AspNetCore.Sitemap;

namespace R8.AspNetCore.WebTest.Pages
{
    [SitemapSettings]
    public class PrivacyModel : PageModel
    {
        public PrivacyModel()
        {
        }

        public IActionResult OnGet()
        {
            return Page();
        }
    }
}