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
            const string str = "noneedtosave";

            // Act
            var act = str.ToEnum<DatabaseSaveState>();

            // Assert
            Assert.Equal(DatabaseSaveState.NoNeedToSave, act);
        }

        [Fact]
        public void CallToEnum()
        {
            // Arrange
            const string str = "NoNeedToSave";

            // Act
            var act = str.ToEnum<DatabaseSaveState>();

            // Assert
            Assert.Equal(DatabaseSaveState.NoNeedToSave, act);
        }
    }
}