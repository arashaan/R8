using Microsoft.EntityFrameworkCore;

using R8.EntityFrameworkCore.Test.FakeDatabase;
using R8.EntityFrameworkCore.Test.FakeDatabase.FakeEntities;
using R8.Lib.Localization;

using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace R8.EntityFrameworkCore.Test
{
    public class EntityLocalizedExtensionsTests
    {
        private readonly ITestOutputHelper _output;

        public EntityLocalizedExtensionsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task CallWhereJson()
        {
            // Assets
            var creatorGuid = Guid.NewGuid();

            // Act
            await using var dbContext = new FakeDbContext(FakeDbRunner.CreateNewContextOptions());
            {
                await dbContext.Database.EnsureCreatedAsync();

                var container = new LocalizerContainer("Admin");
                container.Set("tr", "Destek");
                container.Set("fa", "پشتیبان");
                var role = new Role
                {
                    Name = container,
                    CanonicalName = "admin",
                };

                var roleCreated = dbContext.Add(role, out _);
                var saveStatus = await dbContext.SaveAsync();

                var query = dbContext.Roles.WhereJson("tr", "Destek");
                var doWe = query.FirstOrDefault();
                var sql = query.ToQueryString();

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.NotNull(doWe);
                Assert.Equal("admin", doWe.CanonicalName);
                Assert.Equal("Destek", doWe.Name.Get("tr"));
            }
        }
    }
}