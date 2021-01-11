using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

using R8.Lib.Localization;

namespace R8.AspNetCore.Routing
{
    /// <summary>
    /// A full-fledged PageModel will optimizations for localization.
    /// </summary>
    public abstract class PageModelBase : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        /// <summary>
        /// A full-fledged <see cref="IUrlHelper"/> object with an options to access culture data.
        /// </summary>
        public ICulturalizedUrlHelper Url
        {
            get
            {
                var service = this.HttpContext.RequestServices.GetService(typeof(ICulturalizedUrlHelper));
                return (service ?? base.Url) as ICulturalizedUrlHelper;
            }
            set => Url = value;
        }

        /// <summary>
        /// An <see cref="ILocalizer"/> object to access dictionary data.
        /// </summary>
        public ILocalizer Localizer
        {
            get
            {
                var service = this.HttpContext.RequestServices.GetService(typeof(ILocalizer));
                return service as ILocalizer;
            }
        }

        public IAntiforgery Antiforgery
        {
            get
            {
                var service = HttpContext.RequestServices.GetService(typeof(IAntiforgery));
                return service as IAntiforgery;
            }
        }

        /// <summary>
        /// A <see cref="RequestLocalizationOptions"/> object.
        /// </summary>
        public RequestLocalizationOptions Culture
        {
            get
            {
                var service = this.HttpContext.RequestServices.GetService(typeof(IOptions<RequestLocalizationOptions>));
                return ((IOptions<RequestLocalizationOptions>)service)?.Value;
            }
        }

        /// <summary>
        /// Generates and stores anti-forgery token for ajax requests.
        /// </summary>
        /// <returns></returns>
        public string GetAntiforgeryToken()
        {
            return Antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
        }

        /// <summary>
        /// Gets or sets title of <see cref="PageModelBase"/>
        /// </summary>
        [ViewData]
        public string PageTitle { get; set; }

        protected string PagePath => GetType().GetPagePath();

        public override RedirectToPageResult RedirectToPage()
        {
            var thisRoutes = Request.RouteValues;
            var values = CulturalizedUrlHelper.TryAddCulture(Culture, thisRoutes, new RouteValueDictionary());
            return base.RedirectToPage(PagePath, values);
        }

        public override RedirectToPageResult RedirectToPage(object routeValues)
        {
            var thisRoutes = Request.RouteValues;
            var values = CulturalizedUrlHelper.TryAddCulture(Culture, thisRoutes, routeValues);
            return base.RedirectToPage(PagePath, values);
        }

        public RedirectToPageResult RedirectToPage<TPage>() where TPage : PageModelBase
        {
            var pageName = typeof(TPage).GetPagePath();
            var thisRoutes = Request.RouteValues;
            var values = CulturalizedUrlHelper.TryAddCulture(Culture, thisRoutes, new RouteValueDictionary());
            return base.RedirectToPage(pageName, values);
        }

        public RedirectToPageResult RedirectToPage<TPage>(object routeValues) where TPage : PageModelBase
        {
            var pageName = typeof(TPage).GetPagePath();
            var thisRoutes = Request.RouteValues;
            var values = CulturalizedUrlHelper.TryAddCulture(Culture, thisRoutes, routeValues);
            return base.RedirectToPage(pageName, values);
        }

        public override RedirectToPageResult RedirectToPage(string pageName)
        {
            var thisRoutes = Request.RouteValues;
            var values = CulturalizedUrlHelper.TryAddCulture(Culture, thisRoutes, new RouteValueDictionary());
            return base.RedirectToPage(pageName, values);
        }

        public override RedirectToPageResult RedirectToPage(string pageName, object routeValues)
        {
            var thisRoutes = Request.RouteValues;
            var values = CulturalizedUrlHelper.TryAddCulture(Culture, thisRoutes, routeValues);
            return base.RedirectToPage(pageName, values);
        }
    }
}