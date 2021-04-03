using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using R8.AspNetCore.TagBuilders;
using R8.AspNetCore.TagBuilders.TagHelpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

namespace R8.AspNetCore.Test.TagHelpersTest
{
    public class ExtensionsTests
    {
        [Fact]
        public void CallReplaceHtml_NonTagInHtml()
        {
            // Assets
            var decodedHtml =
                "Daha iyi bir kullanıcı deneyimi sağlamak, site trafiğini analiz etmek ve hedefli reklamlar sunmak için çerezleri kullanıyoruz. Bu web sitesini kullanmaya devam ederek, uygun olarak çerezlerin kullanılmasına izin vermiş olursunuz.";
            var tags = new List<Func<string, IHtmlContent>>();
            tags.Add(str => new HtmlString("<a class=\"inline-link\" asp-page=\"typeof(PrivacyModel).GetPagePath()\">Privacy Policy</a>"));

            // Acts
            var act = decodedHtml.ReplaceHtml(tags.ToArray());
            var html = act.GetString();

            var expected =
                "Daha iyi bir kullanıcı deneyimi sağlamak, site trafiğini analiz etmek ve hedefli reklamlar sunmak için çerezleri kullanıyoruz. Bu web sitesini kullanmaya devam ederek, uygun olarak çerezlerin kullanılmasına izin vermiş olursunuz.";

            // Arrange
            Assert.Equal(expected, html);
        }

        [Fact]
        public void CallParseAsHtml()
        {
            const string html = @"<a href='#'><span class='ss'>Hello!</span><a href='#'>Inner</a></a>";
            var tag = new TagBuilder("a");
            tag.Attributes.Add("href", "#");

            var act = Extensions.ParseAsTagBuilders(html)[0];
            var content = act.InnerHtml.ToString();

            Assert.Equal(tag.TagName, act.TagName);
            Assert.Equal(tag.TagRenderMode, act.TagRenderMode);
            Assert.Equal(tag.Attributes, act.Attributes);
            Assert.NotNull(tag.InnerHtml);
            Assert.NotNull(content);
        }

        [Fact]
        public void CallParseAsHtml3()
        {
            const string html = @"<br /><a href='#'><span class='ss'>Hello!</span><a href='#'>Inner</a></a>";
            var tag = new TagBuilder("a");
            tag.Attributes.Add("href", "#");

            var output = Extensions.ParseAsTagBuilders(html);
            var firstTag = new TagBuilder("br") { TagRenderMode = TagRenderMode.SelfClosing };
            var secondTag = new TagBuilder("a");

            Assert.NotNull(output);
            Assert.NotEmpty(output);
            Assert.Equal(firstTag.TagName, output[0].TagName);
            Assert.Equal(secondTag.TagName, output[1].TagName);
        }

        [Fact]
        public void CallParseAsHtml4()
        {
            const string html = @"<br />123<a href='#'><span class='ss'>Hello!</span><a href='#'>Inner</a></a>";
            var tag = new TagBuilder("a");
            tag.Attributes.Add("href", "#");

            var output = Extensions.ParseAsTagBuilders(html);
            var firstTag = new TagBuilder("br") { TagRenderMode = TagRenderMode.SelfClosing };
            var secondTag = new TagBuilder("a");

            Assert.NotNull(output);
            Assert.NotEmpty(output);
            Assert.Equal(firstTag.TagName, output[0].TagName);
            Assert.Equal(secondTag.TagName, output[1].TagName);
        }

        [Fact]
        public void CallParseAsHtml11()
        {
            const string html = @"<a href='#'><span class='ss'>Hello!</span><a href='#'>Inner</a></a>";
            var tag = new TagBuilder("a");
            tag.Attributes.Add("href", "#");

            var output = Extensions.ParseAsTagBuilders(html)[0];
            var nodes = output.GetInnerHtmlNodes();

            Assert.NotNull(output);
            Assert.NotEmpty(nodes);
            Assert.Equal("span", nodes[0].TagName);
            Assert.Equal("a", nodes[1].TagName);
            Assert.Equal("#", nodes[1].Attributes["href"]);
        }

        [Fact]
        public void CallParseAsHtml12()
        {
            const string html = @"<>";
            Assert.Throws<NullReferenceException>(() => Extensions.ParseAsTagBuilders(html)[0]);
        }

        [Fact]
        public void CallParseAsHtml8()
        {
            const string html = @"<!--— fixed_right —-->";

            var output = Extensions.ParseAsTagBuilders(html);

            Assert.Empty(output);
        }

        [Fact]
        public void CallParseAsHtml9()
        {
            const string html = @"<!--— fixed_right —--><br />123<a href='#'></a>";

            var output = Extensions.ParseAsTagBuilders(html);
            var firstTag = new TagBuilder("br") { TagRenderMode = TagRenderMode.SelfClosing };
            var secondTag = new TagBuilder("a");

            Assert.NotNull(output);
            Assert.NotEmpty(output);
            Assert.Equal(firstTag.TagName, output[0].TagName);
            Assert.Equal(secondTag.TagName, output[1].TagName);
        }

        [Fact]
        public void CallParseAsHtml10()
        {
            const string html = @"<br /><!--— fixed_right —-->123<a href='#'></a>";

            var output = Extensions.ParseAsTagBuilders(html);
            var firstTag = new TagBuilder("br") { TagRenderMode = TagRenderMode.SelfClosing };
            var secondTag = new TagBuilder("a");

            Assert.NotNull(output);
            Assert.NotEmpty(output);
            Assert.Equal(firstTag.TagName, output[0].TagName);
            Assert.Equal(secondTag.TagName, output[1].TagName);
        }

        [Fact]
        public void CallParseAsHtml5()
        {
            const string html = @"<br />123<a href='#'></a>";
            var tag = new TagBuilder("a");
            tag.Attributes.Add("href", "#");

            var output = Extensions.ParseAsTagBuilders(html);
            var firstTag = new TagBuilder("br") { TagRenderMode = TagRenderMode.SelfClosing };
            var secondTag = new TagBuilder("a");

            Assert.NotNull(output);
            Assert.NotEmpty(output);
            Assert.Equal(firstTag.TagName, output[0].TagName);
            Assert.Equal(secondTag.TagName, output[1].TagName);
        }

        [Fact]
        public void CallParseAsHtml6()
        {
            const string html = @"<br />123<a href='#'></a";
            Assert.Throws<NullReferenceException>(() => Extensions.ParseAsTagBuilders(html));
        }

        [Fact]
        public void CallParseAsHtml7()
        {
            const string html = @"<br />123<a href='#'</a";
            Assert.Throws<NullReferenceException>(() => Extensions.ParseAsTagBuilders(html));
        }

        [Fact]
        public void CallParseAsHtml2()
        {
            const string html = @"<br /><a href='#'><span class='ss'>Hello!</span><a href='#'>Inner</a></a>";
            var tag = new TagBuilder("br") { TagRenderMode = TagRenderMode.SelfClosing };

            var act = Extensions.ParseAsTagBuilders(html)[0];

            Assert.Equal(tag.TagName, act.TagName);
            Assert.Equal(tag.TagRenderMode, act.TagRenderMode);
            Assert.Empty(act.Attributes);
            Assert.Null(tag.InnerHtml.GetString());
        }

        [Fact]
        public void CallReplaceHtmlByTagName_NonTagInHtml()
        {
            // Assets
            var decodedHtml =
                "Daha iyi bir kullanıcı deneyimi sağlamak, site trafiğini analiz etmek ve hedefli reklamlar sunmak için çerezleri kullanıyoruz. Bu web sitesini kullanmaya devam ederek, uygun olarak çerezlerin kullanılmasına izin vermiş olursunuz.";
            var tags = new Dictionary<string, Func<string, IHtmlContent>>();
            tags.Add("privacyPolicy", str => new HtmlString("<a class=\"inline-link\" asp-page=\"typeof(PrivacyModel).GetPagePath()\">Privacy Policy</a>"));

            // Acts
            var act = decodedHtml.ReplaceHtmlByTagName(tags);
            var html = act.GetString();

            var expected =
                "Daha iyi bir kullanıcı deneyimi sağlamak, site trafiğini analiz etmek ve hedefli reklamlar sunmak için çerezleri kullanıyoruz. Bu web sitesini kullanmaya devam ederek, uygun olarak çerezlerin kullanılmasına izin vermiş olursunuz.";

            // Arrange
            Assert.Equal(expected, html);
        }

        [Fact]
        public void CallReplaceHtml_TagContentFromHtml()
        {
            // Assets
            var decodedHtml =
                "Daha iyi bir kullanıcı deneyimi sağlamak, site trafiğini analiz etmek ve hedefli reklamlar sunmak için çerezleri kullanıyoruz. Bu web sitesini kullanmaya devam ederek, <0>Privacy Policy</0> uygun olarak çerezlerin kullanılmasına izin vermiş olursunuz.";
            var tags = new List<Func<string, IHtmlContent>>();
            tags.Add(str => new HtmlString("<a class=\"inline-link\" asp-page=\"typeof(PrivacyModel).GetPagePath()\"></a>"));

            // Acts
            var act = decodedHtml.ReplaceHtml(tags.ToArray());
            var html = act.GetString();

            var expected =
                "Daha iyi bir kullanıcı deneyimi sağlamak, site trafiğini analiz etmek ve hedefli reklamlar sunmak için çerezleri kullanıyoruz. Bu web sitesini kullanmaya devam ederek, <a asp-page='typeof(PrivacyModel).GetPagePath()' class='inline-link'>Privacy Policy</a> uygun olarak çerezlerin kullanılmasına izin vermiş olursunuz.";

            // Arrange
            Assert.Equal(expected, html);
        }

        [Fact]
        public void CallReplaceHtmlByTagName_TagContentFromHtml()
        {
            // Assets
            var decodedHtml =
                "Daha iyi bir kullanıcı deneyimi sağlamak, site trafiğini analiz etmek ve hedefli reklamlar sunmak için çerezleri kullanıyoruz. Bu web sitesini kullanmaya devam ederek, <privacyPolicy>Privacy Policy</privacyPolicy> uygun olarak çerezlerin kullanılmasına izin vermiş olursunuz.";
            var tags = new Dictionary<string, Func<string, IHtmlContent>>();
            tags.Add("privacyPolicy",
                str => new HtmlString("<a class=\"inline-link\" asp-page=\"typeof(PrivacyModel).GetPagePath()\"></a>"));

            // Acts
            var act = decodedHtml.ReplaceHtmlByTagName(tags);
            var html = act.GetString();

            var expected =
                "Daha iyi bir kullanıcı deneyimi sağlamak, site trafiğini analiz etmek ve hedefli reklamlar sunmak için çerezleri kullanıyoruz. Bu web sitesini kullanmaya devam ederek, <a asp-page='typeof(PrivacyModel).GetPagePath()' class='inline-link'>Privacy Policy</a> uygun olarak çerezlerin kullanılmasına izin vermiş olursunuz.";

            // Arrange
            Assert.Equal(expected, html);
        }

        [Fact]
        public void CallReplaceHtmlByTagName()
        {
            // Assets
            var decodedHtml =
                "Daha iyi bir kullanıcı deneyimi sağlamak, site trafiğini analiz etmek ve hedefli reklamlar sunmak için çerezleri kullanıyoruz. Bu web sitesini kullanmaya devam ederek, <privacyPolicy></privacyPolicy> uygun olarak çerezlerin kullanılmasına izin vermiş olursunuz.";
            var tags = new Dictionary<string, Func<string, IHtmlContent>>();
            tags.Add("privacyPolicy",
                str => new HtmlString("<a class=\"inline-link\" asp-page=\"typeof(PrivacyModel).GetPagePath()\">Privacy Policy</a>"));

            // Acts
            var act = decodedHtml.ReplaceHtmlByTagName(tags);
            var html = act.GetString();

            var expected =
                "Daha iyi bir kullanıcı deneyimi sağlamak, site trafiğini analiz etmek ve hedefli reklamlar sunmak için çerezleri kullanıyoruz. Bu web sitesini kullanmaya devam ederek, <a asp-page='typeof(PrivacyModel).GetPagePath()' class='inline-link'>Privacy Policy</a> uygun olarak çerezlerin kullanılmasına izin vermiş olursunuz.";

            // Arrange
            Assert.Equal(expected, html);
        }

        [Fact]
        public void CallReplaceHtml()
        {
            // Assets
            var decodedHtml =
                "Daha iyi bir kullanıcı deneyimi sağlamak, site trafiğini analiz etmek ve hedefli reklamlar sunmak için çerezleri kullanıyoruz. Bu web sitesini kullanmaya devam ederek, <0></0> uygun olarak çerezlerin kullanılmasına izin vermiş olursunuz.";
            var tags = new List<Func<string, IHtmlContent>>();
            tags.Add(str => new HtmlString("<a class=\"inline-link\" asp-page=\"typeof(PrivacyModel).GetPagePath()\">Privacy Policy</a>"));

            // Acts
            var act = decodedHtml.ReplaceHtml(tags.ToArray());
            var html = act.GetString();

            var expected =
                "Daha iyi bir kullanıcı deneyimi sağlamak, site trafiğini analiz etmek ve hedefli reklamlar sunmak için çerezleri kullanıyoruz. Bu web sitesini kullanmaya devam ederek, <a asp-page='typeof(PrivacyModel).GetPagePath()' class='inline-link'>Privacy Policy</a> uygun olarak çerezlerin kullanılmasına izin vermiş olursunuz.";

            // Arrange
            Assert.Equal(expected, html);
        }

        [Fact]
        public void CallReplaceHtml_NullHtml()
        {
            // Assets
            var decodedHtml = (string)null;
            var tags = new List<Func<string, IHtmlContent>>();

            // Acts
            var act = decodedHtml.ReplaceHtml(tags.ToArray());
            var html = act.GetString();

            // Arrange
            Assert.Null(html);
        }

        [Fact]
        public void CallReplaceHtmlByTagName_NullHtml()
        {
            // Assets
            var decodedHtml = (string)null;
            var tags = new Dictionary<string, Func<string, IHtmlContent>>();

            // Acts
            var act = decodedHtml.ReplaceHtmlByTagName(tags);
            var html = act.GetString();

            // Arrange
            Assert.Null(html);
        }

        [Fact]
        public void CallReplaceHtml_NullTags()
        {
            // Assets
            var decodedHtml =
                "Daha iyi bir kullanıcı deneyimi sağlamak, site trafiğini analiz etmek ve hedefli reklamlar sunmak için çerezleri kullanıyoruz. Bu web sitesini kullanmaya devam ederek, <0></0> uygun olarak çerezlerin kullanılmasına izin vermiş olursunuz.";
            var tags = new List<Func<string, IHtmlContent>>();

            // Acts
            var act = decodedHtml.ReplaceHtml(tags.ToArray());
            var html = act.GetString();

            // Arrange
            Assert.Equal(decodedHtml, html);
        }

        [Fact]
        public void CallReplaceHtmlByTagName_NullTags()
        {
            // Assets
            var decodedHtml =
                "Daha iyi bir kullanıcı deneyimi sağlamak, site trafiğini analiz etmek ve hedefli reklamlar sunmak için çerezleri kullanıyoruz. Bu web sitesini kullanmaya devam ederek, <privacyPolicy></privacyPolicy> uygun olarak çerezlerin kullanılmasına izin vermiş olursunuz.";
            var tags = new Dictionary<string, Func<string, IHtmlContent>>();

            // Acts
            var act = decodedHtml.ReplaceHtmlByTagName(tags);
            var html = act.GetString();

            // Arrange
            Assert.Equal(decodedHtml, html);
        }

        [Fact]
        public async Task CallRenderAsync()
        {
            // Assets
            var tag = new CustomSpanTagHelper();

            // Act
            var rendered = await tag.GetTagBuilderAsync();
            var html = rendered.GetString();

            // Arrange
            Assert.NotNull(html);
        }

        [Fact]
        public async Task CallRenderAsync2()
        {
            // Assets
            var tag = new CustomSpanTagHelper();

            // Act
            var rendered = await tag.GetTagBuilderAsync();
            var html = rendered.GetString();
            var parsedTag = TagBuilders.Extensions.ParseAsTagBuilder(html);

            // Arrange
            Assert.NotNull(parsedTag);
            Assert.Equal("span", parsedTag.TagName);
        }

        [Fact]
        public async Task CallRenderAsync3()
        {
            // Assets
            var tag = new CustomSpanTagHelper()
            {
                Disabled = false,
                Name = "Input.Fake"
            };

            // Act
            var rendered = await tag.GetTagBuilderAsync();
            var html = rendered.GetString();
            var parsedTag = TagBuilders.Extensions.ParseAsTagBuilder(html);
            var attributes = parsedTag.Attributes.ToList();
            // Arrange
            Assert.NotNull(parsedTag);
            Assert.Equal("span", parsedTag.TagName);
            Assert.Contains(attributes, x => x.Key == "data-valmsg-for");
        }
    }
}