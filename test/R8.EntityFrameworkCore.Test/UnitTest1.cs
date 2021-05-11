using R8.EntityFrameworkCore.Test.FakeDatabase;
using R8.EntityFrameworkCore.Test.FakeDatabase.FakeEntities;
using R8.Lib;
using R8.Lib.Localization;

using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace R8.EntityFrameworkCore.Test
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _output;

        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task CallDbContextConnection()
        {
            // Assets

            // Act
            await using var dbContext = new FakeDbContext(FakeDbRunner.CreateNewContextOptions());
            {
                var isCreated = await dbContext.Database.EnsureCreatedAsync();
                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.NotNull(dbContext);
                Assert.True(isCreated);
            }
        }

        [Fact]
        public void CallGetTableName()
        {
            var type = typeof(User);

            var tableName = R8.EntityFrameworkCore.Extensions.GetTableName(type);

            Assert.NotNull(tableName);
            Assert.Equal("Users", tableName);
            ;
        }

        [Fact]
        public async Task CallUpdate()
        {
            // Assets
            var creatorGuid = Guid.NewGuid();

            // Act
            await using var dbContext = new FakeDbContext(FakeDbRunner.CreateNewContextOptions());
            {
                await dbContext.Database.EnsureCreatedAsync();

                var role = new Role
                {
                    Name = new LocalizerContainer("Admin"),
                    CanonicalName = "admin",
                };
                var user = new User
                {
                    Username = "Arash",
                    Password = "Shabbeh",
                };

                var roleCreated = dbContext.Add(role, out _);
                var userCreated = dbContext.Add(user, out _);
                user.RoleId = role.Id;
                var userUpdated = dbContext.Update(user, creatorGuid, out var userErrors);

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.True(roleCreated);
                Assert.True(userCreated);
                Assert.True(userUpdated);
                Assert.Empty(userErrors);

                Assert.NotEqual(Guid.Empty, user.Id);
                Assert.NotEqual(Guid.Empty, role.Id);
                Assert.Equal(role.Id, user.RoleId);
            }
        }

        [Fact]
        public async Task CallAuditGeneratorByOptions()
        {
            // Assets
            var creatorGuid = Guid.NewGuid();

            // Act
            await using var dbContext = new FakeDbContext(FakeDbRunner.CreateNewContextOptions());
            {
                await dbContext.Database.EnsureCreatedAsync();

                var role = new Role
                {
                    Name = new LocalizerContainer("Admin"),
                    CanonicalName = "admin",
                };
                var roleCreated = dbContext.Add(role, out _);
                var saveStatus = await dbContext.SaveAsync();

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange

                Assert.NotEqual(Guid.Empty, role.Id);
                Assert.Equal(DatabaseSaveState.Saved, saveStatus);
                Assert.Equal("WinDesktop", role.Audits[0].UserAgent);
                Assert.Equal(HttpExtensions.GetLocalIPAddress(), role.Audits[0].LocalIpAddress);
                Assert.Equal(HttpExtensions.GetIPAddressAsync().GetAwaiter().GetResult(), role.Audits[0].RemoteIpAddress);
            }
        }

        [Fact]
        public async Task CallSaveAsync()
        {
            // Assets
            var creatorGuid = Guid.NewGuid();

            // Act
            await using var dbContext = new FakeDbContext(FakeDbRunner.CreateNewContextOptions());
            {
                await dbContext.Database.EnsureCreatedAsync();

                var role = new Role
                {
                    Name = new LocalizerContainer("Admin"),
                    CanonicalName = "admin",
                };
                var user = new User
                {
                    Username = "Arash",
                    Password = "Shabbeh",
                };

                var roleCreated = dbContext.Add(role, out _);
                var userCreated = dbContext.Add(user, out _);
                var saveStatus = await dbContext.SaveAsync();

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.Equal(DatabaseSaveState.Saved, saveStatus);
            }
        }

        [Fact]
        public async Task CallWhereHas()
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

                var timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                var doWe = dbContext.Roles.WhereLocalized("admin", "Admin").FirstOrDefault();
                timer.Stop();

                _output.WriteLine(timer.Elapsed.ToString("c"));

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.NotNull(doWe);
                Assert.Equal("admin", doWe.CanonicalName);
                Assert.Equal("Destek", doWe.Name.Get("tr"));
            }
        }

        [Fact]
        public async Task CallWhereHas2()
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

                var timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                var doWe = dbContext.Roles.WhereJson("tr", "Destek").FirstOrDefault();
                timer.Stop();

                _output.WriteLine(timer.Elapsed.ToString("c"));

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.NotNull(doWe);
                Assert.Equal("admin", doWe.CanonicalName);
                Assert.Equal("Destek", doWe.Name.Get("tr"));
            }
        }

        [Fact]
        public async Task CallAdd5()
        {
            // Assets
            var creatorGuid = Guid.NewGuid();

            // Act
            await using var dbContext = new FakeDbContext(FakeDbRunner.CreateNewContextOptions());
            {
                await dbContext.Database.EnsureCreatedAsync();
                var user = new User
                {
                    Username = "Arash",
                    Password = "Shabbeh",
                };
                dbContext.Add(user, out _);
                user.Username = new LocalizerContainer("iamr8");
                dbContext.Update(user, creatorGuid, out _);

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.NotEmpty(user.Audits);

                var lastAudit = user.Audits.Last;

                Assert.Contains(lastAudit.OldValues, x => x.Key == "Username" && x.Value.ToString() == "Arash");
                Assert.Contains(lastAudit.NewValues, x => x.Key == "Username" && x.Value.ToString() == "iamr8");
            }
        }

        [Fact]
        public async Task CallAdd4()
        {
            // Assets
            var creatorGuid = Guid.NewGuid();

            // Act
            await using var dbContext = new FakeDbContext(FakeDbRunner.CreateNewContextOptions());
            {
                await dbContext.Database.EnsureCreatedAsync();
                var role = new Role
                {
                    Name = new LocalizerContainer("Admin"),
                    CanonicalName = "admin",
                };
                dbContext.Add(role, out _);
                role.Name = new LocalizerContainer("Client");
                dbContext.Update(role, creatorGuid, out _);

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.NotEmpty(role.Audits);

                var lastAudit = role.Audits.Last;

                Assert.Equal(AuditFlags.Changed, lastAudit.Flag);
                Assert.NotEmpty(lastAudit.OldValues);
                Assert.NotEmpty(lastAudit.NewValues);
                Assert.Contains(lastAudit.OldValues, x => x.Key == "NameJson");
            }
        }

        [Fact]
        public async Task CallAdd6()
        {
            // Assets
            var creatorGuid = Guid.NewGuid();

            // Act
            await using var dbContext = new FakeDbContext(FakeDbRunner.CreateNewContextOptions());
            {
                await dbContext.Database.EnsureCreatedAsync();
                var role = new Role
                {
                    Name = new LocalizerContainer("Admin"),
                    CanonicalName = "admin",
                };
                dbContext.Add(role, out _);
                var roleHide = dbContext.UnHide(role, creatorGuid);

                await dbContext.Database.EnsureDeletedAsync();

                Assert.False(roleHide);
            }
        }

        [Fact]
        public async Task CallAdd9()
        {
            // Assets
            var creatorGuid = Guid.NewGuid();

            // Act
            await using var dbContext = new FakeDbContext(FakeDbRunner.CreateNewContextOptions());
            {
                await dbContext.Database.EnsureCreatedAsync();
                var role = new Role
                {
                    Name = new LocalizerContainer("Admin"),
                    CanonicalName = "admin",
                };
                dbContext.Add(role, out _);
                dbContext.Hide(role, creatorGuid);
                dbContext.UnHide(role, creatorGuid);

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.NotEmpty(role.Audits);

                var lastAudit = role.Audits.Last;

                Assert.NotNull(lastAudit);
                Assert.Equal(AuditFlags.UnDeleted, lastAudit.Flag);
            }
        }

        [Fact]
        public async Task CallAdd8()
        {
            // Assets
            var creatorGuid = Guid.NewGuid();

            // Act
            await using var dbContext = new FakeDbContext(FakeDbRunner.CreateNewContextOptions());
            {
                await dbContext.Database.EnsureCreatedAsync();
                var role = new Role
                {
                    Name = new LocalizerContainer("Admin"),
                    CanonicalName = "admin",
                };
                dbContext.Add(role, out _);
                dbContext.Hide(role, creatorGuid);

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.NotEmpty(role.Audits);

                var lastAudit = role.Audits.Last;

                Assert.NotNull(lastAudit);
                Assert.Equal(AuditFlags.Deleted, lastAudit.Flag);
            }
        }

        [Fact]
        public async Task CallAdd10()
        {
            // Assets
            var creatorGuid = Guid.NewGuid();

            // Act
            await using var dbContext = new FakeDbContext(FakeDbRunner.CreateNewContextOptions());
            {
                await dbContext.Database.EnsureCreatedAsync();
                var role = new Role
                {
                    Name = new LocalizerContainer("Admin"),
                    CanonicalName = "admin",
                };
                dbContext.Add(role, out _);
                dbContext.ToggleHiding(role, creatorGuid);

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.NotEmpty(role.Audits);

                var lastAudit = role.Audits.Last;

                Assert.NotNull(lastAudit);
                Assert.Equal(AuditFlags.Deleted, lastAudit.Flag);
            }
        }

        [Fact]
        public async Task CallAdd7()
        {
            // Assets
            var creatorGuid = Guid.NewGuid();

            // Act
            await using var dbContext = new FakeDbContext(FakeDbRunner.CreateNewContextOptions());
            {
                await dbContext.Database.EnsureCreatedAsync();
                var role = new Role
                {
                    Name = new LocalizerContainer("Admin"),
                    CanonicalName = "admin",
                };
                dbContext.Add(role, out _);
                var roleHide = dbContext.Hide(role, creatorGuid);

                await dbContext.Database.EnsureDeletedAsync();

                Assert.True(roleHide);
            }
        }

        [Fact]
        public async Task CallAdd3()
        {
            // Assets
            var creatorGuid = Guid.NewGuid();
            var localIp = HttpExtensions.GetLocalIPAddress();
            var remoteIp = HttpExtensions.GetIPAddressAsync().GetAwaiter().GetResult();

            // Act
            await using var dbContext = new FakeDbContext(FakeDbRunner.CreateNewContextOptions());
            {
                await dbContext.Database.EnsureCreatedAsync();

                var role = new Role
                {
                    Name = new LocalizerContainer("Admin"),
                    CanonicalName = "admin",
                };

                dbContext.Add(role, out _);

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.NotEmpty(role.Audits);

                var lastAudit = role.Audits.Last;
                var createdAudit = role.Audits.Created;

                Assert.NotNull(lastAudit);
                Assert.InRange(lastAudit.Id, 0, 9999999);
                Assert.Equal(lastAudit, createdAudit);
                Assert.Equal(AuditFlags.Created, lastAudit.Flag);
                Assert.Equal(localIp, lastAudit.LocalIpAddress);
                Assert.Equal(remoteIp, lastAudit.RemoteIpAddress);
            }
        }

        [Fact]
        public async Task CallUpdate2()
        {
            // Assets
            var creatorGuid = Guid.NewGuid();

            // Act
            await using var dbContext = new FakeDbContext(FakeDbRunner.CreateNewContextOptions());
            {
                await dbContext.Database.EnsureCreatedAsync();

                var role = new Role
                {
                    Name = new LocalizerContainer("Admin"),
                    CanonicalName = "admin",
                };
                var user = new User
                {
                    Username = "Arash",
                };

                var roleCreated = dbContext.Add(role, out _);
                var userCreated = dbContext.Add(user, out _);
                user.RoleId = role.Id;
                var userUpdated = dbContext.Update(user, creatorGuid, out var userErrors);

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.True(roleCreated);
                Assert.False(userCreated);
                Assert.False(userUpdated);

                Assert.Equal(Guid.Empty, user.Id);
                Assert.NotEqual(Guid.Empty, role.Id);
                Assert.Equal(role.Id, user.RoleId);
            }
        }
    }
}