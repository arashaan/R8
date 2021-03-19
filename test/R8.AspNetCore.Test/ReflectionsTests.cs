using R8.AspNetCore.Test.FakeObjects;

using System.ComponentModel.DataAnnotations;
using System.Linq;

using Xunit;

namespace R8.AspNetCore.Test
{
    public class ReflectionsTests
    {
        [Fact]
        public void CallGetValidatablePropertyModelMetadata()
        {
            var model = new FakeObjHasReq
            {
                LastName = "Shabbeh",
                Name = "Arash"
            };

            var act = model.GetMetadataForProperty(x => x.LastName);
            var attributes = act.Attributes.PropertyAttributes;
            var required = attributes.Any(x => x is RequiredAttribute);

            Assert.True(required);
        }

        [Fact]
        public void CallGetValidatablePropertyModelExplorer2()
        {
            var model = new FakeObjHasReq
            {
                LastName = "Shabbeh",
                Name = "Arash"
            };

            var act = model.GetExplorerForProperty("LastName");
            var attributes = act.Metadata.GetDefaultModelMetadata().Attributes.PropertyAttributes;
            var required = attributes.Any(x => x is RequiredAttribute);

            Assert.True(required);
        }

        [Fact]
        public void CallGetValidatablePropertyModelExplorer()
        {
            var model = new FakeObjHasReq
            {
                LastName = "Shabbeh",
                Name = "Arash"
            };

            var act = model.GetExplorerForProperty(x => x.LastName);
            var attributes = act.Metadata.GetDefaultModelMetadata().Attributes.PropertyAttributes;
            var required = attributes.Any(x => x is RequiredAttribute);

            Assert.True(required);
        }

        [Fact]
        public void CallGetValidatablePropertyModelMetadata3()
        {
            var model = new FakeObjHasReq
            {
                LastName = "Shabbeh",
                Name = "Arash"
            };

            var act = model.GetMetadataForProperty(x => x.LastName);
            var attributes = act.GetAttributes();
            var required = attributes.Any(x => x is RequiredAttribute);

            Assert.True(required);
        }

        [Fact]
        public void CallGetValidatablePropertyModelMetadata4()
        {
            var model = new FakeObjHasReq
            {
                LastName = "Shabbeh",
                Name = "Arash"
            };

            var act = model.GetMetadataForProperty(x => x.LastName);
            var required = act.IsRequired();

            Assert.True(required);
        }

        [Fact]
        public void CallGetValidatablePropertyModelMetadata2()
        {
            var model = new FakeObjHasReq
            {
                LastName = "Shabbeh",
                Name = "Arash"
            };

            var act = model.GetMetadataForProperty("LastName");
            var attributes = act.Attributes.PropertyAttributes;
            var required = attributes.Any(x => x is RequiredAttribute);

            Assert.True(required);
        }
    }
}