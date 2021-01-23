using System;
using System.Globalization;
using System.IO;

namespace R8.Test.Shared
{
    public static class Constants
    {
        public const string Assets = "E:\\Work\\Develope\\R8\\test\\R8.Test.Shared\\Assets";
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