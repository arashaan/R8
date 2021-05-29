using Microsoft.EntityFrameworkCore;

using R8.EntityFrameworkCore.Audits;
using R8.EntityFrameworkCore.Test.FakeDatabase;
using R8.EntityFrameworkCore.Test.FakeDatabase.FakeEntities;
using R8.Lib.Localization;

using System;
using System.Globalization;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace R8.EntityFrameworkCore.Test
{
    public class AuditProviderInterceptorTests
    {
        private readonly DbContextOptions<FakeDbContext> _dbContextOptions;
        private readonly ITestOutputHelper _outputHelper;

        public AuditProviderInterceptorTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _dbContextOptions = FakeDbContext.GetOptions();
        }

        [Fact]
        public async Task CallUpdate()
        {
            // Act
            await using var dbContext = new FakeDbContext(FakeDbContext.GetOptions());
            {
                await dbContext.Database.EnsureCreatedAsync();
                var user = new User
                {
                    Username = "Arash",
                    Password = "Shabbeh",
                };
                dbContext.Add(user);
                await dbContext.SaveChangesAsync();

                user.Username = "iamr8";
                dbContext.Update(user);

                await dbContext.SaveChangesAsync();
                await dbContext.Database.EnsureDeletedAsync();

                _outputHelper.WriteLine(user.Audits.Last.Id.ToString());

                // Arrange
                Assert.NotEmpty(user.Audits);
                Assert.Equal(2, user.Audits.Count);
                Assert.Equal(AuditFlags.Created, user.Audits.Created.Flag);
                Assert.Equal(AuditFlags.Changed, user.Audits.Last.Flag);
                Assert.NotEqual(Guid.Empty, user.Audits.Last.Id);
                Assert.NotEmpty(user.Audits.Last.Changes);
                Assert.Contains(user.Audits.Last.Changes, x => x.Key == "Username" && x.OldValue.ToString() == "Arash");
                Assert.Contains(user.Audits.Last.Changes, x => x.Key == "Username" && x.NewValue.ToString() == "iamr8");
            }
        }

        [Fact]
        public async Task CallUpdateLocalizerContainer()
        {
            // Act
            await using var dbContext = new FakeDbContext(FakeDbContext.GetOptions());
            {
                await dbContext.Database.EnsureCreatedAsync();
                var role = new Role
                {
                    Name = new LocalizerContainer(CultureInfo.GetCultureInfo("en"), "Admin"),
                    Slug = "admin",
                };
                dbContext.Add(role);
                await dbContext.SaveChangesAsync();

                role.UpdateName(CultureInfo.GetCultureInfo("en"), "Client");
                dbContext.Update(role);

                await dbContext.SaveChangesAsync();
                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.NotEmpty(role.Audits);
                Assert.Equal(2, role.Audits.Count);
                Assert.Equal(AuditFlags.Changed, role.Audits.Last.Flag);
                Assert.NotEqual(Guid.Empty, role.Audits.Last.Id);
                Assert.NotEmpty(role.Audits.Last.Changes);
                Assert.Contains(role.Audits.Last.Changes, x => x.Key == nameof(IEntityLocalized.NameJson));
                Assert.Contains("Client",
                    role.Audits.Last.Changes[nameof(IEntityLocalized.NameJson)].NewValue.ToString());
            }
        }

        [Fact]
        public async Task CallAdd()
        {
            await using var dbContext = new FakeDbContext(_dbContextOptions);
            {
                await dbContext.Database.EnsureCreatedAsync();

                var role = new Role
                {
                    Name = new LocalizerContainer("Admin"),
                    Slug = "admin",
                };

                dbContext.Add(role);

                await dbContext.SaveChangesAsync();
                await dbContext.Database.EnsureDeletedAsync();

                Assert.NotEmpty(role.Audits);

                var lastAudit = role.Audits.Last;
                var createdAudit = role.Audits.Created;

                Assert.NotNull(lastAudit);
                Assert.Equal(lastAudit, createdAudit);
                Assert.IsType<FakeAuditAdditional>(lastAudit.Additional);
                Assert.Equal(CultureInfo.GetCultureInfo("en-US"), ((FakeAuditAdditional)lastAudit.Additional).Culture);
                Assert.Equal("MemoryCache", ((FakeAuditAdditional)lastAudit.Additional).Text);
                Assert.Equal(AuditFlags.Created, lastAudit.Flag);
            }
        }

        [Fact]
        public async Task CallComplexDeleteManually()
        {
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
                await dbContext.SaveChangesAsync();

                role.IsDeleted = true;
                dbContext.Update(role);
                await dbContext.SaveChangesAsync();

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.NotEmpty(role.Audits);
                Assert.Equal(2, role.Audits.Count);
                Assert.Equal(AuditFlags.Deleted, role.Audits.Last.Flag);
            }
        }

        [Fact]
        public async Task CallComplexWithDeleteNullAndWithoutPermanentDelete()
        {
            // Act
            await using var dbContext = new FakeDbContext(FakeDbContext.GetOptionsWithoutDeleteColumn());
            {
                await dbContext.Database.EnsureCreatedAsync();
                var role = new Role
                {
                    Name = new LocalizerContainer("Admin"),
                    Slug = "admin",
                };
                dbContext.Add(role);
                await Assert.ThrowsAsync<NullReferenceException>(() => dbContext.SaveChangesAsync());

                await dbContext.Database.EnsureDeletedAsync();
            }
        }

        [Fact]
        public async Task CallComplexPermanentDelete()
        {
            // Act
            await using var dbContext = new FakeDbContext(FakeDbContext.GetOptionsPermanentDelete());
            {
                await dbContext.Database.EnsureCreatedAsync();
                var role = new Role
                {
                    Name = new LocalizerContainer("Admin"),
                    Slug = "admin",
                };
                dbContext.Add(role);
                await dbContext.SaveChangesAsync();

                dbContext.Remove(role);
                await dbContext.SaveChangesAsync();

                var entity = await dbContext.Roles.FirstOrDefaultAsync(x => x.Slug == "admin");

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.Null(entity);
            }
        }

        [Fact]
        public async Task CallComplex()
        {
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
                await dbContext.SaveChangesAsync();

                dbContext.Remove(role);
                await dbContext.SaveChangesAsync();

                dbContext.UnRemove(role);
                await dbContext.SaveChangesAsync();

                role.Slug = "client";
                dbContext.Update(role);
                await dbContext.SaveChangesAsync();

                role = await dbContext.Roles.FirstOrDefaultAsync(x => x.Slug == "client");

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.NotNull(role);
                Assert.NotEmpty(role.Audits);
                Assert.Equal(4, role.Audits.Count);
                Assert.Equal(AuditFlags.Changed, role.Audits.Last.Flag);
            }
        }

        [Fact]
        public async Task CallDuplicateDelete()
        {
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
                await dbContext.SaveChangesAsync();

                dbContext.Remove(role);
                await dbContext.SaveChangesAsync();

                dbContext.Remove(role);
                await dbContext.SaveChangesAsync();

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.NotNull(role);
                Assert.NotEmpty(role.Audits);
                Assert.Equal(2, role.Audits.Count);
                Assert.Equal(AuditFlags.Deleted, role.Audits.Last.Flag);
            }
        }

        [Fact]
        public async Task CallAuditChangeRelation()
        {
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
                await dbContext.SaveChangesAsync();

                var user = new User
                {
                    Password = "123",
                    Username = "Arash",
                };
                dbContext.Add(user);
                await dbContext.SaveChangesAsync();

                user.RoleId = role.Id;
                dbContext.Update(user);
                await dbContext.SaveChangesAsync();

                var change = user.Audits.Last.Changes[nameof(User.RoleId)];
                var entity = await change.GetNavigationEntityObjectAsync(dbContext);

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.NotNull(entity);
                Assert.IsType<Role>(entity);
                Assert.Equal(role.Id, ((Role)entity).Id);
            }
        }

        [Fact]
        public async Task CallSaveAsync()
        {
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
                var saveStatus = await dbContext.SaveChangesFullAsync();
                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.Equal(DatabaseSaveState.Saved, saveStatus);
                Assert.Single(role.Audits);
                Assert.Equal(AuditFlags.Created, role.Audits.Last.Flag);
                Assert.Equal(role.Audits.Created, role.Audits.Last);
            }
        }
    }
}