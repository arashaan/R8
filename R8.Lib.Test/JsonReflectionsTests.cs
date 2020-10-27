using Newtonsoft.Json;

using Xunit;

namespace R8.Lib.Test
{
    public class FakeJsonTest
    {
        [JsonProperty("nm")]
        public string Name { get; set; }
    }

    public class FakeJsonTest2
    {
        public string Name { get; set; }
    }

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
            var jsonName = propertyInfo.GetJsonProperty();

            // Arrange
            Assert.Equal("nm", jsonName);
        }

        [Fact]
        public void CallGetJsonProperty3()
        {
            // Act
            var jsonName = JsonReflections.GetJsonProperty<FakeJsonTest2>(x => x.Name);

            // Arrange
            Assert.Equal("Name", jsonName);
        }

        [Fact]
        public void CallGetJsonProperty2()
        {
            // Act
            var jsonName = JsonReflections.GetJsonProperty<FakeJsonTest>(x => x.Name);

            // Arrange
            Assert.Equal("nm", jsonName);
        }
    }
}