using R8.Lib.Attributes;
using R8.Lib.Test.Enums;

using Xunit;

namespace R8.Lib.Test
{
    public class EnumReflectionsTests
    {
        [Fact]
        public void CallGetAttribute()
        {
            // Arrange
            const Flags @enum = Flags.Success;

            // Act
            var act = @enum.GetAttribute<FlagShowAttribute>();

            // Assert
            Assert.NotNull(act);
        }

        [Fact]
        public void CallToEnum_Lowercase()
        {
            // Arrange
            const string str = "tokenisnotvalid";

            // Act
            var act = str.ToEnum<Flags>();

            // Assert
            Assert.Equal(Flags.TokenIsNotValid, act);
        }

        [Fact]
        public void CallToEnum()
        {
            // Arrange
            const string str = "TokenIsNotValid";

            // Act
            var act = str.ToEnum<Flags>();

            // Assert
            Assert.Equal(Flags.TokenIsNotValid, act);
        }
    }
}