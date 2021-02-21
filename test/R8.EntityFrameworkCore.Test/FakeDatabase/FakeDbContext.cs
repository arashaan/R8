using Microsoft.EntityFrameworkCore;

using R8.EntityFrameworkCore.Test.FakeDatabase.FakeEntities;

namespace R8.EntityFrameworkCore.Test.FakeDatabase
{
    public class FakeDbContext : DbContext
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

    public FakeDbContext(DbContextOptions options) : base(options)
    {
    }
}
}