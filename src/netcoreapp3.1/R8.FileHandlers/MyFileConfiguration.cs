using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace R8.FileHandlers
{
    /// <summary>
    /// An <see cref="MyFileConfiguration"/> abstract class that representing configuration for Save Files.
    /// </summary>
    public class MyFileConfiguration : IMyFileConfiguration, IMyFileConfigurationRouting
    {
        public bool TestDevelopment { get; set; }

        public string Folder { get; set; }

        public string RootPath { get; set; }

        public bool? HierarchicallyFolderNameByDate { get; set; }

        public bool? RealFilename { get; set; }

        public bool? OverwriteFile { get; set; }

        public string GetFilePath(string? currentFileName, string? fileExtension)
        {
            var path = GetPath();
            if (path.EndsWith("/"))
                path = path[..^1];

            if (RealFilename != null && RealFilename == true)
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

        public static string GetCurrentDirectory()
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var appPath = Path.GetDirectoryName(currentAssembly!.Location);
            return appPath;
        }

        public string GetPath()
        {
            if (string.IsNullOrEmpty(RootPath))
                throw new NullReferenceException($"{nameof(RootPath)} must be entered.");

            var rootFolder = RootPath;
            if (rootFolder.Contains("\\"))
                rootFolder = rootFolder.Replace("\\", "/");

            var toDate = DateTime.UtcNow;
            var targetDir = string.Empty;
            if (!string.IsNullOrEmpty(Folder))
                targetDir += $"/{(Folder.StartsWith("/") ? Folder[1..] : Folder)}";

            if (HierarchicallyFolderNameByDate != null && HierarchicallyFolderNameByDate == true)
            {
                var year = toDate.Year.ToString("D4");
                var month = toDate.Month.ToString("D2");
                var day = toDate.Day.ToString("D2");
                targetDir += $"/{year}/{month}/{day}";
            }

            var dirArrays = targetDir.Split("/").Where(x => !string.IsNullOrEmpty(x)).ToArray();
            for (var i = 0; i < dirArrays.Length; i++)
            {
                var inHostPath = string.Join("\\", dirArrays.Take(i + 1));
                var thisPath = Path.Combine(RootPath, inHostPath);
                var directory = new DirectoryInfo(thisPath);
                if (!directory.Exists)
                    directory.Create();
            }

            targetDir = targetDir.Replace("\\", "/");
            rootFolder = rootFolder.EndsWith("/") ? rootFolder[..^1] : rootFolder;
            targetDir = targetDir.StartsWith("/") ? targetDir : $"/{targetDir}";
            var finalPath = rootFolder + targetDir;
            return finalPath;
        }
    }
}