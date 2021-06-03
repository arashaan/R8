using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

using R8.Lib.Localization;

namespace R8.AspNetCore.Routing
{
    public abstract class Controller : Microsoft.AspNetCore.Mvc.Controller
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
    }
}