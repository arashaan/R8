using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using R8.EntityFrameworkCore.Test.FakeDatabase;

using Xunit;
using Xunit.Abstractions;
using R8.EntityFrameworkCore.Test;
using R8.EntityFrameworkCore.Test.FakeDatabase.FakeEntities;
using R8.Lib;
using R8.Lib.Enums;
using R8.Lib.Localization;

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
        public async Task CallAdd()
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

                var roleCreated = dbContext.Add(role, creatorGuid, out _);
                var userCreated = dbContext.Add(user, creatorGuid, out _);
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
        public async Task CallAdd11()
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

                var roleCreated = dbContext.Add(role, creatorGuid, out _);
                var userCreated = dbContext.Add(user, creatorGuid, out _);
                var saveStatus = await dbContext.SaveChangesAsync();

                await dbContext.Database.EnsureDeletedAsync();

                // Arrange
                Assert.Equal(DatabaseSaveState.Saved, saveStatus);
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
                dbContext.Add(user, creatorGuid, out _);
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
                dbContext.Add(role, creatorGuid, out _);
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
                dbContext.Add(role, creatorGuid, out _);
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
                dbContext.Add(role, creatorGuid, out _);
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
                dbContext.Add(role, creatorGuid, out _);
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
                dbContext.Add(role, creatorGuid, out _);
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
                dbContext.Add(role, creatorGuid, out _);
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
            var remoteIp = HttpExtensions.GetIPAddress();

            // Act
            await using var dbContext = new FakeDbContext(FakeDbRunner.CreateNewContextOptions());
            {
                await dbContext.Database.EnsureCreatedAsync();

                var role = new Role
                {
                    Name = new LocalizerContainer("Admin"),
                    CanonicalName = "admin",
                };

                dbContext.Add(role, creatorGuid, out _);

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
        public async Task CallAdd2()
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

                var roleCreated = dbContext.Add(role, creatorGuid, out _);
                var userCreated = dbContext.Add(user, creatorGuid, out _);
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