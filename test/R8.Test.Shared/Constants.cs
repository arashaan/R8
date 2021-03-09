using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace R8.AspNetCore.Test
{
    public static class Constants
    {
        public static string GetProjectRootFolder()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                .Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }

        public static string GetSolutionRootFolder()
        {
            return GetProjectRootFolder().Split("\\test\\")[0];
        }

        public static string Assets => GetSolutionRootFolder() + "\\test\\R8.Test.Shared\\Assets";
        public static string FolderPath => Path.Combine(Assets, "Dictionary");
        public static string GhostScriptFile => Path.Combine(Assets, "gsdll64.dll");
        public static string WatermarkFile => Path.Combine(Assets, "wm.png");
        public static string NewGuidName => Guid.NewGuid().ToString("N");
        public static string ValidZipFile => Path.Combine(Assets, "valid.zip");
        public static string ValidZipFile2 => Path.Combine(Assets, "valid2.zip");

        public static string InvalidZipFile => Path.Combine(Assets, "invalid.zip");
        public static string EmptyZipFile => Path.Combine(Assets, "empty.zip");
        public static string ValidWordFile => Path.Combine(Assets, "valid.docx");
        public static string ValidExcelFile => Path.Combine(Assets, "valid.xlsx");
        public static string ValidImageFile => Path.Combine(Assets, "valid.png");
        public static string ValidPdfFile => Path.Combine(Assets, "valid.pdf");
        public static string ValidSvgFile => Path.Combine(Assets, "valid.svg");
        public static string GoogleJson => Path.Combine(Assets, "google.json");

        public static CultureInfo DefaultCulture => CultureInfo.GetCultureInfo("tr");
        public static string JsonFileName => "dic";
    }
}