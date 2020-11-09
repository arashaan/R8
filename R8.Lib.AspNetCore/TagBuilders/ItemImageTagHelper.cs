using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

using R8.Lib.AspNetCore.Routing;

using System.Linq;
using System.Text.Encodings.Web;

namespace R8.Lib.AspNetCore.TagBuilders
{
    [HtmlTargetElement("item-image", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class ItemImageTagHelper : TagHelper
    {
        private readonly ICulturalizedUrlHelper _urlHelper;

        public ItemImageTagHelper(ICulturalizedUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public string Image { get; set; }
        public string Alt { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(Image))
            {
                output.SuppressOutput();
                return;
            }

            var iconType = IconAttribute.GetIconType(Image);
            switch (iconType)
            {
                case IconType.Image:
                    var url = _urlHelper.Content("~" + Image);
                    output.Attributes.Add("src", url);
                    output.Attributes.Add("alt", Alt);
                    output.TagMode = TagMode.SelfClosing;
                    output.TagName = "img";
                    break;

                case IconType.Glyph:
                    Image.Split(" ")
                        .ToList()
                        .ForEach(cls => output.AddClass(cls, HtmlEncoder.Default));
                    output.TagMode = TagMode.StartTagAndEndTag;
                    output.TagName = "i";
                    break;

                case IconType.SvgPath:
                    var svgTag = new TagBuilder("svg");
                    svgTag.AddCssClass("image-svg");
                    //svgTag.Attributes.Add("xmlns", "http://www.w3.org/2000/svg");
                    //svgTag.Attributes.Add("preserveAspectRatio", "xMinYMin meet");
                    //svgTag.Attributes.Add("width", "100%");
                    //svgTag.Attributes.Add("height", "100%");
                    svgTag.Attributes.Add("version", "1.1");
                    svgTag.InnerHtml.AppendHtml(Image);
                    output.Content.AppendHtml(svgTag);
                    output.AddClass("image-svg-container", NullHtmlEncoder.Default);
                    output.TagMode = TagMode.StartTagAndEndTag;
                    output.TagName = "div";
                    break;

                default:
                    output.SuppressOutput();
                    return;
            }
        }
    }
}