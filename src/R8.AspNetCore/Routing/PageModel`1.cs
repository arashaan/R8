using Microsoft.AspNetCore.Mvc;

using R8.Lib.Search;

namespace R8.AspNetCore.Routing
{
    /// <summary>
    /// A full-fledged PageModel will optimizations for localization.
    /// </summary>
    /// <typeparam name="TSearch">A derived type of <see cref="BaseSearch"/> type.</typeparam>
    public abstract class PageModel<TSearch> : PageModel where TSearch : SearchBase
    {
        /// <summary>
        /// Gets or sets an object of <see cref="BaseSearch"/> to representing search parameters.
        /// </summary>
        [BindProperty]
        public TSearch SearchInput { get; set; }

        //public IActionResult OnPost()
        //{
        //    // var form = this.Request.Form;
        //    return RedirectToPageLocalized(SearchInput.GetRouteData());
        //}

        /// <summary>
        /// Refreshes page with given search parameters.
        /// </summary>
        public RedirectToPageResult RedirectToSearch() => this.RedirectToPageLocalized(SearchInput.GetRouteData());
    }
}