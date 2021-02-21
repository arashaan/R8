using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;

using R8.Lib.Localization;
using R8.Lib.MethodReturn;

namespace R8.AspNetCore.Localization
{
    public static class LocalizerDiConfiguration
    {
        public static void UseResponse(this IServiceProvider serviceProvider)
        {
            ResponseConnection.Services = serviceProvider;
        }

        public static IApplicationBuilder UseResponse(this IApplicationBuilder app)
        {
            ResponseConnection.Services = app.ApplicationServices;
            return app;
        }

        public static IApplicationBuilder UseLocalizer(this IApplicationBuilder app)
        {
            app.UseResponse();
            app.UseMiddleware<LocalizerMiddleware>();
            return app;
        }
    }
}