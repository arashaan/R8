using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace R8.EntityFrameworkCore.Test.FakeDatabase
{
    public static class FakeDbRunner
    {
        public static DbContextOptions<FakeDbContext> CreateNewContextOptions()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<FakeDbContext>();
            builder.UseInMemoryDatabase("FakeDbContext")
                .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
    }
}