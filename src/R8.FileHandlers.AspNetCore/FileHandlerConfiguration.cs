using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;

namespace R8.FileHandlers.AspNetCore
{
    public class FileHandlerConfiguration : IMyFileConfigurationRouting
    {
        public FileHandlerConfiguration(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public IWebHostEnvironment Environment { get; }

        public string Path { get; set; }

        public bool? HierarchicallyDateFolders { get; set; }

        public bool? SaveAsRealName { get; set; }

        public bool? OverwriteExistingFile { get; set; }

        /// <summary>
        /// Gets or sets configurations.
        /// </summary>
        public List<IMyFileRuntime> Runtimes { get; set; } = new List<IMyFileRuntime>();
    }
}