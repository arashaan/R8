using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using R8.EntityFrameworkCore.Test.FakeDatabase;
using R8.EntityFrameworkCore.Test.FakeDatabase.FakeEntities;
using R8.Lib.Localization;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Xunit;

namespace R8.EntityFrameworkCore.Test
{
    public class ExtensionsTests
    {
        [Fact]
        public async Task CallGetExpressionTreeAsync()
        {
            // Assets
            var creatorGuid = Guid.NewGuid();

            // Act
            await using var dbContext = new FakeDbContext(FakeDbContext.GetOptions());
            {
                await dbContext.Database.EnsureCreatedAsync();
                var role = new Role
                {
                    Name = new LocalizerContainer("Admin"),
                    Slug = "admin",
                };
                dbContext.Add(role);
                dbContext.Remove(role);
                var roleQuery = dbContext.Roles
                    .AsNoTracking()
                    .Where(x => x.Slug == "admin")
                    .Select(x => new { x.Slug });

                var queryableTree = roleQuery.GetExpressionTree().ToList();

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.NotNull(queryableTree);
                Assert.NotEmpty(queryableTree);
                Assert.Equal(typeof(QueryRootExpression), queryableTree[0].GetType());
                Assert.Equal("AsNoTracking", ((MethodCallExpression)queryableTree[1]).Method.Name);
                Assert.Equal("Where", ((MethodCallExpression)queryableTree[2]).Method.Name);
                Assert.Equal("Select", ((MethodCallExpression)queryableTree[3]).Method.Name);
            }
        }

        [Fact]
        public void CallGetTableName()
        {
            var type = typeof(User);

            var tableName = Extensions.GetTableName(type);

            Assert.NotNull(tableName);
            Assert.Equal("Users", tableName);
            ;
        }
    }
}