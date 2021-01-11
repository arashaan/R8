using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace R8.AspNetCore.Routing
{
    public interface IViewRenderService
    {
        Task<Page> GetRazorPageAsync<TPageModel>(string pageName, TPageModel model, Action<StringWriter> writer = null) where TPageModel : PageModel;

        Task<Page> GetRazorPageAsync<TPageModel>(params object[] args) where TPageModel : PageModel;

        Task<Page> GetRazorPageAsync<TPageModel>(string pageName, TPageModel model, HttpContext httpContext, ActionContext actionContext, Action<StringWriter> writer = null) where TPageModel : PageModel;

        Task<string> RenderPageAsync<TPageModel>(string viewName, TPageModel model) where TPageModel : PageModel;

        Task<string> RenderPageAsync<TPageModel>(params object[] args) where TPageModel : PageModel;

        Task<string> RenderPageAsync<TPageModel>(string pageName, TPageModel model, HttpContext httpContext, ActionContext actionContext) where TPageModel : PageModel;
    }

    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IActionContextAccessor _actionContext;
        private readonly IRazorPageActivator _activator;
        private readonly IServiceProvider _serviceProvider;

        public ViewRenderService(IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IHttpContextAccessor httpContext,
            IServiceProvider serviceProvider,
            IRazorPageActivator activator,
            IActionContextAccessor actionContext)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _httpContext = httpContext;
            _actionContext = actionContext;
            _activator = activator;
        }

        public async Task<string> RenderPageAsync<TPageModel>(params object[] args) where TPageModel : PageModel
        {
            var pageName = typeof(TPageModel).GetPagePath();
            var model = Activator.CreateInstance(typeof(TPageModel), args) as TPageModel;

            var output = await RenderPageAsync(pageName, model);
            return output;
        }

        public async Task<string> RenderPageAsync<TPageModel>(string pageName, TPageModel model) where TPageModel : PageModel
        {
            var output = string.Empty;
            _ = await GetRazorPageAsync(pageName, model, writer =>
            {
                output = writer.ToString();
            });
            return output;
        }

        public async Task<string> RenderPageAsync<TPageModel>(string pageName, TPageModel model, HttpContext httpContext, ActionContext actionContext) where TPageModel : PageModel
        {
            var output = string.Empty;
            _ = await GetRazorPageAsync(pageName, model, httpContext, actionContext, writer =>
            {
                output = writer.ToString();
            });
            return output;
        }

        public async Task<Page> GetRazorPageAsync<TPageModel>(string pageName, TPageModel model, HttpContext httpContext, ActionContext actionContext, Action<StringWriter> writer = null) where TPageModel : PageModel
        {
            await using var sw = new StringWriter();
            var result = _razorViewEngine.FindPage(actionContext, pageName);

            if (result.Page == null)
                throw new ArgumentNullException($"The page {pageName} cannot be found.");

            var view = new RazorView(_razorViewEngine,
                _activator,
                new List<IRazorPage>(),
                result.Page,
                HtmlEncoder.Default,
                new DiagnosticListener("ViewRenderService"));

            var viewContext = new ViewContext(
                actionContext,
                view,
                new ViewDataDictionary<TPageModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                },
                new TempDataDictionary(
                    _httpContext.HttpContext,
                    _tempDataProvider
                ),
                sw,
                new HtmlHelperOptions()
            );

            var page = (Page)result.Page;

            page.PageContext = new PageContext
            {
                ViewData = viewContext.ViewData
            };
            page.ViewContext = viewContext;

            _activator.Activate(page, viewContext);
            await page.ExecuteAsync();
            writer?.Invoke(sw);

            return page;
        }

        public Task<Page> GetRazorPageAsync<TPageModel>(string pageName, TPageModel model, Action<StringWriter> writer = null) where TPageModel : PageModel
        {
            var httpContext = _httpContext.HttpContext ??= new DefaultHttpContext();
            var routeData = httpContext.GetRouteData();

            var actionDescriptor = _actionContext.ActionContext.ActionDescriptor;
            var actionContext =
                new ActionContext(
                    httpContext,
                    routeData,
                    actionDescriptor
                );
            return GetRazorPageAsync(pageName, model, httpContext, actionContext, writer);
        }

        public Task<Page> GetRazorPageAsync<TPageModel>(params object[] args) where TPageModel : PageModel
        {
            var pageName = typeof(TPageModel).GetPagePath();
            var model = Activator.CreateInstance(typeof(TPageModel), args) as TPageModel;

            return GetRazorPageAsync(pageName, model);
        }
    }
}