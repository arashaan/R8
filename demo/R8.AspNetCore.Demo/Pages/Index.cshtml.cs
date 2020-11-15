using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using R8.AspNetCore.FileHandlers;

using System.Threading.Tasks;

namespace R8.AspNetCore.Demo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public const string Assets = "E:\\Work\\Develope\\Libraries\\R8.Lib\\R8.Lib.FileHandlers.Test\\assets";

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public IFormFile Test { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            var upload = await Test.UploadAsync();
            return RedirectToPage();
        }
    }
}