using Microsoft.AspNetCore.Builder;

using R8.Lib.Wrappers;

using System;

namespace R8.AspNetCore.Localization
{
    public static class LocalizerDiConfiguration
    {
        public static void UseResponse(this IServiceProvider serviceProvider)
        {
            WrapperConnection.Services = serviceProvider;
        }

        public static IApplicationBuilder UseLocalizer(this IApplicationBuilder app)
        {
            WrapperConnection.Services = app.ApplicationServices;
            app.UseMiddleware<LocalizerMiddleware>();
            return app;
        }
    }
}