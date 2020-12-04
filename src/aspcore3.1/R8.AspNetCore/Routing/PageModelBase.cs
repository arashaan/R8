using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

using R8.Lib.Localization;
using R8.Lib.Paginator;

namespace R8.AspNetCore.Routing
{
    /// <summary>
    /// A full-fledged PageModel will optimizations for localization.
    /// </summary>
    /// <typeparam name="TSearch">A derived type of <see cref="BaseSearch"/> type.</typeparam>
    /// <typeparam name="TModel">An object to place in <see cref="Pagination"/>.</typeparam>
    public abstract class PageModelBase<TSearch, TModel> : PageModelBase<TSearch> where TSearch : BaseSearch where TModel : class
    {
        /// <summary>
        /// Gets or sets list of objects that they've already paginated.
        /// </summary>
        public Pagination<TModel> List { get; set; }
    }

    /// <summary>
    /// A full-fledged PageModel will optimizations for localization.
    /// </summary>
    /// <typeparam name="TSearch">A derived type of <see cref="BaseSearch"/> type.</typeparam>
    public abstract class PageModelBase<TSearch> : PageModelBase where TSearch : BaseSearch
    {
        /// <summary>
        /// Gets or sets an object of <see cref="BaseSearch"/> to representing search parameters.
        /// </summary>
        [BindProperty]
        public TSearch SearchInput { get; set; }

        //public IActionResult OnPost()
        //{
        //    // var form = this.Request.Form;
        //    return RedirectToPage(SearchInput.GetRouteData());
        //}

        /// <summary>
        /// Refreshes page with given search parameters.
        /// </summary>
        public RedirectToPageResult RedirectToSearch() => RedirectToPage(SearchInput.GetRouteData());
    }

    /// <summary>
    /// A full-fledged PageModel will optimizations for localization.
    /// </summary>
    public abstract class PageModelBase : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        public new ICulturalizedUrlHelper Url
        {
            get
            {
                var service = this.HttpContext.RequestServices.GetService(typeof(ICulturalizedUrlHelper));
                return (service ?? base.Url) as ICulturalizedUrlHelper;
            }
        }

        public ILocalizer Localizer
        {
            get
            {
                var service = this.HttpContext.RequestServices.GetService(typeof(ILocalizer));
                return service as ILocalizer;
            }
        }

        public RequestLocalizationOptions Culture
        {
            get
            {
                var service = this.HttpContext.RequestServices.GetService(typeof(IOptions<RequestLocalizationOptions>));
                return ((IOptions<RequestLocalizationOptions>)service)?.Value;
            }
        }

        /// <summary>
        /// Gets or sets title of <see cref="PageModelBase"/>
        /// </summary>
        [ViewData]
        public string PageTitle { get; set; }

        // public const string Query_MESSAGE = "message";
        // public const string Query_ID = "id";

        // /// <summary>
        // /// Returns value from 'message' QueryString
        // /// </summary>
        // [FromQuery(Name = Query_MESSAGE)]
        // public string Message { get; set; }

        // /// <summary>
        // /// Returns value from 'id' QueryString
        // /// </summary>
        // [FromQuery(Name = Query_ID)]
        // public string Id { get; set; }
        //
        // public bool IsNew => string.IsNullOrEmpty(Id);

        // [TempData]
        // public string SerializedModel { get; set; }

        protected string PagePath => GetType().GetPagePath();

        public override RedirectToPageResult RedirectToPage()
        {
            var thisRoutes = Request.RouteValues;
            var values = CulturalizedUrlHelper.TryAddCulture(Culture, thisRoutes, new RouteValueDictionary());
            return base.RedirectToPage(PagePath, values);
        }

        public override RedirectToPageResult RedirectToPage(object routeValues)
        {
            var thisRoutes = Request.RouteValues;
            var values = CulturalizedUrlHelper.TryAddCulture(Culture, thisRoutes, routeValues);
            return base.RedirectToPage(PagePath, values);
        }

        public RedirectToPageResult RedirectToPage<TPage>() where TPage : PageModelBase
        {
            var pageName = typeof(TPage).GetPagePath();
            var thisRoutes = Request.RouteValues;
            var values = CulturalizedUrlHelper.TryAddCulture(Culture, thisRoutes, new RouteValueDictionary());
            return base.RedirectToPage(pageName, values);
        }

        public RedirectToPageResult RedirectToPage<TPage>(object routeValues) where TPage : PageModelBase
        {
            var pageName = typeof(TPage).GetPagePath();
            var thisRoutes = Request.RouteValues;
            var values = CulturalizedUrlHelper.TryAddCulture(Culture, thisRoutes, routeValues);
            return base.RedirectToPage(pageName, values);
        }

        public override RedirectToPageResult RedirectToPage(string pageName)
        {
            var thisRoutes = Request.RouteValues;
            var values = CulturalizedUrlHelper.TryAddCulture(Culture, thisRoutes, new RouteValueDictionary());
            return base.RedirectToPage(pageName, values);
        }

        public override RedirectToPageResult RedirectToPage(string pageName, object routeValues)
        {
            var thisRoutes = Request.RouteValues;
            var values = CulturalizedUrlHelper.TryAddCulture(Culture, thisRoutes, routeValues);
            return base.RedirectToPage(pageName, values);
        }

        //
        // public T StoreOrPull<T>(string key, T obj) where T : class
        // {
        //     var deserialized = Pull<T>(key);
        //     if (deserialized != null)
        //         return deserialized;
        //
        //     Store(key, obj);
        //     return obj;
        // }

        // private static JsonSerializerSettings SerializerSetting => new JsonSerializerSettings
        // {
        //     Error = Lib.JsonExtensions.CustomJsonSerializerSettings.Settings.Error,
        //     DefaultValueHandling = Lib.JsonExtensions.CustomJsonSerializerSettings.Settings.DefaultValueHandling,
        //     ReferenceLoopHandling = Lib.JsonExtensions.CustomJsonSerializerSettings.Settings.ReferenceLoopHandling,
        //     ObjectCreationHandling = Lib.JsonExtensions.CustomJsonSerializerSettings.Settings.ObjectCreationHandling,
        //     ContractResolver = Lib.JsonExtensions.CustomJsonSerializerSettings.Settings.ContractResolver,
        //     Formatting = Lib.JsonExtensions.CustomJsonSerializerSettings.Settings.Formatting,
        //     PreserveReferencesHandling = PreserveReferencesHandling.Objects
        // };
        //
        // public string Store<T>(string key, T value) where T : class
        // {
        //     var serialized = JsonConvert.SerializeObject(value, SerializerSetting);
        //     this.TempData[key] = serialized;
        //     return serialized;
        // }
        //
        // public T Pull<T>(string key) where T : class
        // {
        //     var o = this.TempData.Peek(key);
        //     var deserialized = o == null
        //         ? null
        //         : JsonConvert.DeserializeObject<T>((string)o, SerializerSetting);
        //     return deserialized;
        // }
        //
        // protected virtual async Task<IActionResult> OnGetAsync<TSearch, TModel>(string json, Func<TSearch, Task<Pagination<TModel>>> getList, Type pageComponent) where TSearch : BaseSearch where TModel : class
        // {
        //     var model = Activator.CreateInstance<TSearch>();
        //     try
        //     {
        //         foreach (var (key, valueToken) in JObject.Parse(json))
        //         {
        //             var propertyInfo = model
        //               .GetType()
        //               .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        //               .FirstOrDefault(x => x.GetCustomAttribute<FromQueryAttribute>()?.Name == key);
        //             if (propertyInfo == null)
        //                 continue;
        //
        //             var _value = valueToken.Value<object>();
        //             var value = Convert.ChangeType(_value, propertyInfo.PropertyType.GetTypeInfo());
        //             model[propertyInfo.Name] = value;
        //         }
        //         // TODO fix serialized
        //         var list = await getList.Invoke(model);
        //
        //         return new ViewComponentResult
        //         {
        //             ViewComponentType = pageComponent,
        //             Arguments = new
        //             {
        //                 models = list.Items
        //             }
        //         };
        //     }
        //     catch
        //     {
        //         this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //         return new EmptyResult();
        //     }
        // }
        // //
        // public virtual TInputModel InitializeInput<TInputModel>(TInputModel model)
        //     where TInputModel : BaseInputModel<TInputModel>
        // {
        //     var fromMemory = false;
        //     TInputModel finalModel;
        //     TInputModel deserializeModel = null;
        //     try
        //     {
        //         if (!string.IsNullOrEmpty(this.SerializedModel))
        //         {
        //             deserializeModel = JsonConvert.DeserializeObject<TInputModel>(this.SerializedModel);
        //             if (deserializeModel != null
        //                 && !string.IsNullOrEmpty(deserializeModel.Id)
        //                 && deserializeModel.Id.Equals(Id, StringComparison.InvariantCulture)
        //                 && this.Request.Method == HttpMethod.Post.Method)
        //             {
        //                 fromMemory = true;
        //             }
        //         }
        //     }
        //     finally
        //     {
        //         finalModel = (fromMemory ? deserializeModel : model)
        //                      ?? Activator.CreateInstance<TInputModel>();
        //     }
        //
        //     return finalModel;
        // }
    }
}