using R8.Lib.Test.Enums;

using System;
using System.Linq;

using Xunit;
using Xunit.Abstractions;

namespace R8.Lib.Test
{
    public class Enum_Tests
    {
        private readonly ITestOutputHelper _outputHelper;

        public Enum_Tests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void CallEnumParseFromKebabCase()
        {
            var arrange = Enum<Flags>.ParseFromKebabCase("model-is-not-valid");

            Assert.Equal(Flags.ModelIsNotValid, arrange);
        }

        [Fact]
        public void CallEnumTryParseFromKebabCase()
        {
            var arrange = Enum<Flags>.TryParseFromKebabCase("model-is-not-valid", out var @enum);

            Assert.Equal(Flags.ModelIsNotValid, @enum);
            Assert.True(arrange);
        }

        [Fact]
        public void CallEnumTryParseFromKebabCase2()
        {
            var arrange = Enum<Flags>.TryParseFromKebabCase("model-is-not-valid2", out var @enum);

            Assert.Equal(Flags.Failed, @enum);
            Assert.False(arrange);
        }

        [Fact]
        public void CallEnumFromKebabCase2()
        {
            Assert.Throws<ArgumentException>(() => Enum<Flags>.ParseFromKebabCase("model-is-not-valid2"));
        }

        [Fact]
        public void CallEnumToDictionary()
        {
            var arrange = Enum<Flags>.ToDictionary();

            Assert.NotNull(arrange);
            Assert.NotEmpty(arrange);
            Assert.Equal("Failed", arrange.First().Value);
            Assert.Equal(0, arrange.First().Key);
        }

        [Fact]
        public void CallEnumToListOrderBy()
        {
            var arrange = Enum<Flags>.ToListOrderBy(Flags.Aborted, Flags.Success, Flags.ModelIsNotValid,
                Flags.UnexpectedError, Flags.Failed);

            Assert.NotNull(arrange);
            Assert.NotEmpty(arrange);
            Assert.Equal(Flags.Aborted, arrange.First());
            Assert.Equal(Flags.Success, arrange.Skip(1).First());
            Assert.Equal(Flags.ModelIsNotValid, arrange.Skip(2).First());
            Assert.Equal(Flags.UnexpectedError, arrange.Skip(3).First());
            Assert.Equal(Flags.Failed, arrange.Skip(4).First());
        }

        [Fact]
        public void CallEnumCount()
        {
            var arrange = Enum<Flags>.Count;
            Assert.Equal(41, arrange);
        }
    }
}