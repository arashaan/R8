using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using R8.Lib.Localization;

namespace R8.AspNetCore.Localization
{
    public class LocalizerMiddleware
    {
        private readonly RequestDelegate _next;

        public LocalizerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var localizer = context.RequestServices.GetService<ILocalizer>();
            if (localizer == null)
                throw new NullReferenceException($"{nameof(ILocalizer)} must be registered as a service.");

            await localizer.RefreshAsync();
            await _next(context);
        }
    }
}
