using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

using R8.Lib.Localization;

namespace R8.AspNetCore.Routing
{
    /// <summary>
    /// A full-fledged PageModel will optimizations for localization.
    /// </summary>
    public abstract class PageModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        ///// <summary>
        ///// A full-fledged <see cref="IUrlHelper"/> object with an options to access culture data.
        ///// </summary>
        //public ICulturalizedUrlHelper Url
        //{
        //    get
        //    {
        //        var service = this.HttpContext.RequestServices.GetService(typeof(ICulturalizedUrlHelper));
        //        return (service ?? base.Url) as ICulturalizedUrlHelper;
        //    }
        //    set => Url = value;
        //}

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
        /// Generates and stores anti-forgery token for ajax requests.
        /// </summary>
        /// <returns></returns>
        public string GetAntiforgeryToken()
        {
            return Antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
        }

        /// <summary>
        /// Gets or sets title of <see cref="PageModel"/>
        /// </summary>
        [ViewData]
        public string PageTitle { get; set; }

        protected string PagePath => GetType().GetPagePath();
    }
}