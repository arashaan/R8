using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using R8.Lib.Paginator;

namespace R8.AspNetCore.Routing
{
    public static class PaginationComponentExtensions
    {
        /// <summary>
        /// Returns a view component result for Pagination view component.
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="component"></param>
        /// <param name="viewUrl"></param>
        /// <param name="paginationPropertyName"></param>
        /// <param name="pageNoQueryName"></param>
        /// <remarks>Output model is a collection of type <see cref="PaginationPageModel"/>.</remarks>
        public static IViewComponentResult InvokePagination<TComponent>(this TComponent component, string viewUrl, string paginationPropertyName, string pageNoQueryName = "pageNo") where TComponent : ViewComponent
        {
            var viewDataModel = component.ViewContext.ViewData.Model;
            if (viewDataModel == null)
                throw new NullReferenceException($"Cannot find a working {nameof(component.ViewData)} model from {nameof(component.ViewContext)}.");

            if (!(viewDataModel is PageModel pageModel))
                throw new NullReferenceException($"Cannot recognize {nameof(component.ViewData)} model type. Given type should be a derived class of {nameof(PageModel)}. and/or {nameof(PageModel)} itself.");

            var paginationListProp = pageModel
                .GetType()
                .GetProperty(paginationPropertyName);
            if (paginationListProp == null)
                throw new NullReferenceException($"Cannot find a property with given name => '{paginationPropertyName}'.");

            if (!(paginationListProp.GetValue(viewDataModel) is IPagination pagination))
                throw new Exception($"Given property type does not respect {typeof(IPagination)} type.");

            var currentUrlData = component.ViewContext.RouteData;
            if (currentUrlData?.Values == null
                || currentUrlData.Values.Count == 0
                || currentUrlData.Values.Values.Count == 0)
            {
                throw new NullReferenceException($"Cannot find expected route values from {nameof(component.ViewContext)}.");
            }

            var currentUrlIsPage = currentUrlData.Values.Keys.FirstOrDefault() == "page";
            var currentUrlRoutes = currentUrlData.Values.Values;

            var model = new List<PaginationPageModel>();

            var routeTemplate = new Dictionary<string, string>();
            var queryString = component.ViewContext.HttpContext.Request.QueryString.Value;
            if (!string.IsNullOrEmpty(queryString))
            {
                queryString = queryString.StartsWith("?") ? queryString[1..] : queryString;
                if (!string.IsNullOrEmpty(queryString))
                {
                    routeTemplate = queryString
                        .Split("&")
                        .Where(x => x.Split("=")[0] != pageNoQueryName)
                        .ToDictionary(x => x.Split("=")[0], x => HttpUtility.UrlDecode(x.Split("=")[1]));
                }
            }

            for (var x = 0; x < pagination.Pages; x++)
            {
                var page = x + 1;
                var routes = new Dictionary<string, string>(routeTemplate) { { pageNoQueryName, page.ToString() } };

                string currentUrl;
                if (currentUrlIsPage)
                {
                    var str = currentUrlRoutes.FirstOrDefault()?.ToString();
                    currentUrl = component.Url.Page(str, routes);
                }
                else
                {
                    var arr = currentUrlRoutes.Take(2).Cast<string>().ToArray();
                    currentUrl = component.Url.Action(arr[1], arr[0], routes);
                }

                model.Add(new PaginationPageModel
                {
                    Num = page,
                    IsCurrent = page == pagination.CurrentPage,
                    Link = currentUrl,
                });
            }

            var viewData = new ViewDataDictionary<List<PaginationPageModel>>(component.ViewData, model);
            var result = new ViewViewComponentResult()
            {
                ViewData = viewData,
                TempData = component.TempData,
                ViewEngine = component.ViewEngine,
                ViewName = viewUrl
            };
            return result;
        }
    }
}