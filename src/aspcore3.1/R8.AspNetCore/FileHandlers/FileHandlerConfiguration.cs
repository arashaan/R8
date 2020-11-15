using Microsoft.AspNetCore.Hosting;

using R8.FileHandlers;

using System.Collections.Generic;

namespace R8.AspNetCore.FileHandlers
{
    public class FileHandlerConfiguration : IMyFileConfigurationRouting
    {
        public IWebHostEnvironment Environment { get; private set; }

        public string Folder { get; set; }

        public bool? HierarchicallyFolderNameByDate { get; set; }

        public string RootPath { get; set; }

        public bool? RealFilename { get; set; }

        public bool? OverwriteFile { get; set; }

        /// <summary>
        /// Gets or sets configurations.
        /// </summary>
        public List<IMyFileRuntime> Runtimes { get; set; } = new List<IMyFileRuntime>();

        internal void SetWebHost(IWebHostEnvironment env)
        {
            Environment = env;
        }
    }
}