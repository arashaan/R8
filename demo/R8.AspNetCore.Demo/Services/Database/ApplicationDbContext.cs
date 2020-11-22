using Microsoft.EntityFrameworkCore;

using R8.AspNetCore.Demo.Services.Database.Entities;
using R8.EntityFrameworkCore;

namespace R8.AspNetCore.Demo.Services.Database
{
    public class ApplicationDbContext : DbContextBase
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Translation> Translation { get; set; }
    }
}