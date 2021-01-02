using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace R8.Lib.Test
{
    public class RandomSelectorTests
    {
        [Fact]
        public void CallGenerateDigitToken()
        {
            // Assets

            // Act
            var token = RandomSelector.GenerateDigitToken();
            var tokenInt = int.Parse(token);

            // Arrange
            Assert.NotNull(token);
            Assert.InRange(tokenInt, 0, 999999);
            Assert.Equal(6, token.Length);
        }
    }
}