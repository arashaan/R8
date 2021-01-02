using System;
using System.Globalization;
using Xunit;

namespace R8.Lib.Test
{
    public class ExtensionsTests
    {
        [Fact]
        public void CallGetTwoLetterCulture_NullCulture()
        {
            // Arrange
            Assert.Throws<ArgumentNullException>(() => Extensions.GetTwoLetterCulture(null));
        }

        [Fact]
        public void CallGetTwoLetterCulture()
        {
            // Assets
            var culture = CultureInfo.GetCultureInfo("en");

            // Act
            var iso = culture.GetTwoLetterCulture();

            // Arrange
            Assert.Equal("en", iso);
        }
    }
}