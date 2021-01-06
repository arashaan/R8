using Microsoft.AspNetCore.Mvc;
using R8.AspNetCore.Routing;
using R8.AspNetCore3_1.Demo.Services.Enums;
using R8.Lib.Search;

namespace R8.AspNetCore3_1.Demo.Services.Routing
{
    public class PageModel<TSearch, TModel> : PageModelBase<TSearch, TModel> where TSearch : SearchBase where TModel : class
    {
    }

    public class PageModel<TSearch> : PageModelBase<TSearch> where TSearch : SearchBase
    {
    }

    public class PageModel : PageModelBase
    {
        public const string Query_STATUS = "status";

        // public const string Query_CALLBACK = "callback";
        public const string Query_MESSAGE = "message";

        /// <summary>
        /// Returns <see cref="Flags"/> from 'status' QueryString
        /// </summary>
        [FromQuery(Name = Query_STATUS)]
        public Flags? Status { get; set; }

        /// <summary>
        /// Returns value from 'message' QueryString
        /// </summary>
        [FromQuery(Name = Query_MESSAGE)]
        public string Message { get; set; }

        // /// <summary>
        // /// Initial string of 'callback' QueryString
        // /// </summary>
        // [FromQuery(Name = Query_CALLBACK)]
        // public string CallbackUrl { get; set; }
        //
        // /// <summary>
        // /// Returns value from 'callback' QueryString
        // /// </summary>
        // public string GetCallbackUrl()
        // {
        //     return GetCallbackUrl(CallbackUrl);
        // }
        //
        // protected virtual string GetCallbackUrl(string callback)
        // {
        //     if (string.IsNullOrEmpty(callback) || !base.Url.IsLocalUrl(callback))
        //         return callback;
        //
        //     var uri = new Uri($"{HttpContext.GetBaseUrl()}{callback}");
        //     var queries = uri.GetQueryStrings();
        //     if (queries?.Any() != true)
        //         return null;
        //
        //     var status = int.TryParse(queries
        //       .FirstOrDefault(x => x.Key.Equals(Query_STATUS, StringComparison.InvariantCultureIgnoreCase)).Value, out var statusId);
        //     var message = queries
        //       .FirstOrDefault(x => x.Key.Equals(Query_MESSAGE, StringComparison.InvariantCultureIgnoreCase)).Value;
        //
        //     if (status && Enum.TryParse(statusId.ToString(), out Flags sts))
        //         Status = sts;
        //
        //     if (message != null)
        //         Message = WebUtility.UrlDecode(message);
        //
        //     var result = callback.Split("?")[0];
        //     return result;
        // }
        //
        // protected virtual async Task<IActionResult> HandleAsync<TEntity, TInputModel>(TInputModel model,
        //   Func<Task<Response<TEntity>>> action, Func<Response<TEntity>, IActionResult> redirect)
        //   where TEntity : class where TInputModel : ValidatableObject
        // {
        //     if (action == null)
        //         throw new ArgumentNullException(nameof(action));
        //
        //     var methodStatus = await action.Invoke().ConfigureAwait(false);
        //     var core = this.RedirectCore(model, methodStatus);
        //
        //     var url = redirect.Invoke(methodStatus);
        //     if (!(url is RedirectToPageResult pageUrl))
        //         return url;
        //
        //     var finalUrl = pageUrl.Combine(core.RouteData);
        //     return finalUrl;
        // }
        //
        // protected virtual Task<IActionResult> HandleAsync<TEntity, TInputModel>(TInputModel model,
        //   Func<Task<Response<TEntity>>> action)
        //   where TEntity : class where TInputModel : ValidatableObject
        // {
        //     return this.HandleAsync(model, action, _ => RedirectToPage(PagePath));
        // }
        //
        // /// <summary>
        // /// An automated API to handle OnPost requests asynchronously
        // /// </summary>
        // /// <typeparam name="TEntity">Type of Entity that will returns after running <paramref name="action"/> —Based on <see cref="EntityBase"/></typeparam>
        // /// <typeparam name="TInputModel">Type of Input model based on <see cref="InputBase"/></typeparam>
        // /// <param name="model">Input model that supposed to push to API</param>
        // /// <param name="action">API action that returns <see cref="Response{TSource}"/></param>
        // /// <param name="redirectOnSuccess">Page url to go after success</param>
        // /// <param name="redirectOnFailure">Page url to go after failure</param>
        // /// <returns></returns>
        // protected virtual async Task<IActionResult> HandleAsync<TEntity, TInputModel>(TInputModel model,
        //   Func<Task<Response<TEntity>>> action,
        //   Func<Response<TEntity>, IActionResult> redirectOnSuccess,
        //   Func<Response<TEntity>, IActionResult> redirectOnFailure) where TEntity : class where TInputModel : ValidatableObject
        // {
        //     if (action == null)
        //         return this.Page();
        //
        //     var methodStatus = await action.Invoke().ConfigureAwait(false);
        //     var core = this.RedirectCore(model, methodStatus);
        //
        //     var targetResult = core.Status != Flags.Success
        //       ? redirectOnFailure.Invoke(methodStatus)
        //       : redirectOnSuccess.Invoke(methodStatus);
        //
        //     if (!(targetResult is RedirectToPageResult pageResult))
        //         throw new NullReferenceException(nameof(pageResult));
        //
        //     var result = pageResult.Combine(core.RouteData);
        //     return result;
        // }
        //
        // public static Dictionary<string, object> FixRoute<TInput>(Response response, TInput model) where TInput : InputBase<TInput>
        // {
        //     var isValid = model.Validate();
        //     string message;
        //     if (isValid)
        //     {
        //         message = response.Message;
        //     }
        //     else
        //     {
        //         response = new Response(Flags.RetryAfterReview);
        //         message = model?.ValidationErrors?.Any() == true
        //             ? model.ToQueryString()
        //             : response.Message;
        //     }
        //
        //     var routeData = new Dictionary<string, object>
        //     {
        //         {Query_STATUS, (int) response.Status},
        //         {Query_MESSAGE, WebUtility.UrlEncode(message)}
        //     };
        //
        //     return routeData;
        // }
        //
        // /// <summary>
        // /// An internal API to handle OnPost request
        // /// </summary>
        // /// <typeparam name="TEntity">Type of Entity that will returns after running <paramref name="action"/> —Based on <see cref="EntityBase"/></typeparam>
        // /// <typeparam name="TInputModel">Type of Input model based on <see cref="InputBase"/></typeparam>
        // /// <param name="model">Input model that supposed to push to API</param>
        // /// <param name="responseWrapper">API action that returns <see cref="Response{TSource}"/></param>
        // private HandlerResponse RedirectCore<TEntity, TInputModel>(TInputModel model,
        //   Response<TEntity> responseWrapper)
        //   where TEntity : class where TInputModel : ValidatableObject
        // {
        //     var (actionStatus, entity) = responseWrapper
        //                                  ?? throw new ArgumentNullException($"{nameof(responseWrapper)} must be filled");
        //     var isValid = model.Validate();
        //
        //     InputBase<TInputModel> baseInputModel;
        //     var isNewProp = model.GetType().GetPublicProperties()
        //       .Find(x => x.Name.Equals(nameof(baseInputModel.IsNew), StringComparison.Ordinal));
        //     var isNew = isNewProp != null && (bool)isNewProp.GetValue(model);
        //
        //     string message;
        //     Response response;
        //     if (isValid)
        //     {
        //         response = new Response(actionStatus);
        //         message = response.Message;
        //     }
        //     else
        //     {
        //         response = new Response(Flags.RetryAfterReview);
        //         message = model?.ValidationErrors?.Any() == true
        //           ? model.ToQueryString()
        //           : response.Message;
        //     }
        //
        //     var routeData = new Dictionary<string, object>
        //     {
        //         {Query_STATUS, (int) response.Status},
        //         {Query_MESSAGE, WebUtility.UrlEncode(message)}
        //     };
        //     SerializedModel = JsonConvert.SerializeObject(model);
        //
        //     return new HandlerResponse
        //     {
        //         IsNew = isNew,
        //         RouteData = routeData,
        //         Status = response.Status,
        //         Message = response.Message
        //     };
        // }
    }
}