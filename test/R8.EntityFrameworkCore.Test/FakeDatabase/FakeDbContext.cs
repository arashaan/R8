using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using R8.EntityFrameworkCore.Test.FakeDatabase.FakeEntities;
using R8.Lib;

using System;

namespace R8.EntityFrameworkCore.Test.FakeDatabase
{
    public class FakeDbContext : DbContext, IAuditGenerator
    {
        public FakeDbContext()
        {
        }

        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ScanConfigurations(this);
            base.OnModelCreating(modelBuilder);
        }

        public Func<EntityEntry, AuditOptions> AuditOptions => entry => new AuditOptions
        {
            LocalIpAddress = HttpExtensions.GetLocalIPAddress(),
            RemoteIpAddress = HttpExtensions.GetIPAddressAsync().GetAwaiter().GetResult(),
            UserAgent = "WinDesktop"
        };

        public FakeDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}