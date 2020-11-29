using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using R8.AspNetCore.Demo.Services.Routing;
using R8.AspNetCore.FileHandlers;
using R8.Lib.Localization;

namespace R8.AspNetCore.Demo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ILocalizer _localizer;
        private readonly IOptions<RequestLocalizationOptions> _options;
        private readonly IWebHostEnvironment _environment;
        public const string Assets = "E:\\Work\\Develope\\Libraries\\R8.Lib\\R8.Lib.FileHandlers.Test\\assets";

        public IndexModel(ILogger<IndexModel> logger, ILocalizer localizer, IOptions<RequestLocalizationOptions> options, IWebHostEnvironment environment)
        {
            _logger = logger;
            _localizer = localizer;
            _options = options;
            _environment = environment;
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