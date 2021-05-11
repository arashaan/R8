using System.Globalization;

using Xunit;

namespace R8.Lib.Test
{
    public class TextsTests
    {
        [Fact]
        public void CallToCamelCase()
        {
            // Arrange
            const string str = "ThisIsFake";

            // Act
            var act = str.ToCamelCase();

            // Assert
            Assert.Equal("thisIsFake", act);
        }

        [Fact]
        public void CallGetStringBetween()
        {
            const string text = "this Text is [ Fake ] .";
            var act = text.GetStringBetween('[', ']').Trim();

            Assert.Equal("Fake", act);
        }

        [Fact]
        public void CallFromKebabCase()
        {
            // Arrange
            const string str = "this-is-fake";

            // Act
            var act = str.FromKebabCase();

            // Assert
            Assert.Equal("this is fake", act);
        }

        [Fact]
        public void CallToKebabCase()
        {
            // Arrange
            const string str = "This Is Fake";

            // Act
            var act = str.ToKebabCase();

            // Assert
            Assert.Equal("this-is-fake", act);
        }

        [Fact]
        public void CallToNormalized_NullParam()
        {
            // Arrange

            // Act
            var act = Texts.Humanize(null);

            // Assert
            Assert.Null(act);
        }

        [Fact]
        public void CallToNormalized_WithoutSpace_ThisIsFake()
        {
            // Arrange
            const string text = "ThisIsFake";

            // Act
            var act = text.Humanize(false);

            // Assert
            Assert.Equal("This Is Fake", act);
        }

        [Fact]
        public void CallToNormalized_WithSpace_ThisIsFake_Lowercase3()
        {
            // Arrange
            const string text = "AlışverişMerkezi";

            // Act
            var act = text.Humanize(false);

            // Assert
            Assert.Equal("Alışveriş Merkezi", act);
        }

        [Fact]
        public void CallToNormalized_WithSpace_ThisIsFake_Lowercase()
        {
            // Arrange
            const string text = "this is fake";

            // Act
            var act = text.Humanize(false);

            // Assert
            Assert.Equal("This Is Fake", act);
        }

        [Fact]
        public void CallToNormalized_ThreeD()
        {
            // Arrange
            const string text = "ThreeD";

            // Act
            var act = text.Humanize(false);

            // Assert
            Assert.Equal("Three D", act);
        }

        [Fact]
        public void CallToNormalized_WithSpace_ThisIsFake_Lowercase2()
        {
            // Arrange
            const string text = "^Fake";

            // Act
            var act = text.Humanize(false);

            // Assert
            Assert.Equal("^ Fake", act);
        }

        [Fact]
        public void CallToNormalized_WithSpace_ThisIsFake()
        {
            // Arrange
            const string text = "This Is Fake";

            // Act
            var act = text.Humanize(true);

            // Assert
            Assert.Equal("ThisIsFake", act);
        }

        [Fact]
        public void CallToNormalized_WithSpace_THISISFAKE()
        {
            // Arrange
            const string text = "THIS IS FAKE";

            // Act
            var act = text.Humanize(culture: new CultureInfo("en-US"), forceToTitleCase: true);

            // Assert
            Assert.Equal("This Is Fake", act);
        }

        [Fact]
        public void CallToNormalized_NoSpace_WeirdCharacters()
        {
            // Arrange
            const string text = "Café";

            // Act
            var act = text.Humanize(false);

            // Assert
            Assert.Equal("Café", act);
        }

        [Fact]
        public void CallToNormalized_ContainsDash2()
        {
            // Arrange
            const string text = "Central-PayMeter";

            // Act
            var act = text.Humanize(false);

            // Assert
            Assert.Equal("Central - Pay Meter", act);
        }

        [Fact]
        public void CallToNormalized_NoSpace_PreciousMetals()
        {
            // Arrange
            const string text = "PreciousMetals";

            // Act
            var act = text.Humanize(false);

            // Assert
            Assert.Equal("Precious Metals", act);
        }

        [Fact]
        public void CallToNormalized_Spaceless_Booking()
        {
            // Arrange
            const string text = "Booking";

            // Act
            var act = text.Humanize(true);

            // Assert
            Assert.Equal("Booking", act);
        }

        [Fact]
        public void CallToNormalized_Escaped_WithoutSpaces_OilAndGas()
        {
            // Arrange
            const string text = "Oil&Gas";

            // Act
            var act = text.Humanize(false);

            // Assert
            Assert.Equal("Oil & Gas", act);
        }

        [Fact]
        public void CallToNormalized_WithoutSpace_TurkishWord()
        {
            // Arrange
            const string text = "TadilatHizmetleri";

            // Act
            var act = text.Humanize(false);

            // Assert
            Assert.Equal("Tadilat Hizmetleri", act);
        }

        [Fact]
        public void CallToNormalized_Yer()
        {
            // Arrange
            const string text = "yer";

            // Act
            var act = text.Humanize(false);

            // Assert
            Assert.Equal("Yer", act);
        }

        [Fact]
        public void CallToNormalized_Escaped_WithSpaces_IELTSAndTOMER()
        {
            // Arrange
            const string text = "IELTS&TOMER";

            // Act
            var act = text.Humanize(false);

            // Assert
            Assert.Equal("IELTS & TOMER", act);
        }
    }
}