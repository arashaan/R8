using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using R8.AspNetCore.Demo.Services.Routing;
using R8.AspNetCore.FileHandlers;
using R8.AspNetCore.Sitemap;

namespace R8.AspNetCore.Demo.Pages
{
    [SitemapSettings]
    public class IndexModel : PageModel
    {
        public IndexModel()
        {
            PageTitle = "Index";
        }

        [BindProperty]
        public IFormFile Test { get; set; }

        public void OnGet()
        {
            var testCulture = this.Culture;
            var testLocalizer = this.Localizer;
            var testCulturizedUrl = this.Url;
            if (testCulturizedUrl == null)
            {
            }
        }

        public async Task<IActionResult> OnPost()
        {
            var upload = await Test.UploadAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetUpdateLocalizerAsync()
        {
            await Localizer.RefreshAsync(true);
            return Page();
        }
    }
}