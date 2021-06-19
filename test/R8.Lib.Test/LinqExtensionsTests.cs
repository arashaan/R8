using R8.Lib.Test.Enums;

using System.Linq;

using Xunit;

namespace R8.Lib.Test
{
    public class LinqExtensionsTests
    {
        [Fact]
        public void CallOrderBy()
        {
            var list = Enum<Flags>.ToList();
            var arrange = list
                .Select(x => ((Flags)x).ToString())
                .OrderBy(x => x, "Aborted", "Success", "ModelIsNotValid",
                "UnexpectedError", "Failed");

            Assert.NotNull(arrange);
            Assert.Equal("Aborted", arrange.First());
            Assert.Equal("Success", arrange.Skip(1).First());
            Assert.Equal("ModelIsNotValid", arrange.Skip(2).First());
            Assert.Equal("UnexpectedError", arrange.Skip(3).First());
            Assert.Equal("Failed", arrange.Skip(4).First());
        }

        [Fact]
        public void CallFindIndex()
        {
            var list = Enum<Flags>.ToList();

            var index = list
                .Select(x => ((Flags)x).ToString())
                .IndexOf(x => x == "Success");

            Assert.NotEqual(-1, index);
            Assert.InRange(index, 0, 999);
            Assert.Equal(4, index);
        }

        [Fact]
        public void CallMinBy()
        {
            var dictionary = Enum<Flags>.ToDictionary();
            var str = dictionary.MinBy(x => x.Key);

            Assert.Equal("Failed", str.Value);
        }

        [Fact]
        public void CallMaxBy()
        {
            var dictionary = Enum<Flags>.ToDictionary();
            var str = dictionary.MaxBy(x => x.Key);

            Assert.Equal("UnexpectedError", str.Value);
        }
    }
}