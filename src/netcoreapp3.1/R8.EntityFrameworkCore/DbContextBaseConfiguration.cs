using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace R8.EntityFrameworkCore
{
    /// <summary>
    /// A <see cref="DbContextBaseConfiguration"/> instance for <see cref="DbContextBase"/> configuration.
    /// </summary>
    public class DbContextBaseConfiguration
    {
        /// <summary>
        /// Gets or sets an <see cref="string"/> value that representing connection string to specific database.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets namespace for specific project to containing migration assemblies.
        /// </summary>
        public string MigrationAssembly { get; set; }

        /// <summary>
        /// Gets or sets time-out for commands in type of <see cref="TimeSpan"/>.
        /// </summary>
        /// <remarks>default is <c>TimeSpan.FromMinutes(3)</c></remarks>
        public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromMinutes(3);

        /// <summary>
        /// Gets or sets a logger factory for logging sensitive commands of database.
        /// </summary>
        public ILoggerFactory LoggerFactory { get; set; }

        /// <summary>
        /// Gets of sets a <see cref="Action{TResult}"/> for more accuracy.
        /// </summary>
        public Action<SqlServerDbContextOptionsBuilder> Action { get; set; }

        //public Func<HttpRequest, DbContextRequestConfig> RequestConfiguration { get; set; }
    }
}