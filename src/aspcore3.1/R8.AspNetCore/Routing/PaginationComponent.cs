using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using R8.Lib.Paginator;

namespace R8.AspNetCore.Routing
{
    /// <summary>
    /// Initializes a <see cref="PaginationComponentBase"/> instance of type <see cref="ViewComponent"/>.
    /// </summary>
    /// <remarks>Output model is a collection of type <see cref="PaginationPageModel"/>.</remarks>
    [ViewComponent(Name = "Pagination")]
    public abstract class PaginationComponentBase : ViewComponent
    {
        private readonly string _viewUrl;
        private readonly string _paginationPropertyName;
        private readonly string _pageNoQueryName;

        protected PaginationComponentBase(string viewUrl, string paginationPropertyName, string pageNoQueryName = "pageNo")
        {
            _viewUrl = viewUrl;
            _paginationPropertyName = paginationPropertyName;
            _pageNoQueryName = pageNoQueryName;
        }

        public virtual IViewComponentResult Invoke()
        {
            var model = ViewContext.ViewData.Model;
            if (model == null)
                throw new NullReferenceException($"Cannot find a working {nameof(ViewData)} model from {nameof(ViewContext)}.");

            if (!(model is PageModel pageModel))
                throw new NullReferenceException($"Cannot recognize {nameof(ViewData)} model type. Given type should be a derived class of {nameof(PageModel)}. and/or {nameof(PageModel)} itself.");

            var paginationListProp = pageModel
                .GetType()
                .GetProperty(_paginationPropertyName);
            if (paginationListProp == null)
                throw new NullReferenceException($"Cannot find a property with given name => '{_paginationPropertyName}'.");

            if (!(paginationListProp.GetValue(model) is IPagination pagination))
                throw new Exception($"Given property type does not respect {typeof(IPagination)} type.");

            var currentUrlData = ViewContext.RouteData;
            if (currentUrlData?.Values == null
                || currentUrlData.Values.Count == 0
                || currentUrlData.Values.Values.Count == 0)
            {
                throw new NullReferenceException($"Cannot find expected route values from {nameof(ViewContext)}.");
            }

            var currentUrlIsPage = currentUrlData.Values.Keys.FirstOrDefault() == "page";
            var currentUrlRoutes = currentUrlData.Values.Values;

            var final = new List<PaginationPageModel>();

            var routeTemplate = new Dictionary<string, string>();
            var queryString = ViewContext.HttpContext.Request.QueryString.Value;
            if (!string.IsNullOrEmpty(queryString))
            {
                queryString = queryString.StartsWith("?") ? queryString[1..] : queryString;
                if (!string.IsNullOrEmpty(queryString))
                {
                    routeTemplate = queryString
                        .Split("&")
                        .Where(x => x.Split("=")[0] != _pageNoQueryName)
                        .ToDictionary(x => x.Split("=")[0], x => HttpUtility.UrlDecode(x.Split("=")[1]));
                }
            }

            for (var x = 0; x < pagination.Pages; x++)
            {
                var page = x + 1;
                var routes = new Dictionary<string, string>(routeTemplate) { { _pageNoQueryName, page.ToString() } };

                string currentUrl;
                if (currentUrlIsPage)
                {
                    var str = currentUrlRoutes.FirstOrDefault()?.ToString();
                    currentUrl = Url.Page(str, routes);
                }
                else
                {
                    var arr = currentUrlRoutes.Take(2).Cast<string>().ToArray();
                    currentUrl = Url.Action(arr[1], arr[0], routes);
                }

                final.Add(new PaginationPageModel
                {
                    Num = page,
                    IsCurrent = page == pagination.CurrentPage,
                    Link = currentUrl,
                });
            }

            return View(_viewUrl, final);
        }
    }
}