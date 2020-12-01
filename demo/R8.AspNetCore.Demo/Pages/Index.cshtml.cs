using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using R8.AspNetCore.Demo.Services.Routing;
using R8.AspNetCore.FileHandlers;

namespace R8.AspNetCore.Demo.Pages
{
    public class IndexModel : PageModel
    {
        public const string Assets = "E:\\Work\\Develope\\Libraries\\R8.Lib\\R8.Lib.FileHandlers.Test\\assets";

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
    }
}