using System.ComponentModel.DataAnnotations;

using Xunit;

namespace R8.Lib.Test
{
    public class FakeValidatableObjectTest : ValidatableObject
    {
        [Required]
        public string Name { get; set; }

        public string this[string key] => string.Empty;
    }

    public class FakeValidatableObjectTest2 : ValidatableObject
    {
    }

    public class ValidatableObjectTests
    {
        [Fact]
        public void CallValidatableObject()
        {
            // Assets
            var obj = new FakeValidatableObjectTest
            {
                Name = "Arash"
            };

            // Act
            var act = obj.Validate();

            // Arrange
            Assert.True(act);
        }

        [Fact]
        public void CallValidatableObject2()
        {
            // Assets
            var obj = new FakeValidatableObjectTest2();

            // Act
            var act = obj.Validate();

            // Arrange
            Assert.True(act);
        }

        [Fact]
        public void CallValidatableObject3()
        {
            // Assets
            var obj = new FakeValidatableObjectTest
            {
                Name = string.Empty,
            };

            // Act
            var act = obj.Validate();

            // Arrange
            Assert.False(act);
            Assert.NotEmpty(obj.ValidationErrors);
        }

        [Fact]
        public void CallValidatableObject4()
        {
            // Act
            var act = ValidatableObject.TryValidateProperty<FakeValidatableObjectTest, string>(x => x.Name, "Arash", out _);

            // Arrange
            Assert.True(act);
        }

        [Fact]
        public void CallValidatableObject5()
        {
            // Act
            var act = ValidatableObject.TryValidateProperty<FakeValidatableObjectTest, string>(x => x.Name, null, out var error);

            // Arrange
            Assert.False(act);
            Assert.NotNull(error);
        }

        [Fact]
        public void CallValidatableObject6()
        {
            // Assets
            var obj = new FakeValidatableObjectTest
            {
                Name = "Arash"
            };
            var context = new ValidationContext(obj);

            // Act
            var act = ValidatableObject.Validate(context, (FakeValidatableObjectTest)null);

            // Arrange
            Assert.Empty(act);
        }

        [Fact]
        public void CallValidatableObject7()
        {
            // Assets
            var obj = new FakeValidatableObjectTest
            {
                Name = "Arash"
            };
            var context = new ValidationContext(obj);

            // Act
            var act = obj.Validate(context);

            // Arrange
            Assert.Empty(act);
        }

        [Fact]
        public void CallValidatableObject8()
        {
            // Assets
            var obj = new FakeValidatableObjectTest
            {
                Name = null
            };
            var context = new ValidationContext(obj);

            // Act
            var act = obj.Validate(context);

            // Arrange
            Assert.NotNull(act);
            Assert.NotEmpty(act);
        }
    }
}