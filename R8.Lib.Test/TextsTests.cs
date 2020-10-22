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
        public void CallToCollected_NullParam()
        {
            // Arrange
            const string str = "";

            // Act
            var act = str.ToCollected();

            // Assert
            Assert.Equal(string.Empty, act);
        }

        [Fact]
        public void CallToCollected_EndingWithSh()
        {
            // Arrange
            const string str = "Arash";

            // Act
            var act = str.ToCollected();

            // Assert
            Assert.Equal("Arashes", act);
        }

        [Fact]
        public void CallToCollected_EndingWithCh()
        {
            // Arrange
            const string str = "Batch";

            // Act
            var act = str.ToCollected();

            // Assert
            Assert.Equal("Batches", act);
        }

        [Fact]
        public void CallToCollected_EndingWithX()
        {
            // Arrange
            const string str = "Fax";

            // Act
            var act = str.ToCollected();

            // Assert
            Assert.Equal("Faxes", act);
        }

        [Fact]
        public void CallToCollected_EndingWithS()
        {
            // Arrange
            const string str = "Bus";

            // Act
            var act = str.ToCollected();

            // Assert
            Assert.Equal("Buses", act);
        }

        [Fact]
        public void CallToCollected_EndingWithAy()
        {
            // Arrange
            const string str = "Bay";

            // Act
            var act = str.ToCollected();

            // Assert
            Assert.Equal("Bays", act);
        }

        [Fact]
        public void CallToCollected_EndingWithOy()
        {
            // Arrange
            const string str = "Boy";

            // Act
            var act = str.ToCollected();

            // Assert
            Assert.Equal("Boys", act);
        }

        [Fact]
        public void CallToCollected_EndingWithEy()
        {
            // Arrange
            const string str = "Monkey";

            // Act
            var act = str.ToCollected();

            // Assert
            Assert.Equal("Monkeys", act);
        }

        [Fact]
        public void CallToCollected_EndingWithF()
        {
            // Arrange
            const string str = "Roof";

            // Act
            var act = str.ToCollected();

            // Assert
            Assert.Equal("Roofs", act);
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
            var act = Texts.ToNormalized(null);

            // Assert
            Assert.Null(act);
        }

        [Fact]
        public void CallToNormalized_WithoutSpace_ThisIsFake()
        {
            // Arrange
            const string text = "ThisIsFake";

            // Act
            var act = text.ToNormalized(false);

            // Assert
            Assert.Equal("This Is Fake", act);
        }

        [Fact]
        public void CallToNormalized_WithSpace_ThisIsFake_Lowercase3()
        {
            // Arrange
            const string text = "AlışverişMerkezi";

            // Act
            var act = text.ToNormalized(false);

            // Assert
            Assert.Equal("Alışveriş Merkezi", act);
        }

        [Fact]
        public void CallToNormalized_WithSpace_ThisIsFake_Lowercase()
        {
            // Arrange
            const string text = "this is fake";

            // Act
            var act = text.ToNormalized(false);

            // Assert
            Assert.Equal("This Is Fake", act);
        }

        [Fact]
        public void CallToNormalized_ThreeD()
        {
            // Arrange
            const string text = "ThreeD";

            // Act
            var act = text.ToNormalized(false);

            // Assert
            Assert.Equal("Three D", act);
        }

        [Fact]
        public void CallToNormalized_WithSpace_ThisIsFake_Lowercase2()
        {
            // Arrange
            const string text = "^Fake";

            // Act
            var act = text.ToNormalized(false);

            // Assert
            Assert.Equal("^ Fake", act);
        }

        [Fact]
        public void CallToNormalized_WithSpace_ThisIsFake()
        {
            // Arrange
            const string text = "This Is Fake";

            // Act
            var act = text.ToNormalized(true);

            // Assert
            Assert.Equal("ThisIsFake", act);
        }

        [Fact]
        public void CallToNormalized_WithSpace_THISISFAKE()
        {
            // Arrange
            const string text = "THIS IS FAKE";

            // Act
            var act = text.ToNormalized(culture: new CultureInfo("en-US"), forceToTitleCase: true);

            // Assert
            Assert.Equal("This Is Fake", act);
        }

        [Fact]
        public void CallToNormalized_NoSpace_WeirdCharacters()
        {
            // Arrange
            const string text = "Café";

            // Act
            var act = text.ToNormalized(false);

            // Assert
            Assert.Equal("Café", act);
        }

        [Fact]
        public void CallToNormalized_ContainsDash2()
        {
            // Arrange
            const string text = "Central-PayMeter";

            // Act
            var act = text.ToNormalized(false);

            // Assert
            Assert.Equal("Central - Pay Meter", act);
        }

        [Fact]
        public void CallToNormalized_ContainsDash()
        {
            // Arrange
            const string text = "Built-inElectricCooker";

            // Act
            var act = text.ToNormalized(false);

            // Assert
            Assert.Equal("Built-in Electric Cooker", act);
        }

        [Fact]
        public void CallToNormalized_NoSpace_PreciousMetals()
        {
            // Arrange
            const string text = "PreciousMetals";

            // Act
            var act = text.ToNormalized(false);

            // Assert
            Assert.Equal("Precious Metals", act);
        }

        [Fact]
        public void CallToNormalized_Spaceless_Booking()
        {
            // Arrange
            const string text = "Booking";

            // Act
            var act = text.ToNormalized(true);

            // Assert
            Assert.Equal("Booking", act);
        }

        [Fact]
        public void CallToNormalized_Escaped_WithoutSpaces_OilAndGas()
        {
            // Arrange
            const string text = "Oil&Gas";

            // Act
            var act = text.ToNormalized(false);

            // Assert
            Assert.Equal("Oil & Gas", act);
        }

        [Fact]
        public void CallToNormalized_WithoutSpace_TurkishWord()
        {
            // Arrange
            const string text = "TadilatHizmetleri";

            // Act
            var act = text.ToNormalized(false);

            // Assert
            Assert.Equal("Tadilat Hizmetleri", act);
        }

        [Fact]
        public void CallToNormalized_Yer()
        {
            // Arrange
            const string text = "yer";

            // Act
            var act = text.ToNormalized(false);

            // Assert
            Assert.Equal("Yer", act);
        }

        [Fact]
        public void CallToNormalized_Escaped_WithSpaces_IELTSAndTOMER()
        {
            // Arrange
            const string text = "IELTS&TOMER";

            // Act
            var act = text.ToNormalized(false);

            // Assert
            Assert.Equal("IELTS & TOMER", act);
        }
    }
}