using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace R8.AspNetCore.Test
{
    public static class Constants
    {
        public static string GetProjectFolder()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var exePath = Path.GetDirectoryName(currentAssembly.Location);
            var projectRegex = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+(bin|.vs))");
            var projectFolder = projectRegex.Match(exePath).Value;
            return projectFolder;
        }

        public static string GetSolutionFolder(string fallback = null)
        {
            var projectFolder = GetProjectFolder();
            var array = projectFolder.Split("\\test\\");
            var solutionFolder = array[0];
            var driveRegex = new Regex(@"[A-Z]:\\");
            if (!driveRegex.Match(solutionFolder).Value.Equals(projectFolder, StringComparison.InvariantCultureIgnoreCase))
                return solutionFolder;

            // use fallback ( for live unit testing )
            if (string.IsNullOrEmpty(fallback))
                throw new ArgumentNullException(fallback);

            return fallback;
        }

        public static string GetAssetsFolder()
        {
            var solutionFolder = GetSolutionFolder("C:\\Users\\VorTex\\Downloads\\R8");

            var assetsFolder = solutionFolder + "\\test\\R8.Test.Shared\\Assets";
            return assetsFolder;
        }

        public static string GetLocalizerDictionaryPath()
        {
            var assetsFolder = GetAssetsFolder();
            var dictionaryPath = Path.Combine(assetsFolder, "Dictionary");
            return dictionaryPath;
        }

        public static string GetGhostScriptFile() => Path.Combine(GetAssetsFolder(), "gsdll64.dll");
        public static string GetWatermarkFile() => Path.Combine(GetAssetsFolder(), "wm.png");
        public static string GetValidZipFile() => Path.Combine(GetAssetsFolder(), "valid.zip");
        public static string GetValidZipFile2() => Path.Combine(GetAssetsFolder(), "valid2.zip");

        public static string GetInvalidZipFile() => Path.Combine(GetAssetsFolder(), "invalid.zip");
        public static string GetEmptyZipFile() => Path.Combine(GetAssetsFolder(), "empty.zip");
        public static string GetValidWordFile() => Path.Combine(GetAssetsFolder(), "valid.docx");
        public static string GetValidExcelFile() => Path.Combine(GetAssetsFolder(), "valid.xlsx");
        public static string GetValidImageFile() => Path.Combine(GetAssetsFolder(), "valid.png");
        public static string GetValidPdfFile() => Path.Combine(GetAssetsFolder(), "valid.pdf");
        public static string GetValidSvgFile() => Path.Combine(GetAssetsFolder(), "valid.svg");
        public static string GetGoogleJson() => Path.Combine(GetAssetsFolder(), "google.json");

        public static CultureInfo DefaultCulture => CultureInfo.GetCultureInfo("tr");
        public static string JsonFileName => "dic";
    }
}