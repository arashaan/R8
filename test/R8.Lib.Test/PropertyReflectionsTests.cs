using R8.Test.Shared.FakeObjects;

using System;
using System.Linq.Expressions;

using Xunit;

namespace R8.Lib.Test
{
    public class PropertyReflectionsTests
    {
        [Fact]
        public void CallGetPropertyInfo()
        {
            // Assets
            Expression<Func<FakeObj, string>> func = o => o.Name;

            // Act
            var act = func.GetPropertyInfo();

            Assert.NotNull(act);
            Assert.Equal("Name", act.Name);
        }

        [Fact]
        public void CallGetExpressionValue()
        {
            // Assets
            Expression<Func<FakeObj, string>> func = o => o.Name;

            // Act
            var act = func.GetExpressionValue();

            Assert.NotNull(act);
            Assert.IsType<Func<FakeObj, string>>(act);
        }

        [Fact]
        public void CallGetDisplayName2()
        {
            // Assets
            Expression<Func<FakeObj, string>> func = o => o.LastName;

            // Act
            var act = func.GetPropertyInfo().GetDisplayName();

            Assert.NotNull(act);
            Assert.Equal("LastName", act);
        }

        [Fact]
        public void CallHasBaseType2()
        {
            // Assets
            Expression<Func<FakeObj, string>> func = o => o.LastName;

            // Act
            var act = typeof(FakeObj).HasBaseType(typeof(string));

            Assert.False(act);
        }

        [Fact]
        public void CallGetLambda()
        {
            // Assets
            Expression<Func<FakeObj, string>> func = o => o.LastName;

            // Act
            var act = func.TryGetLambda(out var lambda);

            Assert.True(act);
            Assert.NotNull(lambda);
        }

        [Fact]
        public void CallGetDisplayName()
        {
            // Assets
            Expression<Func<FakeObj, string>> func = o => o.Name;

            // Act
            var act = func.GetPropertyInfo().GetDisplayName();

            Assert.NotNull(act);
            Assert.Equal("Arash", act);
        }

        // [Fact]
        // public void CallGetMemberValue()
        // {
        //     // Assets
        //     Expression<Func<FakeObj, string>> func = o => o.Name;
        //
        //     // Act
        //     var act = func.GetMemberName();
        //
        //     Assert.NotNull(act);
        //     Assert.Equal("Name", act);
        // }

        [Fact]
        public void CallGetMemberName()
        {
            // Assets
            Expression<Func<FakeObj, string>> func = o => o.Name;

            // Act
            var act = func.GetMemberName();

            Assert.NotNull(act);
            Assert.Equal("Name", act);
        }
    }
}