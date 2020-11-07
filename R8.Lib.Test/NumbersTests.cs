using Xunit;

namespace R8.Lib.Test
{
    public class NumbersTests
    {
        [Fact]
        public void CallFixCurrency_NullArg()
        {
            // Assets
            var price = "";

            // Act
            var final = Numbers.HumanizeCurrencyTo3Numbers(price);

            var expected = (string)null;

            // Arrange
            Assert.Equal(expected, final);
        }

        [Fact]
        public void CallFixCurrency()
        {
            // Assets
            var price = "10000";

            // Act
            var final = Numbers.HumanizeCurrencyTo3Numbers(price);

            var expected = "10,000";

            // Arrange
            Assert.Equal(expected, final);
        }

        [Fact]
        public void CallCurrencyToWords_NonPrice()
        {
            // Assets
            var price = "Arash";

            // Act
            var final = Numbers.HumanizeCurrency(price);

            // Arrange
            Assert.Equal(price, final);
        }

        [Fact]
        public void CallCurrencyToWords_Zero()
        {
            // Assets
            var price = "0";

            // Act
            var final = Numbers.HumanizeCurrency(price);

            var expected = "صفر";

            // Arrange
            Assert.Equal(expected, final);
        }

        [Fact]
        public void CallCurrencyToWords()
        {
            // Assets
            var price = "10000";

            // Act
            var final = Numbers.HumanizeCurrency(price);

            var expected = "10 هزار";

            // Arrange
            Assert.Equal(expected, final);
        }

        [Fact]
        public void CallFixUnicodeNumbers_NullArg()
        {
            // Assets
            var price = (string)null;

            // Act
            var final = price.FixUnicodeNumbers();

            var expected = (string)null;

            // Arrange
            Assert.Equal(expected, final);
        }

        [Fact]
        public void CallFixUnicodeNumbers()
        {
            // Assets
            var price = "۰۹۳۶۴۰۹۱۲۰۹";

            // Act
            var final = price.FixUnicodeNumbers();

            var expected = "09364091209";

            // Arrange
            Assert.Equal(expected, final);
        }
    }
}