using System;
using System.Collections.Generic;
using System.Text;

using R8.Test.Shared.FakeObjects;

using Xunit;

namespace R8.Lib.Test
{
    public class SearchBaseTests
    {
        [Fact]
        public void CallTest()
        {
            // Assets
            var search = new FakeSearchModel
            {
                TestParam = "Arash"
            };

            // Act
            var actual = search["TestParam"];

            // Arrange
            Assert.NotEqual(0, search.PageSize);
            Assert.NotEqual(0, search.PageNo);
            Assert.Equal("Arash", actual);
        }

        [Fact]
        public void CallTest2()
        {
            // Assets
            var search = new FakeSearchModel();

            // Act
            search["TestParam"] = "Arash";

            // Arrange
            Assert.Equal("Arash", search.TestParam);
        }

        [Fact]
        public void CallTest5()
        {
            // Assets
            var search = new FakeSearchModel();
            search.PageNo = 0;

            // Act

            // Arrange
            Assert.Equal(1, search.PageNo);
        }

        [Fact]
        public void CallTest4()
        {
            // Assets
            var search = new FakeSearchModel();

            // Act

            // Arrange
            Assert.Throws<NullReferenceException>(() => search["TestParam2"]);
        }

        [Fact]
        public void CallTest3()
        {
            // Assets
            var search = new FakeSearchModel();

            // Act

            // Arrange
            Assert.Throws<NullReferenceException>(() => search["TestParam2"] = "Arash");
        }
    }
}