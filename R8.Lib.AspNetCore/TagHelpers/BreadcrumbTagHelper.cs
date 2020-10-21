using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

using R8.Lib.AspNetCore.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using PageModel = R8.Lib.AspNetCore.Routing.PageModel;

namespace R8.Lib.AspNetCore.TagHelpers
{
    [HtmlTargetElement("breadcrumb", Attributes = ActionAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", Attributes = ControllerAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", Attributes = AreaAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", Attributes = PageAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", Attributes = PageHandlerAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", Attributes = RouteAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", Attributes = RouteValuesDictionaryName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", Attributes = RouteValuesPrefix + "*", TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("breadcrumb", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class BreadcrumbTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private const string HttpContextKey = "Breadcrumb";

        private const string RouteAttributeName = "asp-route";

        private const string RouteValuesDictionaryName = "asp-all-route-data";
        private const string RouteValuesPrefix = "asp-route-";
        private const string ActionAttributeName = "asp-action";
        private const string ControllerAttributeName = "asp-controller";
        private const string AreaAttributeName = "asp-area";
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
        private readonly IHtmlGenerator _htmlGenerator;
        private readonly IOptions<RequestLocalizationOptions> _options;

        public BreadcrumbTagHelper(IHttpContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory, IHtmlHelper htmlHelper, IHtmlGenerator htmlGenerator, IOptions<RequestLocalizationOptions> options)
        {
            _httpContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
            _htmlHelper = htmlHelper;
            _htmlGenerator = htmlGenerator;
            _options = options;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var currentPage = false;
            var contentContext = await output.GetChildContentAsync();
            var content = contentContext.GetContent();
            if (string.IsNullOrEmpty(content))
            {
                var page = ViewContext.ViewData.Model as PageModel
                           ?? throw new NullReferenceException(nameof(Page));
                if (page.PageTitle == null)
                {
                    output.SuppressOutput();
                    return;
                }

                content = page.PageTitle;
                currentPage = true;
            }

            var positions = _httpContextAccessor.HttpContext.Items.FirstOrDefault(x => x.Key.Equals(HttpContextKey)).Value
              ?.ToString();
            int.TryParse(positions, out var position);
            position++;
            (_htmlHelper as IViewContextAware)?.Contextualize(ViewContext);

            var span = new TagBuilder("span");
            span.Attributes.Add("itemprop", "name");
            span.InnerHtml.AppendHtml(content);

            var meta = new TagBuilder("meta");
            meta.Attributes.Add("itemprop", "position");
            meta.Attributes.Add("content", position.ToString());

            var hasUrl = !string.IsNullOrEmpty(Page) || !string.IsNullOrEmpty(Action);
            TagBuilder anchor;
            if (hasUrl)
            {
                var culture = (string)ViewContext.HttpContext.Request.RouteValues[LanguageRouteConstraint.Key];
                if (culture != _options.Value.DefaultRequestCulture.Culture.Name)
                    RouteValues[LanguageRouteConstraint.Key] = culture;

                anchor = await new AnchorTagHelper(_htmlGenerator)
                {
                    Action = Action,
                    Area = Area,
                    Controller = Controller,
                    Page = Page,
                    PageHandler = PageHandler,
                    Route = Route,
                    RouteValues = RouteValues,
                    ViewContext = ViewContext
                }.RenderAsync();
            }
            else
            {
                anchor = new TagBuilder("span");
            }

            anchor.Attributes.Add("itemprop", "item");
            anchor.InnerHtml.AppendHtml(span).AppendHtml(meta);

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "li";

            output.AddClass("breadcrumb-item", HtmlEncoder.Default);
            if (currentPage)
                output.AddClass("active", HtmlEncoder.Default);

            output.Attributes.Add("aria-current", "page");
            output.Attributes.Add("itemscope", "");
            output.Attributes.Add("itemprop", "itemListElement");
            output.Attributes.Add("itemtype", "http://schema.org/ListItem");
            output.Content.AppendHtml(anchor);

            _httpContextAccessor.HttpContext.Items[HttpContextKey] = position;
        }
    }
}