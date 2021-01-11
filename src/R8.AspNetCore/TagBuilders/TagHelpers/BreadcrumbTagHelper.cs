using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

using R8.AspNetCore.HttpContextExtensions;
using R8.AspNetCore.Localization;
using R8.AspNetCore.Routing;

namespace R8.AspNetCore.TagBuilders.TagHelpers
{
    [HtmlTargetElement("breadcrumb", Attributes = ActionAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", Attributes = ControllerAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", Attributes = AreaAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", Attributes = PageAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", Attributes = PageHandlerAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", Attributes = RouteAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", Attributes = ProtocolAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", Attributes = HostAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", Attributes = FragmentAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", Attributes = RouteValuesDictionaryName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", Attributes = RouteValuesPrefix + "*", TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class BreadcrumbTagHelper : TagHelper
    {
        private const string HttpContextKey = "Breadcrumb";

        private const string RouteAttributeName = "asp-route";

        private const string RouteValuesDictionaryName = "asp-all-route-data";
        private const string RouteValuesPrefix = "asp-route-";
        private const string ActionAttributeName = "asp-action";
        private const string ControllerAttributeName = "asp-controller";
        private const string HostAttributeName = "asp-host";
        private const string AreaAttributeName = "asp-area";
        private const string ProtocolAttributeName = "asp-protocol";
        private const string FragmentAttributeName = "asp-fragment";
        private const string PageAttributeName = "asp-page";
        private const string PageHandlerAttributeName = "asp-page-handler";
        private IDictionary<string, string> _routeValues;

        /// <summary>
        /// The name of the action method.
        /// </summary>
        /// <remarks>
        /// Must be <c>null</c> if <see cref="Route"/> or <see cref="Page"/> is non-<c>null</c>.
        /// </remarks>
        [HtmlAttributeName(ActionAttributeName)]
        public string Action { get; set; }

        /// <summary>
        /// The protocol for the URL, such as &quot;http&quot; or &quot;https&quot;.
        /// </summary>
        [HtmlAttributeName(ProtocolAttributeName)]
        public string Protocol { get; set; }

        /// <summary>
        /// The host name.
        /// </summary>
        [HtmlAttributeName(HostAttributeName)]
        public string Host { get; set; }

        /// <summary>
        /// The URL fragment name.
        /// </summary>
        [HtmlAttributeName(FragmentAttributeName)]
        public string Fragment { get; set; }

        /// <summary>
        /// The name of the controller.
        /// </summary>
        /// <remarks>
        /// Must be <c>null</c> if <see cref="Route"/> or <see cref="Page"/> is non-<c>null</c>.
        /// </remarks>
        [HtmlAttributeName(ControllerAttributeName)]
        public string Controller { get; set; }

        /// <summary>
        /// The name of the area.
        /// </summary>
        /// <remarks>
        /// Must be <c>null</c> if <see cref="Route"/> is non-<c>null</c>.
        /// </remarks>
        [HtmlAttributeName(AreaAttributeName)]
        public string Area { get; set; }

        /// <summary>
        /// The name of the page.
        /// </summary>
        /// <remarks>
        /// Must be <c>null</c> if <see cref="Route"/> or <see cref="Action"/>, <see cref="Controller"/>
        /// is non-<c>null</c>.
        /// </remarks>
        [HtmlAttributeName(PageAttributeName)]
        public string Page { get; set; }

        /// <summary>
        /// The name of the page handler.
        /// </summary>
        /// <remarks>
        /// Must be <c>null</c> if <see cref="Route"/> or <see cref="Action"/>, or <see cref="Controller"/>
        /// is non-<c>null</c>.
        /// </remarks>
        [HtmlAttributeName(PageHandlerAttributeName)]
        public string PageHandler { get; set; }

        /// <summary>
        /// Name of the route.
        /// </summary>
        /// <remarks>
        /// Must be <c>null</c> if one of <see cref="Action"/>, <see cref="Controller"/>, <see cref="Area"/>
        /// or <see cref="Page"/> is non-<c>null</c>.
        /// </remarks>
        [HtmlAttributeName(RouteAttributeName)]
        public string Route { get; set; }

        /// <summary>
        /// Additional parameters for the route.
        /// </summary>
        [HtmlAttributeName(RouteValuesDictionaryName, DictionaryAttributePrefix = RouteValuesPrefix)]
        public IDictionary<string, string> RouteValues
        {
            get => _routeValues ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            set => _routeValues = value;
        }

        [ViewContext, HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        private readonly IHtmlHelper _htmlHelper;
        public readonly IHtmlGenerator HtmlGenerator;
        private readonly IOptions<RequestLocalizationOptions> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BreadcrumbTagHelper(IHtmlHelper htmlHelper, IHtmlGenerator htmlGenerator, IOptions<RequestLocalizationOptions> options, IHttpContextAccessor httpContextAccessor)
        {
            _htmlHelper = htmlHelper;
            HtmlGenerator = htmlGenerator;
            _options = options;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var currentPage = false;
            var contentContent = await output.GetChildContentAsync();
            var content = contentContent.GetContent();
            if (string.IsNullOrEmpty(content))
            {
                if (ViewContext == null)
                    throw new NullReferenceException($"{nameof(ViewContext)} shouldn't be null.");

                if (!(ViewContext.ViewData.Model is PageModelBase page))
                    throw new NullReferenceException($"{nameof(ViewContext.ViewData.Model)} must be a derived type of {typeof(PageModelBase)}");

                if (page.PageTitle == null)
                {
                    output.SuppressOutput();
                    return;
                }

                content = page.PageTitle;
                currentPage = true;
            }

            var position = 0;
            if (_httpContextAccessor.HttpContext.Items.ContainsKey(HttpContextKey))
                int.TryParse(_httpContextAccessor.HttpContext.Items[HttpContextKey].ToString(), out position);
            position++;

            var span = new TagBuilder("span");
            span.Attributes.Add("itemprop", "name");
            span.InnerHtml.AppendHtml(content);

            var routeLink = Route != null;
            var actionLink = Controller != null || Action != null;
            var pageLink = Page != null || PageHandler != null;
            if ((routeLink && actionLink) || (routeLink && pageLink) || (actionLink && pageLink))
            {
                output.Content.AppendHtml(span);
            }
            else
            {
                (_htmlHelper as IViewContextAware)?.Contextualize(ViewContext);

                var culture = (string)ViewContext.HttpContext.Request.RouteValues[LanguageRouteConstraint.Key];
                if (culture != _options.Value.DefaultRequestCulture.Culture.Name)
                    RouteValues[LanguageRouteConstraint.Key] = culture;

                if (!string.IsNullOrEmpty(Area))
                    RouteValues["area"] = Area;

                var anchor = await new AnchorTagHelper(HtmlGenerator)
                {
                    Action = Action,
                    Area = Area,
                    Controller = Controller,
                    Page = Page,
                    PageHandler = PageHandler,
                    Route = Route,
                    RouteValues = RouteValues,
                    ViewContext = ViewContext,
                    Fragment = Fragment,
                    Host = Host,
                    Protocol = Protocol
                }.GetTagBuilderAsync();

                var hrefPath = anchor.Attributes["href"][1..];
                var baseUrl = _httpContextAccessor.HttpContext.GetBaseUrl()[..^1];
                var pageFinalUrl = $"{baseUrl}/{hrefPath}";

                anchor.Attributes.Add("itemprop", "item");
                anchor.Attributes.Add("itemtype", "https://schema.org/WebPage");
                anchor.Attributes.Add("itemscope", "");
                anchor.Attributes.Add("itemid", pageFinalUrl);

                anchor.InnerHtml.AppendHtml(span);
                output.Content.AppendHtml(anchor);
            }

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "li";

            output.AddClass("breadcrumb-item", HtmlEncoder.Default);
            if (currentPage)
                output.AddClass("active", HtmlEncoder.Default);

            output.Attributes.Add("aria-current", "page");
            output.Attributes.Add("itemscope", "");
            output.Attributes.Add("itemprop", "itemListElement");
            output.Attributes.Add("itemtype", "http://schema.org/ListItem");

            var meta = new TagBuilder("meta");
            meta.Attributes.Add("itemprop", "position");
            meta.Attributes.Add("content", position.ToString());

            output.Content.AppendHtml(meta);

            if (!_httpContextAccessor.HttpContext.Items.ContainsKey(BreadcrumbListTagHelper.BreadcrumbListContextName))
                _httpContextAccessor.HttpContext.Items[HttpContextKey] = position;
        }
    }
}