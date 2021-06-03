using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <param name="pageNoIdentifier"></param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <remarks>Output model is a collection of type <see cref="PaginationPageModel"/>.</remarks>
        public static IViewComponentResult InvokePagination<TComponent>(this TComponent component, string viewUrl, string paginationPropertyName = "List", string pageNoIdentifier = "pageNo") where TComponent : ViewComponent
        {
            var pageModel = (Microsoft.AspNetCore.Mvc.RazorPages.PageModel)component.ViewContext.ViewData.Model;
            var paginationListProp = pageModel.GetType().GetProperty(paginationPropertyName);
            if (paginationListProp == null)
                return new ContentViewComponentResult(string.Empty);

            var paginationList = paginationListProp.GetValue(pageModel);
            if (!(paginationList is IPagination pagination))
                return new ContentViewComponentResult(string.Empty);

            var currentUrlData = component.ViewContext.RouteData;
            if (currentUrlData?.Values == null
                || currentUrlData.Values.Count == 0
                || currentUrlData.Values.Values.Count == 0)
            {
                throw new NullReferenceException($"Cannot find expected route values from {nameof(component.ViewContext)}.");
            }

            var routeTemplate = new Dictionary<string, object>();
            var model = new List<PaginationPageModel>();
            for (var x = 0; x < pagination.Pages; x++)
            {
                var pageNo = x + 1;
                var routes = new Dictionary<string, object>(routeTemplate) { { pageNoIdentifier, pageNo } };

                string currentUrl;
                if (currentUrlData.Values.ContainsKey("page"))
                {
                    var page = currentUrlData.Values["page"].ToString();
                    currentUrl = component.Url.PageLocalized(page, routes);
                }
                else
                {
                    var arr = currentUrlData.Values.Take(2).Cast<string>().ToList();
                    var controller = arr[0];
                    var action = arr[1];
                    currentUrl = component.Url.ActionLocalized(action, controller, routes);
                }

                model.Add(new PaginationPageModel(pageNo, pageNo == pagination.CurrentPage, currentUrl));
            }

            var viewData = new ViewDataDictionary<List<PaginationPageModel>>(component.ViewData, model);
            return new ViewViewComponentResult
            {
                ViewData = viewData,
                TempData = component.TempData,
                ViewEngine = component.ViewEngine,
                ViewName = viewUrl
            };
        }
    }
}