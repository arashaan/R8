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

        //[Fact]
        //public async Task CallWhereJson()
        //{
        //    // Assets
        //    var creatorGuid = Guid.NewGuid();

        //    // Act
        //    await using var dbContext = new FakeDbContext(FakeDbContextFactory.GetDbContextOptions());
        //    {
        //        await dbContext.Database.EnsureCreatedAsync();

        //        var container = new LocalizerContainer("Admin");
        //        container.Set("tr", "Destek");
        //        container.Set("fa", "پشتیبان");
        //        var role = new Role
        //        {
        //            Name = container,
        //            Slug = "admin",
        //        };

        //        var roleCreated = dbContext.Add(role);
        //        var saveStatus = await dbContext.SaveChangesFullAsync();

        //        var query = dbContext.Roles.WhereJson("tr", "Destek");
        //        var doWe = query.FirstOrDefault();
        //        var sql = query.ToQueryString();

        //        await dbContext.Database.EnsureDeletedAsync();

        //        // Arrange
        //        Assert.NotNull(doWe);
        //        Assert.Equal("admin", doWe.Slug);
        //        Assert.Equal("Destek", doWe.Name.Get("tr"));
        //    }
        //}
    }
}