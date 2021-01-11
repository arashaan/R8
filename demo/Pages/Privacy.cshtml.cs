using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using R8.AspNetCore.Sitemap;

namespace R8.AspNetCore3_1.Demo.Pages
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