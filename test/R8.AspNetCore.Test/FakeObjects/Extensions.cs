using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

using Moq;

using System;
using System.IO;

namespace R8.AspNetCore.Test.FakeObjects
{
    public static class Extensions
    {
        public static ActionContext GetFakeActionContext(this IServiceProvider serviceProvider)
        {
            var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };
            var routeData = httpContext.GetRouteData();
            var actionDescriptor = new ActionDescriptor();

            return new ActionContext(httpContext, routeData, actionDescriptor);
        }

        public static ViewContext GetFakeViewContext(this IServiceProvider serviceProvider, ITempDataProvider tempDataProvider, ActionContext actionContext = null, TextWriter writer = null)
        {
            actionContext ??= serviceProvider.GetFakeActionContext();
            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            var tempData = new TempDataDictionary(actionContext.HttpContext, tempDataProvider);

            var viewContext = new ViewContext(
                actionContext,
                NullView.Instance,
                viewData,
                tempData,
                writer ?? TextWriter.Null,
                new HtmlHelperOptions());

            return viewContext;
        }

        public static void GetPageRequirements(out PageContext pageContext, out TempDataDictionary tempData, out UrlHelper urlHelper)
        {
            var httpContext = new DefaultHttpContext();
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            urlHelper = new UrlHelper(actionContext);
            tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };
        }
    }
}