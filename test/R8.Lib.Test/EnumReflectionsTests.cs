using R8.Lib.Enums;

using Xunit;

namespace R8.Lib.Test
{
    public class EnumReflectionsTests
    {
        //[Fact]
        //public void CallFromKebabCaseToEnum()
        //{
        //    // Arrange
        //    const string str = "project-owner";

        //    // Act
        //    var act = str.FromKebabCaseToEnum<Roles>();

        //    // Assert
        //    Assert.Equal(Roles.Operator, act);
        //}

        [Fact]
        public void CallToEnum_Lowercase()
        {
            // Arrange
            const string str = "operator";

            // Act
            var act = str.ToEnum<Roles>();

            // Assert
            Assert.Equal(Roles.Operator, act);
        }

        [Fact]
        public void CallToEnum()
        {
            // Arrange
            const string str = "Operator";

            // Act
            var act = str.ToEnum<Roles>();

            // Assert
            Assert.Equal(Roles.Operator, act);
        }
    }
}