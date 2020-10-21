using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace R8.Lib.FileHandlers
{
    public class MyFileConfiguration
    {
        public bool RealFilename { get; set; } = false;

        public string UploadFolder { get; set; } = "uploads";

        public bool WatermarkOnImages { get; set; } = false;
        public string GhostScriptDllPath { get; set; }
        public string WatermarkPath { get; set; } = "/img/wm.png";
        public bool GregorianDateFolderName { get; set; } = true;
        public bool UseImageEncoder { get; set; } = true;

        public IImageEncoder ImageEncoder { get; set; } = new JpegEncoder { Quality = 70 };
    }
}