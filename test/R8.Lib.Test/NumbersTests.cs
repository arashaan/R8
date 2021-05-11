using System.Collections.Generic;
using System.Linq;

using Xunit;
using Xunit.Abstractions;

namespace R8.Lib.Test
{
    public class NumbersTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public NumbersTests(ITestOutputHelper output)
        {
            _outputHelper = output;
        }

        [Fact]
        public void CallRoundUp()
        {
            const double num = 1.78;
            var act = num.RoundToUp();

            Assert.Equal(2, act);
        }

        [Fact]
        public void CallHumanizeTelephoneNumbers()
        {
            var list = new List<string>()
            {
                "44447832", "44447833"
            };
            var act = Numbers.HumanizeTelephoneNumbers(list);
            Assert.Equal("44447832-3", act.First().Name);
        }

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
        public void CallCurrencyToWords2()
        {
            // Assets
            var price = "310500";

            // Act
            var final = Numbers.HumanizeCurrency(price);

            var expected = "310 هزار و 500 ";

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