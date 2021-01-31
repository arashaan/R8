using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

using R8.AspNetCore.Attributes;
using R8.AspNetCore.Localization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace R8.AspNetCore.TagBuilders.TagHelpers
{
    [HtmlTargetElement("nav-button", Attributes = ActionAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("nav-button", Attributes = ControllerAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("nav-button", Attributes = AreaAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("nav-button", Attributes = PageAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("nav-button", Attributes = PageHandlerAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("nav-button", Attributes = RouteAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("nav-button", Attributes = RouteValuesDictionaryName, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("nav-button", Attributes = RouteValuesPrefix + "*", TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("nav-button", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class NavBarButtonTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public NavBarButtonTagHelper(IUrlHelperFactory urlHelperFactory, IHtmlGenerator htmlGenerator, IOptions<RequestLocalizationOptions> options)
        {
            _urlHelperFactory = urlHelperFactory;
            _htmlGenerator = htmlGenerator;
            _options = options;
        }

        [HtmlAttributeName("subject")]
        public string Subject { get; set; }

        [HtmlAttributeName("text")]
        public string Text { get; set; }

        [HtmlAttributeName("class")]
        public string Class { get; set; }

        [HtmlAttributeName("icon")]
        public string Icon { get; set; }

        [HtmlAttributeName("badge")] public int BadgeNumber { get; set; } = 0;

        [HtmlAttributeName("show-badge-when-zero")]
        public bool ShowBadgeWhenZero { get; set; } = false;

        [HtmlAttributeName("show-badge")]
        public bool ShowBadge { get; set; } = false;

        [HtmlAttributeName("hideOnTrigger")]
        public bool HideOnTrigger { get; set; }

        private const string RouteAttributeName = "asp-route";

        private const string RouteValuesDictionaryName = "asp-all-route-data";
        private const string RouteValuesPrefix = "asp-route-";
        private const string ActionAttributeName = "asp-action";
        private const string ControllerAttributeName = "asp-controller";
        private const string AreaAttributeName = "asp-area";
        private const string PageAttributeName = "asp-page";
        private const string PageHandlerAttributeName = "asp-page-handler";
        private IDictionary<string, string> _routeValues;
        private IHtmlGenerator _htmlGenerator;
        private readonly IOptions<RequestLocalizationOptions> _options;

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

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (Text != null && !string.IsNullOrEmpty(Subject))
                throw new Exception("One of the localized and subject must be filled");

            if (Text == null && string.IsNullOrEmpty(Subject))
                throw new Exception("Atleast, one of the localized and subject must be filled");

            var contentContext = await output.GetChildContentAsync();
            var content = contentContext.GetContent();

            var classes = new List<string>();

            var subject = !string.IsNullOrEmpty(Text) ? Text : Subject;
            if (string.IsNullOrEmpty(subject))
            {
                output.SuppressOutput();
                return;
            }

            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

            var navLink = new TagBuilder("a");

            var isWrapper = string.IsNullOrEmpty(Page) && string.IsNullOrEmpty(Action);

            var title = new TagBuilder("a");
            title.AddCssClass("nav-link");
            if (isWrapper)
            {
                // includes children

                title.Attributes.Add("href", "#");
                output.Attributes.Add("aria-expanded", "false");
            }
            else
            {
                // single item

                var culture = (string)ViewContext.HttpContext.Request.RouteValues[LanguageRouteConstraint.Key];
                if (culture != _options.Value.DefaultRequestCulture.Culture.Name)
                    RouteValues[LanguageRouteConstraint.Key] = culture;

                var anchor = await new AnchorTagHelper(_htmlGenerator)
                {
                    Action = Action,
                    Area = Area,
                    Controller = Controller,
                    Page = Page,
                    PageHandler = PageHandler,
                    Route = Route,
                    RouteValues = RouteValues,
                    ViewContext = ViewContext
                }.GetTagBuilderAsync();
                var link = anchor.Attributes.First(x => x.Key == "href").Value;

                if (ViewContext.HttpContext.Items
                  .FirstOrDefault(x => x.Key.Equals(NavBarAttribute.Key))
                  .Value is string current)
                {
                    var assembly = Assembly.GetEntryAssembly();
                    var currentPageType = assembly.GetType(current);
                    if (currentPageType != null)
                    {
                        if (currentPageType.FullName != null)
                        {
                            var dividedUrl = currentPageType.FullName.Split(".Pages.")[1];
                            if (!string.IsNullOrEmpty(dividedUrl))
                            {
                                if (dividedUrl.EndsWith("Model"))
                                {
                                    var modelIndex = dividedUrl.LastIndexOf("Model", StringComparison.CurrentCultureIgnoreCase);
                                    dividedUrl = dividedUrl.Substring(0, modelIndex);
                                }

                                dividedUrl = dividedUrl.Replace(".", "/");

                                if (!dividedUrl.StartsWith("/"))
                                    dividedUrl = $"/{dividedUrl}";

                                var currentPageLink = urlHelper.Page(dividedUrl);
                                var success = currentPageLink == link;
                                if (success)
                                {
                                    if (HideOnTrigger)
                                    {
                                        output.SuppressOutput();
                                        return;
                                    }
                                    classes.Add("active");
                                }
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(Class))
                    classes.AddRange(Class.Split(' '));

                title.Attributes.Add("href", link);
                title.AddCssClass("is-active");
            }

            var image = new TagBuilder("div");
            image.AddCssClass("img");
            if (!string.IsNullOrEmpty(Icon))
            {
                TagBuilder img;
                if (Icon.StartsWith("~/") || Icon.StartsWith("/"))
                {
                    img = new TagBuilder("img");
                    img.Attributes.Add("src", urlHelper.Content(Icon));
                    img.Attributes.Add("alt", subject);
                }
                else
                {
                    // fa
                    img = new TagBuilder("i");
                    img.AddCssClass(Icon);
                }

                image.InnerHtml.AppendHtml(img);
            }
            else
            {
                image.AddCssClass("no-image");
            }

            var text = new TagBuilder("div");
            text.AddCssClass("text");
            text.InnerHtml.Append(subject);

            title.InnerHtml.AppendHtml(image).AppendHtml(text);
            output.Content.AppendHtml(title);

            if (isWrapper && !string.IsNullOrEmpty(content))
            {
                var subItems = new TagBuilder("ul");
                subItems.AddCssClass("sub-items");
                subItems.Attributes.Add("aria-hidden", "true");
                subItems.InnerHtml.AppendHtml(content);

                output.Content.AppendHtml(subItems);
            }

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "li";
            output.AddClass("nav-item", HtmlEncoder.Default);

            //if (ShowBadge)
            //{
            //    if ((BadgeNumber == 0 && ShowBadgeWhenZero) || BadgeNumber > 0)
            //    {
            //        var badge = new TagBuilder("span");
            //        badge.AddCssClass("badge");
            //        badge.AddCssClass("badge-dark");
            //        badge.InnerHtml.AppendHtml(BadgeNumber.ToString());

            //        navLink.InnerHtml.AppendHtml(badge);
            //    }
            //}
        }
    }
}