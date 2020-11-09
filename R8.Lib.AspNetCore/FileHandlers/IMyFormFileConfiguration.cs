using Microsoft.AspNetCore.Hosting;

using R8.Lib.FileHandlers;

using System;

namespace R8.Lib.AspNetCore.FileHandlers
{
    /// <summary>
    /// Initializes an <see cref="MyFormFileConfiguration"/> instance.
    /// </summary>
    public class MyFormFileConfiguration : MyFileConfiguration
    {
        private IWebHostEnvironment _environment;

        /// <summary>
        /// Initializes an <see cref="MyFormFileConfiguration"/> instance.
        /// </summary>
        public MyFormFileConfiguration()
        {
        }

        /// <summary>
        /// Initializes an <see cref="MyFormFileConfiguration"/> instance.
        /// </summary>
        /// <param name="environment">A <see cref="IWebHostEnvironment"/> instance.</param>
        public MyFormFileConfiguration(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        /// <summary>
        /// Applies <see cref="IWebHostEnvironment"/> to the builder to get wwwroot folder.
        /// </summary>
        /// <param name="environment">A <see cref="IWebHostEnvironment"/> instance.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void ApplyEnvironment(IWebHostEnvironment environment)
        {
            this._environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }
    }
}