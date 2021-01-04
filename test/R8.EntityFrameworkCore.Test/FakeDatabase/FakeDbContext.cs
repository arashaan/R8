using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;

using R8.EntityFrameworkCore;

namespace R8.EntityFrameworkCore.Test.FakeDatabase
{
    public class FakeDbContext : DbContextBase
    {
        public FakeDbContext()
        {
        }

        public FakeDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}