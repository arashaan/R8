using Newtonsoft.Json;

using R8.Lib.JsonExtensions;
using R8.Lib.Test.FakeObjects;
using R8.Lib.Test.MethodTests;

using Xunit;
using PropertyReflections = R8.Lib.PropertyReflections;

namespace R8.Lib.Test
{
    public class JsonReflectionsTests
    {
        [Fact]
        public void CallGetJsonProperty()
        {
            // Assets
            var obj = new FakeJsonTest
            {
                Name = "Arash"
            };

            // Act
            var propertyInfo = obj.GetType().GetProperty(nameof(obj.Name));
            var jsonName = propertyInfo.GetJsonName();

            // Arrange
            Assert.Equal("nm", jsonName);
        }

        [Fact]
        public void CallGetJsonProperty10()
        {
            // Act
            var property = JsonHandler<FakeResponse>.GetProperty(x => x.Status);

            // Arrange
            Assert.Equal("sts", property);
        }

        [Fact]
        public void CallGetJsonProperty4()
        {
            // Assets
            var obj = new FakeJsonTest
            {
                Name = "Arash"
            };

            // Act
            var propertyInfo = JsonHandler<FakeJsonTest>.GetProperty(x => x.Name);

            // Arrange
            Assert.Equal("nm", propertyInfo);
        }

        [Fact]
        public void CallGetJsonProperty3()
        {
            // Act
            var jsonName = PropertyReflections.GetPropertyInfo<FakeObj>(x => x.Name).GetJsonName();

            // Arrange
            Assert.Equal("Name", jsonName);
        }

        [Fact]
        public void CallGetJsonProperty2()
        {
            // Act
            var jsonName = PropertyReflections.GetPropertyInfo<FakeJsonTest>(x => x.Name).GetJsonName();

            // Arrange
            Assert.Equal("nm", jsonName);
        }
    }
}