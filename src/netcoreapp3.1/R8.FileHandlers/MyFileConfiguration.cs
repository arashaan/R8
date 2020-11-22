using System;
using System.Collections.Generic;
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

        public string Path { get; set; }

        public bool? HierarchicallyDateFolders { get; set; }

        public bool? SaveAsRealName { get; set; }

        public bool? OverwriteExistingFile { get; set; }

        public Dictionary<string, object> Internal { get; set; }

        public string GetFilePath(string? currentFileName, string? fileExtension)
        {
            var path = GetPath();
            if (path.EndsWith("/"))
                path = path[..^1];

            if (SaveAsRealName != null && SaveAsRealName == true)
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

            if (!IsWebPath())
                path = path.Replace("/", "\\");

            return path;
        }

        private bool IsWebPath() => Internal?.ContainsKey("isWeb") == true;

        public static string GetCurrentDirectory()
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var appPath = System.IO.Path.GetDirectoryName(currentAssembly!.Location);
            return appPath;
        }

        public string GetPath()
        {
            var path = string.Empty;
            if (Internal?.ContainsKey("baseUrl") == true)
                path += Internal["baseUrl"].ToString();

            if (path?.Contains("\\") == true)
                path = path.Replace("\\", "/");

            if (string.IsNullOrEmpty(path) && string.IsNullOrEmpty(Path))
                throw new NullReferenceException("Missing valid path to determine file path.");

            string targetFolder;
            var isWeb = IsWebPath();
            if (!isWeb)
                path = Path;

            if (HierarchicallyDateFolders != null && HierarchicallyDateFolders == true)
            {
                var toDate = DateTime.UtcNow;
                var year = toDate.Year.ToString("D4");
                var month = toDate.Month.ToString("D2");
                var day = toDate.Day.ToString("D2");

                var datePath = $"{year}/{month}/{day}";
                if (isWeb)
                {
                    targetFolder = !string.IsNullOrEmpty(Path)
                        ? $"/{(Path.StartsWith("/") ? Path[1..] : Path)}/{datePath}"
                        : $"/{datePath}";
                }
                else
                {
                    targetFolder = Path.EndsWith("/") ? Path[..^1] : Path + $"/{datePath}";
                }
            }
            else
            {
                if (isWeb)
                {
                    targetFolder = !string.IsNullOrEmpty(Path)
                        ? $"/{(Path.StartsWith("/") ? Path[1..] : Path)}/"
                        : "/";
                }
                else
                {
                    targetFolder = Path;
                }
            }

            if (!isWeb)
                targetFolder = targetFolder.Replace("/", "\\");

            var dirArrays = isWeb
                ? targetFolder.Split("/").Where(x => !string.IsNullOrEmpty(x)).ToArray()
                : targetFolder.Split("\\").Where(x => !string.IsNullOrEmpty(x)).ToArray();
            DirectoryInfo directory = null;
            for (var i = 0; i < dirArrays.Length; i++)
            {
                string tempPath;
                if (isWeb)
                {
                    tempPath = string.Join("/", dirArrays.Take(i + 1));
                    tempPath = $"{path}/{tempPath}";
                }
                else
                {
                    tempPath = string.Join("\\", dirArrays.Take(i + 1));
                }

                directory = new DirectoryInfo(tempPath);
                if (!directory.Exists)
                    directory.Create();
            }

            var final = directory.FullName;
            var final2 = final.Replace("\\", "/");
            return final2;
        }
    }
}