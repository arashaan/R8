using R8.Lib.Test.Enums;
using R8.Lib.Test.FakeObjects;
using R8.Lib.Validatable;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Xunit;

namespace R8.Lib.Test
{
    public class TypeReflectionsTests
    {
        [Fact]
        public void GetIsNumeric()
        {
            const int num = 123;

            var isNumeric = num.GetType().IsNumeric();

            Assert.True(isNumeric);
        }

        [Fact]
        public void GetIsNumeric2()
        {
            const string num = "123";

            var isNumeric = num.GetType().IsNumeric();

            Assert.False(isNumeric);
        }

        [Fact]
        public void GetGetTypesAssignableFrom()
        {
            var type = typeof(FakeObjHasReq);
            var assembly = type.Assembly;
            var types = assembly.GetTypesAssignableFrom<ValidatableObject<FakeObjHasReq>>();

            Assert.Contains(types, x => x == type);
        }

        [Fact]
        public void CallGetUnderlyingType_Object()
        {
            // Assets
            var list = new List<object>();

            // Act
            var act = list.GetType().GetUnderlyingType();

            Assert.Equal(typeof(object), act);
        }

        [Fact]
        public void CallGetUnderlyingType_ObjectNullable()
        {
            // Assets
            var list = new List<object?>();

            // Act
            var act = list.GetType().GetUnderlyingType();

            Assert.Equal(typeof(object), act);
        }

        [Fact]
        public void CallGetUnderlyingType_IntNullable()
        {
            // Assets
            var list = new List<int?>();

            // Act
            var act = list.GetType().GetUnderlyingType(false);

            Assert.Equal(typeof(int?), act);
        }

        [Fact]
        public void CallGetUnderlyingType_Int()
        {
            // Assets
            var list = new List<int?>();

            // Act
            var act = list.GetType().GetUnderlyingType(false);

            Assert.NotEqual(typeof(int), act);
        }

        [Fact]
        public void CallGetUnderlyingType5()
        {
            // Assets
            int? list = 5;

            // Act
            var act = list.GetType().GetUnderlyingType(false);

            Assert.Equal(typeof(int), act);
        }

        [Fact]
        public void CallTrySetValue_DateTime8()
        {
            // Assets
            var input = new List<string>()
            {
                "2020/05/23",
                "2020/05/232",
                "2020/05/23",
            };
            var type = typeof(List<DateTime>);

            // Act
            var method = type.TryParse(input, out var output);

            var expected = new DateTime(2020, 05, 23);
            DateTime.SpecifyKind(expected, DateTimeKind.Utc);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<List<DateTime>>(output);
        }

        [Fact]
        public void CallTrySetValue_DateTime9()
        {
            // Assets
            var input = new[]
            {
                "2020/05/23",
                "2020/05/232",
                "2020/05/23",
            };
            var type = typeof(DateTime[]);

            // Act
            var method = type.TryParse(input, out var output);

            var expected = new DateTime(2020, 05, 23);
            DateTime.SpecifyKind(expected, DateTimeKind.Utc);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<DateTime[]>(output);
        }

        [Fact]
        public void CallTrySetValue_DateTime2()
        {
            // Assets
            var input = "2020/05/223";
            var type = typeof(DateTime);

            // Act
            var method = type.TryParse(input, out var output);

            var expected = new DateTime(2020, 05, 23);
            DateTime.SpecifyKind(expected, DateTimeKind.Utc);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_DateTime()
        {
            // Assets
            var input = "2020/05/23";
            var type = typeof(DateTime);

            // Act
            var method = type.TryParse(input, out var output);

            var expected = new DateTime(2020, 05, 23);
            DateTime.SpecifyKind(expected, DateTimeKind.Utc);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<DateTime>(output);
            Assert.Equal(expected, output);
        }

        [Fact]
        public void CallTrySetValue_DateTime3()
        {
            // Assets
            var input = "2020";
            var type = typeof(DateTime);

            // Act
            var method = type.TryParse(input, out var output);

            var expected = new DateTime(2020, 01, 01);
            DateTime.SpecifyKind(expected, DateTimeKind.Utc);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<DateTime>(output);
            Assert.Equal(expected, output);
        }

        [Fact]
        public void CallTrySetValue_DateTime4()
        {
            // Assets
            var input = "05-2020";
            var type = typeof(DateTime);

            // Act
            var method = type.TryParse(input, out var output);

            var expected = new DateTime(2020, 05, 01);
            DateTime.SpecifyKind(expected, DateTimeKind.Utc);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<DateTime>(output);
            Assert.Equal(expected, output);
        }

        [Fact]
        public void CallTrySetValue_DateTime5()
        {
            // Assets
            var input = "2020/05";
            var type = typeof(DateTime);

            // Act
            var method = type.TryParse(input, out var output);

            var expected = new DateTime(2020, 05, 01);
            DateTime.SpecifyKind(expected, DateTimeKind.Utc);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<DateTime>(output);
            Assert.Equal(expected, output);
        }

        [Fact]
        public void CallTrySetValue_DateTime6()
        {
            // Assets
            var input = "Arash";
            var type = typeof(DateTime);

            // Act
            var method = type.TryParse(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_DateTime7()
        {
            // Assets
            string input = null;
            var type = typeof(DateTime);

            // Act
            var method = type.TryParse(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_Boolean()
        {
            // Assets
            var input = true.ToString();
            var type = typeof(bool);

            // Act
            var method = type.TryParse(input, out var output);

            // Arranges
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<bool>(output);
            Assert.True((bool)output);
        }

        [Fact]
        public void CallTrySetValue_Boolean3()
        {
            // Assets
            var input = "off";
            var type = typeof(bool);

            // Act
            var method = type.TryParse(input, out var output);

            // Arranges
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<bool>(output);
            Assert.False((bool)output);
        }

        [Fact]
        public void CallTrySetValue_Boolean2()
        {
            // Assets
            var input = "100";
            var type = typeof(bool);

            // Act
            var method = type.TryParse(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_Int()
        {
            // Assets
            var input = "1000000000000";
            var type = typeof(int);

            // Act
            var method = type.TryParse(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_Int2()
        {
            // Assets
            var input = "1000000000000000000000";
            var type = typeof(int);

            // Act
            var method = type.TryParse(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_Int3()
        {
            // Assets
            var input = "100";
            var type = typeof(int);

            // Act
            var method = type.TryParse(input, out var output);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<int>(output);
            Assert.Equal(100, output);
        }

        [Fact]
        public void CallTrySetValue_Int4()
        {
            // Assets
            var input = "100.2";
            var type = typeof(int);

            // Act
            var method = type.TryParse(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_String()
        {
            // Assets
            var input = (string)null;
            var type = typeof(string);

            // Act
            var method = type.TryParse(input, out var output);

            // Arrange
            Assert.True(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_Double()
        {
            // Assets
            var input = "100..2";
            var type = typeof(double);

            // Act
            var method = type.TryParse(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_Double2()
        {
            // Assets
            var input = "100.2";
            var type = typeof(double);

            // Act
            var method = type.TryParse(input, out var output);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<double>(output);
            Assert.Equal(100.2, output);
        }

        [Fact]
        public void CallTrySetValue_Long()
        {
            // Assets
            var input = "100";
            var type = typeof(long);

            // Act
            var method = type.TryParse(input, out var output);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<long>(output);
        }

        [Fact]
        public void CallTrySetValue_Long2()
        {
            // Assets
            var input = "100.1";
            var type = typeof(long);

            // Act
            var method = type.TryParse(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_Long3()
        {
            // Assets
            var input = "Fake";
            var type = typeof(long);

            // Act
            var method = type.TryParse(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_Enum4()
        {
            // Assets
            var input = "SUCCESS";
            var type = typeof(Flags);

            // Act
            var method = type.TryParse(input, out var output);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.Equal(Flags.Success, output);
            Assert.IsType<Flags>(output);
        }

        [Fact]
        public void CallTrySetValue_Enum3()
        {
            // Assets
            var input = 100.ToString();
            var type = typeof(Flags);

            // Act
            var method = type.TryParse(input, out var output);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.Equal(Flags.Success, output);
            Assert.IsType<Flags>(output);
        }

        [Fact]
        public void CallTrySetValue_Enum()
        {
            // Assets
            var input = Flags.Success.ToString();
            var type = typeof(Flags);

            // Act
            var method = type.TryParse(input, out var output);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.Equal(Flags.Success, output);
            Assert.IsType<Flags>(output);
        }

        [Fact]
        public void CallTrySetValue_Enum2()
        {
            // Assets
            var input = "Fake";
            var type = typeof(Flags);

            // Act
            var method = type.TryParse(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallHasBaseType2()
        {
            // Assets
            Expression<Func<FakeObj, string>> func = o => o.LastName;

            // Act
            var act = typeof(FakeObj).HasRootType(typeof(string));

            Assert.False(act);
        }
    }
}