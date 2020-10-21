using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using R8.Lib.AspNetCore.Base;
using R8.Lib.Enums;
using R8.Lib.MethodReturn;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace R8.Lib.AspNetCore.Routing
{
    public class PageModel<TSearch, TModel> : PageModel<TSearch> where TSearch : BaseSearchModel where TModel : class
    {
        public Pagination<TModel> List { get; set; }
    }

    public class PageModel<TSearch> : PageModel where TSearch : BaseSearchModel
    {
        [BindProperty]
        public TSearch SearchInput { get; set; }

        public IActionResult OnPost()
        {
            var form = this.Request.Form;
            return RedirectToPage(SearchInput.GetRouteData());
        }
    }

    public class PageModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        //private readonly ILocalizer _localizer;

        //public PageModel(ILocalizer localizer)
        //{
        //    _localizer = localizer;
        //}

        public new ICulturalizedUrlHelper Url;
        public readonly ILocalizer Localizer;

        public IOptions<RequestLocalizationOptions> Culture =>
            HttpContext.RequestServices.GetService(typeof(IOptions<RequestLocalizationOptions>)) as
                IOptions<RequestLocalizationOptions>;

        public PageModel([FromServices] ICulturalizedUrlHelper page = null, [FromServices] ILocalizer localizer = null)
        {
            Url = page;
            Localizer = localizer;
        }

        /// <summary>
        /// Gets or sets title of <see cref="PageModel"/>
        /// </summary>
        [ViewData]
        public string PageTitle { get; set; }

        public const string Query_STATUS = "status";
        public const string Query_MESSAGE = "message";
        public const string Query_ID = "id";
        public const string Query_CALLBACK = "callback";

        /// <summary>
        /// Returns value from 'message' QueryString
        /// </summary>
        [FromQuery(Name = Query_MESSAGE)]
        public string Message { get; set; }

        /// <summary>
        /// Returns <see cref="Flags"/> from 'status' QueryString
        /// </summary>
        [FromQuery(Name = Query_STATUS)]
        public Flags? Status { get; set; }

        /// <summary>
        /// Returns value from 'id' QueryString
        /// </summary>
        [FromQuery(Name = Query_ID)]
        public string Id { get; set; }

        /// <summary>
        /// Initial string of 'callback' QueryString
        /// </summary>
        [FromQuery(Name = Query_CALLBACK)]
        public string CallbackUrl { get; set; }

        /// <summary>
        /// Returns value from 'callback' QueryString
        /// </summary>
        public string GetCallbackUrl()
        {
            return GetCallbackUrl(CallbackUrl);
        }

        public bool IsNew => string.IsNullOrEmpty(Id);

        [TempData]
        private string SerializedModel { get; set; }

        protected string PagePath => GetType().GetPagePath();

        protected virtual string GetCallbackUrl(string callback)
        {
            if (string.IsNullOrEmpty(callback) || !base.Url.IsLocalUrl(callback))
                return callback;

            var uri = new Uri($"{HttpContext.GetBaseUrl()}{callback}");
            var queries = uri.GetQueryStrings();
            if (queries?.Any() != true)
                return null;

            var status = int.TryParse(queries
              .FirstOrDefault(x => x.Key.Equals(Query_STATUS, StringComparison.InvariantCultureIgnoreCase)).Value, out var statusId);
            var message = queries
              .FirstOrDefault(x => x.Key.Equals(Query_MESSAGE, StringComparison.InvariantCultureIgnoreCase)).Value;

            if (status && Enum.TryParse(statusId.ToString(), out Flags sts))
                Status = sts;

            if (message != null)
                Message = WebUtility.UrlDecode(message);

            var result = callback.Split("?")[0];
            return result;
        }

        protected virtual async Task<IActionResult> HandleAsync<TEntity, TInputModel>(TInputModel model,
          Func<Task<Response<TEntity>>> action, Func<Response<TEntity>, IActionResult> redirect)
          where TEntity : class where TInputModel : ValidatableObject
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var methodStatus = await action.Invoke().ConfigureAwait(false);
            var core = this.RedirectCore(model, methodStatus);

            var url = redirect.Invoke(methodStatus);
            if (!(url is RedirectToPageResult pageUrl))
                return url;

            var finalUrl = pageUrl.Combine(core.RouteData);
            return finalUrl;
        }

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

        public RedirectToPageResult RedirectToPage<TPage>() where TPage : PageModel
        {
            var pageName = typeof(TPage).GetPagePath();
            var thisRoutes = Request.RouteValues;
            var values = CulturalizedUrlHelper.TryAddCulture(Culture, thisRoutes, new RouteValueDictionary());
            return base.RedirectToPage(pageName, values);
        }

        public RedirectToPageResult RedirectToPage<TPage>(object routeValues) where TPage : PageModel
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

        public T StoreOrPull<T>(string key, T obj) where T : class
        {
            var deserialized = Pull<T>(key);
            if (deserialized != null)
                return deserialized;

            Store(key, obj);
            return obj;
        }

        private static JsonSerializerSettings SerializerSetting => new JsonSerializerSettings
        {
            Error = JsonSettingsExtensions.JsonNetSettings.Error,
            DefaultValueHandling = JsonSettingsExtensions.JsonNetSettings.DefaultValueHandling,
            ReferenceLoopHandling = JsonSettingsExtensions.JsonNetSettings.ReferenceLoopHandling,
            ObjectCreationHandling = JsonSettingsExtensions.JsonNetSettings.ObjectCreationHandling,
            ContractResolver = JsonSettingsExtensions.JsonNetSettings.ContractResolver,
            Formatting = JsonSettingsExtensions.JsonNetSettings.Formatting,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        };

        public string Store<T>(string key, T value) where T : class
        {
            var serialized = JsonConvert.SerializeObject(value, SerializerSetting);
            this.TempData[key] = serialized;
            return serialized;
        }

        public T Pull<T>(string key) where T : class
        {
            var o = this.TempData.Peek(key);
            var deserialized = o == null
                ? null
                : JsonConvert.DeserializeObject<T>((string)o, SerializerSetting);
            return deserialized;
        }

        protected virtual Task<IActionResult> HandleAsync<TEntity, TInputModel>(TInputModel model,
          Func<Task<Response<TEntity>>> action)
          where TEntity : class where TInputModel : ValidatableObject
        {
            return this.HandleAsync(model, action, _ => RedirectToPage(PagePath));
        }

        private class HandlerResponse
        {
            public Dictionary<string, object> RouteData { get; set; }
            public bool IsNew { get; set; }
            public Flags Status { get; set; }
            public string Message { get; set; }

            #region Overrides of Object

            /// <summary>Returns a string that represents the current object.</summary>
            /// <returns>A string that represents the current object.</returns>
            public override string ToString()
            {
                return RouteData?.Any() == true
                  ? $"?{string.Join("&", RouteData.Select(x => $"{x.Key}={WebUtility.UrlDecode(x.Value.ToString())}").ToList())}"
                  : Message;
            }

            #endregion Overrides of Object
        }

        /// <summary>
        /// An automated API to handle OnPost requests asynchronously
        /// </summary>
        /// <typeparam name="TEntity">Type of Entity that will returns after running <paramref name="action"/> —Based on <see cref="EntityBase"/></typeparam>
        /// <typeparam name="TInputModel">Type of Input model based on <see cref="BaseInputModel"/></typeparam>
        /// <param name="model">Input model that supposed to push to API</param>
        /// <param name="action">API action that returns <see cref="Response{TSource}"/></param>
        /// <param name="redirectOnSuccess">Page url to go after success</param>
        /// <param name="redirectOnFailure">Page url to go after failure</param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> HandleAsync<TEntity, TInputModel>(TInputModel model,
          Func<Task<Response<TEntity>>> action,
          Func<Response<TEntity>, IActionResult> redirectOnSuccess,
          Func<Response<TEntity>, IActionResult> redirectOnFailure) where TEntity : class where TInputModel : ValidatableObject
        {
            if (action == null)
                return this.Page();

            var methodStatus = await action.Invoke().ConfigureAwait(false);
            var core = this.RedirectCore(model, methodStatus);

            var targetResult = core.Status != Flags.Success
              ? redirectOnFailure.Invoke(methodStatus)
              : redirectOnSuccess.Invoke(methodStatus);

            if (!(targetResult is RedirectToPageResult pageResult))
                throw new NullReferenceException(nameof(pageResult));

            var result = pageResult.Combine(core.RouteData);
            return result;
        }

        //public static Dictionary<string, object> FixRoute<TInput>(Response response, TInput model) where TInput : BaseInputModel<TInput>
        //{
        //    var isValid = model.Validate();
        //    string message;
        //    if (isValid)
        //    {
        //        message = response.Message;
        //    }
        //    else
        //    {
        //        response = AspNetCore.Base.Response.FromFlags(Flags.RetryAfterReview);
        //        message = model?.ValidationErrors?.Any() == true
        //            ? model.ToQueryString()
        //            : response.Message;
        //    }

        //    var routeData = new Dictionary<string, object>
        //    {
        //        {Query_STATUS, (int) response.Status},
        //        {Query_MESSAGE, WebUtility.UrlEncode(message)}
        //    };

        //    return routeData;
        //}

        /// <summary>
        /// An internal API to handle OnPost request
        /// </summary>
        /// <typeparam name="TEntity">Type of Entity that will returns after running <paramref name="action"/> —Based on <see cref="EntityBase"/></typeparam>
        /// <typeparam name="TInputModel">Type of Input model based on <see cref="BaseInputModel"/></typeparam>
        /// <param name="model">Input model that supposed to push to API</param>
        /// <param name="responseWrapper">API action that returns <see cref="Response{TSource}"/></param>
        private HandlerResponse RedirectCore<TEntity, TInputModel>(TInputModel model,
          Response<TEntity> responseWrapper)
          where TEntity : class where TInputModel : ValidatableObject
        {
            var (actionStatus, entity) = responseWrapper
                                         ?? throw new ArgumentNullException($"{nameof(responseWrapper)} must be filled");
            var isValid = model.Validate();

            BaseInputModel<TInputModel> baseInputModel;
            var isNewProp = model.GetType().GetPublicProperties()
              .Find(x => x.Name.Equals(nameof(baseInputModel.IsNew), StringComparison.Ordinal));
            var isNew = isNewProp != null && (bool)isNewProp.GetValue(model);

            string message;
            Response response;
            if (isValid)
            {
                response = new Response(actionStatus);
                message = response.Message;
            }
            else
            {
                response = new Response(Flags.RetryAfterReview);
                message = model?.ValidationErrors?.Any() == true
                  ? model.ToQueryString()
                  : response.Message;
            }

            var routeData = new Dictionary<string, object>
            {
                {Query_STATUS, (int) response.Status},
                {Query_MESSAGE, WebUtility.UrlEncode(message)}
            };
            SerializedModel = JsonConvert.SerializeObject(model);

            return new HandlerResponse
            {
                IsNew = isNew,
                RouteData = routeData,
                Status = response.Status,
                Message = response.Message
            };
        }

        protected virtual async Task<IActionResult> OnGetAsync<TSearch, TModel>(string json, Func<TSearch, Task<Pagination<TModel>>> getList, Type pageComponent) where TSearch : BaseSearchModel where TModel : class
        {
            var model = Activator.CreateInstance<TSearch>();
            try
            {
                foreach (var (key, valueToken) in JObject.Parse(json))
                {
                    var propertyInfo = model
                      .GetType()
                      .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                      .FirstOrDefault(x => x.GetCustomAttribute<FromQueryAttribute>()?.Name == key);
                    if (propertyInfo == null)
                        continue;

                    var _value = valueToken.Value<object>();
                    var value = Convert.ChangeType(_value, propertyInfo.PropertyType.GetTypeInfo());
                    model[propertyInfo.Name] = value;
                }
                // TODO fix serialized
                var list = await getList.Invoke(model);

                return new ViewComponentResult
                {
                    ViewComponentType = pageComponent,
                    Arguments = new
                    {
                        models = list.Items
                    }
                };
            }
            catch
            {
                this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new EmptyResult();
            }
        }

        public virtual TInputModel InitializeInput<TInputModel>(TInputModel model)
            where TInputModel : BaseInputModel<TInputModel>
        {
            var fromMemory = false;
            TInputModel finalModel;
            TInputModel deserializeModel = null;
            try
            {
                if (!string.IsNullOrEmpty(this.SerializedModel))
                {
                    deserializeModel = JsonConvert.DeserializeObject<TInputModel>(this.SerializedModel);
                    if (deserializeModel != null
                        && !string.IsNullOrEmpty(deserializeModel.Id)
                        && deserializeModel.Id.Equals(Id, StringComparison.InvariantCulture)
                        && this.Request.Method == HttpMethod.Post.Method)
                    {
                        fromMemory = true;
                    }
                }
            }
            finally
            {
                finalModel = (fromMemory ? deserializeModel : model)
                             ?? Activator.CreateInstance<TInputModel>();
            }

            return finalModel;
        }
    }
}