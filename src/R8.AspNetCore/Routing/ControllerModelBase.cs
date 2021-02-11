using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

using R8.Lib.Localization;

namespace R8.AspNetCore.Routing
{
    public abstract class ControllerModelBase : Controller
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

        public override RedirectToPageResult RedirectToPage(string pageName, string pageHandler)
        {
            var thisRoutes = Request.RouteValues;
            var values = CulturalizedUrlHelper.TryAddCulture(Culture, thisRoutes, new RouteValueDictionary());
            return base.RedirectToPage(pageName, pageHandler, values);
        }

        public override RedirectToPageResult RedirectToPage(string pageName, string pageHandler, object routeValues)
        {
            var thisRoutes = Request.RouteValues;
            var values = CulturalizedUrlHelper.TryAddCulture(Culture, thisRoutes, routeValues);
            return base.RedirectToPage(pageName, pageHandler, values);
        }

        public override RedirectToPageResult RedirectToPage(string pageName, string pageHandler, object routeValues, string fragment)
        {
            var thisRoutes = Request.RouteValues;
            var values = CulturalizedUrlHelper.TryAddCulture(Culture, thisRoutes, routeValues);
            return base.RedirectToPage(pageName, pageHandler, values, fragment);
        }

        public override RedirectToPageResult RedirectToPage(string pageName, string pageHandler, string fragment)
        {
            var thisRoutes = Request.RouteValues;
            var values = CulturalizedUrlHelper.TryAddCulture(Culture, thisRoutes, new RouteValueDictionary());
            return base.RedirectToPage(pageName, pageHandler, values, fragment);
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