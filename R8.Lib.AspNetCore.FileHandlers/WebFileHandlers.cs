using ICSharpCode.SharpZipLib.Zip;

using Microsoft.AspNetCore.Http;

using R8.Lib.FileHandlers;

using SixLabors.ImageSharp;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace R8.Lib.AspNetCore.FileHandlers
{
    public static class WebFileHandlers
    {
        public static FileTypes GetFileType(this IFormFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            var fileExtension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(fileExtension))
                return FileTypes.Unknown;

            var fields = typeof(FileTypes)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(x => new
                {
                    Field = x,
                    Display = x.GetCustomAttribute<DisplayAttribute>()
                })
                .Where(x => x.Display != null)
                .ToList();
            if (fields.Count == 0)
                return FileTypes.Unknown;

            foreach (var complex in fields)
            {
                var allowedExtensions = complex.Display.Name.Split("|");
                if (allowedExtensions.Length == 0)
                    continue;

                if (fileExtension.StartsWith("."))
                    fileExtension = fileExtension.Split(".")[1];

                var contain = allowedExtensions
                    .Select(x => x.ToLower())
                    .Contains(fileExtension.ToLower());
                if (!contain)
                    continue;

                return Enum.Parse<FileTypes>(complex.Field.Name, true);
            }

            return FileTypes.Unknown;
        }

        public static async Task<MyThumbnailFile> TryUploadAsync(this IFormFile file, FileTypes fileType,
            string existingFilePath, string rootPath)
        {
            if (string.IsNullOrEmpty(rootPath))
                return null;

            if (file == null || file.Length <= 0)
                return null;

            if (!string.IsNullOrEmpty(existingFilePath))
            {
                var fullPath = Path.Combine(rootPath.Replace("\\", "/"), existingFilePath);
                var stream = await Lib.FileHandlers.FileHandlers.OpenFileAsync(fullPath).ConfigureAwait(false);
                if (stream != null)
                {
                    var equal = await file.CompareAsync(stream).ConfigureAwait(false);
                    if (equal)
                        return null;
                }
            }

            var result = await file.UploadAsync(fileType, rootPath, config => config.WatermarkOnImages = false).ConfigureAwait(false);
            return result;
        }

        public static string ReservedFilePathWithoutExtension(this IFormFile file, string rootPath,
            MyFileConfiguration config)
        {
            var toDate = DateTime.Now;
            string year;
            string month;
            string day;

            if (config.GregorianDateFolderName)
            {
                year = toDate.Year.ToString("D4");
                month = toDate.Month.ToString("D2");
                day = toDate.Day.ToString("D2");
            }
            else
            {
                var persianC = new PersianCalendar();
                year = persianC.GetYear(toDate).ToString("D4");
                month = persianC.GetMonth(toDate).ToString("D2");
                day = persianC.GetDayOfMonth(toDate).ToString("D2");
            }

            var targetDir = $"{year}/{month}/{day}";
            Lib.FileHandlers.FileHandlers.GetOrCreateDirectory(rootPath, $"{config.UploadFolder}/{targetDir}");

            var filename = config.RealFilename
                ? Path.GetFileNameWithoutExtension(file.FileName)
                : Guid.NewGuid().ToString();
            var inHostPath = $"{config.UploadFolder}/{targetDir}/{filename}";
            return inHostPath;
        }

        public static async Task<bool> CompareAsync(this IFormFile file, MemoryStream memoryStream)
        {
            if (file.Length == 0)
                return false;

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms).ConfigureAwait(false);

            var areEqual = await ms.CompareAsync(memoryStream).ConfigureAwait(false);
            await ms.DisposeAsync().ConfigureAwait(false);
            await ms.FlushAsync().ConfigureAwait(false);
            return areEqual;
        }

        public static async Task<MyThumbnailFile> SaveImageAsync(this IFormFile file, string rootPath,
            string absolutePath, Action<MyFileConfiguration> config)
        {
            if (file == null)
                return null;

            using var image = await Image.LoadAsync(file.OpenReadStream());
            var result = await image.SaveImageAsync(rootPath, absolutePath, config);
            return result;
        }

        private static async Task<MyThumbnailFile> UploadAsync(this IFormFile file, FileTypes fileType,
            string rootPath, Action<MyFileConfiguration> config = null)
        {
            var thisConfig = new MyFileConfiguration();
            config?.Invoke(thisConfig);

            if (string.IsNullOrEmpty(rootPath))
                return null;

            if (file == null || file.Length == 0)
                return null;

            var fileExtension = Path.GetExtension(file.FileName).Substring(1);
            var absolutePath = file.ReservedFilePathWithoutExtension(rootPath, thisConfig);
            rootPath = rootPath.Replace("\\", "/");

            switch (fileType)
            {
                case FileTypes.Image:
                    {
                        try
                        {
                            var outputImage = await file
                                .SaveImageAsync(rootPath, absolutePath, config)
                                .ConfigureAwait(false);
                            return outputImage;
                        }
                        catch (Exception ex)
                        {
                            return null;
                        }
                    }

                case FileTypes.Document:
                    {
                        var isPdf = await file.OpenReadStream().IsPdfAsync();
                        if (isPdf)
                        {
                            try
                            {
                                var pdfThumbnailStream = await file
                                    .OpenReadStream()
                                    .PdfToImageAsync(thisConfig.ImageEncoder, thisConfig.GhostScriptDllPath)
                                    .ConfigureAwait(false);
                                if (pdfThumbnailStream == null)
                                    return null;

                                var pdfThumbnail = await pdfThumbnailStream
                                    .SaveImageAsync(rootPath, absolutePath, config)
                                    .ConfigureAwait(false);
                                if (pdfThumbnail == null)
                                    return null;

                                var previewFile = await file
                                    .OpenReadStream()
                                    .SaveFileAsync(rootPath, $"{absolutePath}.pdf")
                                    .ConfigureAwait(false);
                                if (previewFile == null)
                                    return null;

                                var outputPdf = new MyThumbnailFile
                                {
                                    FilePath = previewFile,
                                    ThumbnailPath = pdfThumbnail
                                };
                                return outputPdf;
                            }
                            catch (Exception e)
                            {
                                return null;
                            }
                        }
                        else
                        {
                            var outputDocument = await file
                                .SaveFileAsync(fileExtension, rootPath, absolutePath)
                                .ConfigureAwait(false);
                            return outputDocument;
                        }
                    }
                case FileTypes.Svg:
                    {
                        bool validSvgFile;
                        try
                        {
                            validSvgFile = await file.TestSvgAsync()
                                .ConfigureAwait(false);
                        }
                        catch (Exception e)
                        {
                            return null;
                        }

                        if (!validSvgFile)
                            return null;

                        var outputSvgFile = await file
                            .SaveFileAsync(fileExtension, rootPath, absolutePath)
                            .ConfigureAwait(false);
                        return outputSvgFile;
                    }
                case FileTypes.Zip:
                    {
                        bool validZipFile;
                        try
                        {
                            validZipFile = await file.TestArchiveAsync(false)
                                .ConfigureAwait(false);
                        }
                        catch (Exception e)
                        {
                            return null;
                        }

                        if (!validZipFile)
                            return null;

                        var outputZipFile = await file
                            .SaveFileAsync(fileExtension, rootPath, absolutePath)
                            .ConfigureAwait(false);
                        return outputZipFile;
                    }
                case FileTypes.Video:
                case FileTypes.Unknown:
                default:
                    var outputFile = await file
                        .SaveFileAsync(fileExtension, rootPath, absolutePath)
                        .ConfigureAwait(false);
                    return outputFile;
            }
        }

        public static async Task<bool> TestSvgAsync(this IFormFile file)
        {
            var xml = new XmlDocument();
            await using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                ms.Position = 0;
                ms.Seek(0, SeekOrigin.Begin);

                xml.Load(ms);
            }

            var docElement = xml.DocumentElement;
            return docElement?.Name.Equals("svg", StringComparison.InvariantCultureIgnoreCase) == true;
        }

        public static async Task<bool> TestArchiveAsync(this IFormFile file, bool testData)
        {
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;
            ms.Seek(0, SeekOrigin.Begin);

            var zipFile = new ZipFile(ms);
            return zipFile.TestArchive(testData, TestStrategy.FindFirstError, null);
        }

        public static bool TestArchive(this ZipFile zipFile, bool testData)
        {
            return zipFile.TestArchive(testData, TestStrategy.FindFirstError, null);
        }

        public static Task<MyThumbnailFile> UploadAsync(this IFormFile file,
          string rootPath, Action<MyFileConfiguration> config = null)
        {
            var fileType = file.GetFileType();
            return file.UploadAsync(fileType, rootPath, config);
        }

        internal static async Task<MyThumbnailFile> SaveFileAsync(this IFormFile file, string fileExtension, string rootPath, string absolutePath)
        {
            var outputFile = await file.OpenReadStream()
                .SaveFileAsync(rootPath, $"{absolutePath}.{fileExtension}")
                .ConfigureAwait(false);
            return new MyThumbnailFile(outputFile);
        }

        public static Task<List<MyThumbnailFile>> UploadAsync(this IFormFileCollection files,
            FileTypes fileType, string rootPath, Action<MyFileConfiguration> config = null)
        {
            return files.ToList().UploadAsync(fileType, rootPath, config);
        }

        public static async Task<List<MyThumbnailFile>> UploadAsync(this List<IFormFile> files,
            FileTypes fileType, string rootPath, Action<MyFileConfiguration> config = null)
        {
            var thisConfig = new MyFileConfiguration();
            config?.Invoke(thisConfig);

            if (files?.Any() != true)
                return null;

            var validFiles = files.Where(x => x.Length > 0).ToList();
            if (validFiles?.Any() != true)
                return null;

            var list = new List<MyThumbnailFile>();
            foreach (var validFile in validFiles)
            {
                var file = await validFile.UploadAsync(fileType, rootPath, config).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(file))
                    list.Add(file);
            }

            var result = list.Where(x => !string.IsNullOrEmpty(x)).ToList();
            return result;
        }
    }
}