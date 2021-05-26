using R8.Lib.Test.FakeObjects;

using System;
using System.Linq.Expressions;

using Xunit;

namespace R8.Lib.Test
{
    public class PropertyReflectionsTests
    {
        [Fact]
        public void CallToDictionary()
        {
            var obj = new FakeObj
            {
                Name = "Arash",
                LastName = "Shabbeh"
            };

            var act = obj.ToDictionary();

            Assert.NotNull(act);
            Assert.NotEmpty(act);
            Assert.Equal("Shabbeh", act["LastName"]);
        }

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