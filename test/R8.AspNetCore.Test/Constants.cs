using System;
using System.IO;

namespace R8.AspNetCore.Test
{
    public static class Constants
    {
        public const string Assets = "E:\\Work\\Develope\\R8\\test\\R8.FileHandlers.Test\\assets";
        public static string GhostScriptFile => Path.Combine(Assets, "gsdll64.dll");
        public static string WatermarkFile => Path.Combine(Assets, "wm.png");
        public static string NewGuidName => Guid.NewGuid().ToString("N");
        public static string ValidZipFile => Path.Combine(Assets, "valid.zip");
        public static string ValidZipFile2 => Path.Combine(Assets, "valid2.zip");

        public static string InvalidZipFile => Path.Combine(Assets, "invalid.zip");
        public static string EmptyZipFile => Path.Combine(Assets, "empty.zip");
        public static string ValidImageFile => Path.Combine(Assets, "valid.png");
        public static string ValidPdfFile => Path.Combine(Assets, "valid.pdf");
        public static string ValidSvgFile => Path.Combine(Assets, "valid.svg");
    }
}