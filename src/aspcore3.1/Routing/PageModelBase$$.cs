using R8.Lib.Paginator;
using R8.Lib.Search;

namespace R8.AspNetCore.Routing
{
    /// <summary>
    /// A full-fledged PageModel will optimizations for localization.
    /// </summary>
    /// <typeparam name="TSearch">A derived type of <see cref="BaseSearch"/> type.</typeparam>
    /// <typeparam name="TModel">An object to place in <see cref="Pagination"/>.</typeparam>
    public abstract class PageModelBase<TSearch, TModel> : PageModelBase<TSearch>
        where TSearch : SearchBase where TModel : class
    {
        /// <summary>
        /// Gets or sets list of objects that they've already paginated.
        /// </summary>
        public Pagination<TModel> List { get; set; }
    }
}