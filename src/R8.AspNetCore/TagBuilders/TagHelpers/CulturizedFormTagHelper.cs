using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using R8.AspNetCore.Localization;

namespace R8.AspNetCore.TagBuilders.TagHelpers
{
    [HtmlTargetElement("form", Attributes = ActionAttributeName)]
    [HtmlTargetElement("form", Attributes = ControllerAttributeName)]
    [HtmlTargetElement("form", Attributes = AreaAttributeName)]
    [HtmlTargetElement("form", Attributes = PageAttributeName)]
    [HtmlTargetElement("form", Attributes = PageHandlerAttributeName)]
    [HtmlTargetElement("form", Attributes = FragmentAttributeName)]
    [HtmlTargetElement("form", Attributes = HostAttributeName)]
    [HtmlTargetElement("form", Attributes = ProtocolAttributeName)]
    [HtmlTargetElement("form", Attributes = RouteAttributeName)]
    [HtmlTargetElement("form", Attributes = RouteValuesDictionaryName)]
    [HtmlTargetElement("form", Attributes = RouteValuesPrefix + "*")]
    public class CulturizedFormTagHelper : FormTagHelper
    {
        private readonly IOptions<RequestLocalizationOptions> _options;

        public CulturizedFormTagHelper(IHtmlGenerator generator, IOptions<RequestLocalizationOptions> options) : base(generator)
        {
            _options = options;
        }

        private const string ActionAttributeName = "asp-action";
        private const string ControllerAttributeName = "asp-controller";
        private const string AreaAttributeName = "asp-area";
        private const string PageAttributeName = "asp-page";
        private const string PageHandlerAttributeName = "asp-page-handler";
        private const string FragmentAttributeName = "asp-fragment";
        private const string HostAttributeName = "asp-host";
        private const string ProtocolAttributeName = "asp-protocol";
        private const string RouteAttributeName = "asp-route";
        private const string RouteValuesDictionaryName = "asp-all-route-data";
        private const string RouteValuesPrefix = "asp-route-";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var culture = (string)ViewContext.HttpContext.Request.RouteValues[LanguageRouteConstraint.Key];
            if (culture != _options.Value.DefaultRequestCulture.Culture.Name)
                RouteValues[LanguageRouteConstraint.Key] = culture;

            base.Process(context, output);
        }
    }
}