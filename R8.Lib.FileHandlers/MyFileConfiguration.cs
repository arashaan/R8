using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace R8.Lib.FileHandlers
{
    /// <summary>
    /// An <see cref="MyFileConfiguration"/> abstract class that representing configuration for Save Files.
    /// </summary>
    public class MyFileConfiguration
    {
        /// <summary>
        /// Gets or sets an <see cref="bool"/> value that avoid to save file into disk. For testing.
        /// </summary>
        /// <remarks>This is an internal API, and may be removed in future versions.</remarks>
        public bool TestDevelopment { get; set; }

        /// <summary>
        /// Gets or sets a collection of objects.
        /// </summary>
        private List<object> _services = new List<object>();

        /// <summary>
        /// Gets or sets an <see cref="string"/> value that representing root folder for uploading files.
        /// </summary>
        /// <remarks>default: <c>/uploads</c></remarks>
        public string Folder { get; set; } = "/uploads";

        /// <summary>
        /// Gets or sets an <see cref="bool"/> that representing hierarchically directory names from root to file.
        /// If true hierarchically folders, otherwise right in <see cref="Folder"/>.
        /// example: <example><c>/<see cref="Folder"/>/2020/10/03/file.xyz</c></example>.
        /// </summary>
        /// <remarks>default: <c>true</c></remarks>
        public bool HierarchicallyFolderNameByDate { get; set; } = true;

        /// <summary>
        /// Gets or sets an <see cref="bool"/> value that representing if file should be named as real name or <c>Guid.NewGuid</c>.
        /// </summary>
        /// <remarks>default: <c>false</c></remarks>
        public bool RealFilename { get; set; } = false;

        /// <summary>
        /// Gets or sets an <see cref="bool"/> value that overwrite on existing file or create new file with incremented number
        /// </summary>
        /// <remarks>default: <c>false</c></remarks>
        public bool OverwriteFile { get; set; } = false;

        /// <summary>
        /// Returns specified type from services.
        /// </summary>
        /// <typeparam name="T">A generic type of service that already registered.</typeparam>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A instance with specified type.</returns>
        public T GetService<T>() where T : class
        {
            if (_services == null)
                throw new NullReferenceException(nameof(_services));

            var service = _services.OfType<T>().FirstOrDefault();
            return service;
        }

        /// <summary>
        /// Returns specified type from services.
        /// </summary>
        /// <param name="type">An instance of generic type that has been added to services.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A instance with specified type.</returns>
        public object GetService(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (_services == null)
                throw new NullReferenceException(nameof(_services));

            var service = _services.Find(x => x.GetType() == type);
            return service;
        }

        /// <summary>
        /// Adds specific type of class to services.
        /// </summary>
        /// <typeparam name="T">A generic type of service.</typeparam>
        /// <param name="type">An instance of generic type to be added to services.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddService<T>(T type) where T : class
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            _services.Add(type);
        }

        /// <summary>
        /// Returns final file path
        /// </summary>
        /// <param name="currentFileName">An <see cref="string"/> value that representing real file name</param>
        /// <param name="fileExtension"></param>
        /// <returns>An <see cref="string"/> value that representing file path</returns>
        /// <remarks>If you don't want to Get Real File Path, you can leave <c>currentFileName</c> null and fill <c>fileExtension</c>, and otherwise if you do need for real file name, you have to fill <c>currentFileName</c>.</remarks>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public string GetFilePath(string? currentFileName, string? fileExtension)
        {
            var path = GetPath();
            if (path.EndsWith("/"))
                path = path[..^1];

            if (RealFilename)
            {
                if (string.IsNullOrEmpty(currentFileName))
                    throw new ArgumentNullException(nameof(currentFileName));

                path += $"/{currentFileName}";
            }
            else
            {
                if (string.IsNullOrEmpty(fileExtension))
                    throw new ArgumentNullException(nameof(fileExtension));

                path += $"/{Guid.NewGuid()}.{fileExtension}";
            }

            return path;
        }

        /// <summary>
        /// Returns final path
        /// </summary>
        /// <returns>An <see cref="string"/> value that representing path</returns>
        /// <exception cref="NullReferenceException"></exception>
        public string GetPath()
        {
            if (Folder == null)
                throw new NullReferenceException(nameof(Folder));

            var rootFolder = Folder;
            if (rootFolder.Contains("\\"))
                rootFolder = rootFolder.Replace("\\", "/");

            if (rootFolder.StartsWith("/"))
            {
                var currentAssembly = Assembly.GetEntryAssembly();
                var appPath = Path.GetDirectoryName(currentAssembly!.Location);
                rootFolder = appPath + rootFolder;
            }

            if (rootFolder.EndsWith("/"))
                rootFolder = rootFolder[..^1];

            var toDate = DateTime.UtcNow;
            var targetDir = rootFolder;
            if (!HierarchicallyFolderNameByDate)
                return targetDir;

            var year = toDate.Year.ToString("D4");
            var month = toDate.Month.ToString("D2");
            var day = toDate.Day.ToString("D2");
            targetDir += $"/{year}/{month}/{day}";

            return targetDir;
        }
    }
}