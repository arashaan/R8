namespace R8.AspNetCore3_1.Demo.Services.Routing
{
    //public class Controller : Microsoft.AspNetCore.Mvc.Controller
    //{
    //    [NonAction]
    //    public virtual ActionResult RedirectToForwardedPage(string fallback = "/Index")
    //    {
    //        var request = HttpContext.Request;
    //        var refererUrl = request.Headers[HeaderNames.Referer].ToString();
    //        var uri = string.IsNullOrEmpty(refererUrl)
    //            ? PageHandlers.GetUrlUri(fallback)
    //            : PageHandlers.GetUrlUri(refererUrl);

    //        var query = new QueryString(uri.Query);
    //        var finalUrl = $"{uri.AbsolutePath}{query}";
    //        return new RedirectResult(finalUrl);
    //    }

    //    public RouteValueDictionary GetRouteValuesFromUrl(string url)
    //    {
    //        if (url == null)
    //            return new RouteValueDictionary();

    //        var routes = this.RouteData.Routers.OfType<Route>();

    //        var route = routes?.FirstOrDefault(p => p.Name == "page");
    //        if (route == null)
    //            return new RouteValueDictionary();

    //        var template = route.ParsedTemplate;
    //        var matcher = new TemplateMatcher(template, route.Defaults);
    //        var routeValues = new RouteValueDictionary();
    //        var localPath = (new Uri(url)).LocalPath;
    //        if (!matcher.TryMatch(localPath, routeValues))
    //            throw new Exception("Could not identity controller and action");
    //        return routeValues;
    //    }

    //    [NonAction]
    //    public virtual ActionResult RedirectToForwardedPage<TInput, TResponse>(TInput inputModel, TResponse response,
    //        string fallback = "/Index") where TInput : ValidatableObject where TResponse : Response
    //    {
    //        var request = this.HttpContext.Request;
    //        var refererUrl = request.Headers[HeaderNames.Referer].ToString();
    //        var uri = string.IsNullOrEmpty(refererUrl)
    //            ? PageHandlers.GetUrlUri(fallback)
    //            : PageHandlers.GetUrlUri(refererUrl);

    //        if (response == null)
    //            return new RedirectToPageResult(uri.PathAndQuery);
    //        var newRoutes = new Dictionary<string, object>
    //        {
    //            { PageModel.Query_STATUS, (int)response.Status }
    //        };

    //        if (inputModel != null)
    //        {
    //            inputModel.Validate();
    //            if (inputModel.ValidationErrors?.Any() == true)
    //            {
    //                var errorsHtml = inputModel.ToQueryString();
    //                newRoutes.Add(PageModel.Query_MESSAGE, errorsHtml);
    //            }
    //        }

    //        var routes = uri.Combine(newRoutes);
    //        var finalUrl = $"{uri.AbsolutePath}{routes.QueryBuilder.ToQueryString()}";
    //        return new RedirectResult(finalUrl);
    //    }

    //    [NonAction]
    //    public virtual ActionResult RedirectToForwardedPage<TResponse>(TResponse response, string fallback = "/Index")
    //        where TResponse : Response
    //    {
    //        var request = HttpContext.Request;
    //        var refererUrl = request.Headers[HeaderNames.Referer].ToString();
    //        var uri = string.IsNullOrEmpty(refererUrl)
    //            ? PageHandlers.GetUrlUri(fallback)
    //            : PageHandlers.GetUrlUri(refererUrl);

    //        if (response == null)
    //            return new RedirectToPageResult(uri.PathAndQuery);
    //        var newRoutes = new Dictionary<string, object>
    //        {
    //            { PageModel.Query_STATUS, (int)response.Status },
    //            { PageModel.Query_MESSAGE, response.Message },
    //        };

    //        var routes = uri.Combine(newRoutes);
    //        var finalUrl = $"{uri.AbsolutePath}{routes.QueryBuilder.ToQueryString()}";
    //        return new RedirectResult(finalUrl);
    //    }
    //}
}