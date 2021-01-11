using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Html;

using R8.AspNetCore.TagBuilders;
using R8.AspNetCore.TagBuilders.TagHelpers;

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
            Assert.Empty(html);
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
            Assert.Empty(html);
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
            var parsedTag = TagBuilders.Extensions.GetTagBuilder(html);

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
            var parsedTag = TagBuilders.Extensions.GetTagBuilder(html);
            var attributes = parsedTag.Attributes.ToList();
            // Arrange
            Assert.NotNull(parsedTag);
            Assert.Equal("span", parsedTag.TagName);
            Assert.Contains(attributes, x => x.Key == "data-valmsg-for");
        }
    }
}