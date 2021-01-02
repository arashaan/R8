using System;
using System.Globalization;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace R8.AspNetCore.Localization
{
    public static class Extensions
    {
        public static CultureInfo GetCulture(this HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var features = context.Features ?? throw new ArgumentNullException(nameof(context.Features));
            var requestCulture = features.Get<IRequestCultureFeature>() ?? throw new ArgumentNullException("features.Get<IRequestCultureFeature>()");
            return requestCulture.RequestCulture.Culture;
        }
    }
}