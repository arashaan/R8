using System;
using System.IO;

using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

using Xunit;

namespace R8.FileHandlers.Test
{
    public class FileHandlersTests
    {
        [Fact]
        public void CallSaveImage()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new JpegEncoder(),
                Path = Constants.Assets,
                HierarchicallyDateFolders = false,
                SaveAsRealName = true,
                OverwriteExistingFile = true,
                TestDevelopment = true,
                WatermarkPath = Constants.WatermarkFile
            });

            Assert.NotNull(file);
            Assert.Equal("E:\\Work\\Develope\\R8\\test\\R8.FileHandlers.Test\\assets\\test.jpg", file.FilePath);
        }

        [Fact]
        public void CallSaveImage_WithWatermark_Png()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new PngEncoder(),
                Path = Constants.Assets,
                HierarchicallyDateFolders = false,
                SaveAsRealName = true,
                OverwriteExistingFile = true,
                TestDevelopment = true,
                WatermarkPath = Constants.WatermarkFile
            });

            Assert.NotNull(file);
            Assert.Equal("E:\\Work\\Develope\\R8\\test\\R8.FileHandlers.Test\\assets\\test.png", file.FilePath);
        }

        [Fact]
        public void CallSaveImage_WithWatermark_Bmp()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new BmpEncoder(),
                Path = Constants.Assets,
                HierarchicallyDateFolders = false,
                SaveAsRealName = true,
                OverwriteExistingFile = true,
                TestDevelopment = true,
                WatermarkPath = Constants.WatermarkFile
            });

            Assert.NotNull(file);
            Assert.Equal("E:\\Work\\Develope\\R8\\test\\R8.FileHandlers.Test\\assets\\test.bmp", file.FilePath);
        }

        [Fact]
        public void CallSave_StreamNull()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => ((FileStream)null).Save(null, null));
        }

        [Fact]
        public void CallSave_NameNull()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            Assert.Throws<ArgumentNullException>(() => fileStream.Save(null, null));
        }

        [Fact]
        public void CallSaveImage_WithWatermark_Gif()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new GifEncoder(),
                Path = Constants.Assets,
                HierarchicallyDateFolders = false,
                SaveAsRealName = true,
                OverwriteExistingFile = true,
                TestDevelopment = true,
                WatermarkPath = Constants.WatermarkFile
            });

            Assert.NotNull(file);
            Assert.Equal("E:\\Work\\Develope\\R8\\test\\R8.FileHandlers.Test\\assets\\test.gif", file.FilePath);
        }

        [Fact]
        public void CallSave_WithoutExtension()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            Assert.Throws<NullReferenceException>(() => fileStream.Save("test", null));
        }

        [Fact]
        public void CallSave_Zip()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidZipFile2, FileMode.Open);
            var file = fileStream.Save<MyFileConfiguration>("valid.zip", cfg =>
            {
                cfg.Path = Constants.Assets;
                cfg.HierarchicallyDateFolders = false;
                cfg.SaveAsRealName = true;
                cfg.OverwriteExistingFile = true;
                cfg.TestDevelopment = true;
            });

            Assert.NotNull(file);
            Assert.Equal("E:\\Work\\Develope\\R8\\test\\R8.FileHandlers.Test\\assets\\valid.zip", file.FilePath);
        }

        [Fact]
        public void CallSave_Pdf()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidPdfFile, FileMode.Open);
            var file = fileStream.Save<MyFileConfigurationPdf>("test.pdf", cfg =>
            {
                cfg.GhostScriptDllPath = Constants.GhostScriptFile;
                cfg.Path = Constants.Assets;
                cfg.HierarchicallyDateFolders = false;
                cfg.SaveAsRealName = true;
                cfg.OverwriteExistingFile = true;
                cfg.TestDevelopment = false;
            });

            Assert.NotNull(file);
            Assert.Equal("E:\\Work\\Develope\\R8\\test\\R8.FileHandlers.Test\\assets\\test.pdf", file.FilePath);
            Assert.Equal("E:\\Work\\Develope\\R8\\test\\R8.FileHandlers.Test\\assets\\test_thumbnail.jpg", file.ThumbnailPath);
        }

        [Fact]
        public void CallSave_Image()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            var file = fileStream.Save<MyFileConfigurationImage>("test.jpg", cfg =>
            {
                cfg.ImageEncoder = new JpegEncoder();
                cfg.Path = Constants.Assets;
                cfg.HierarchicallyDateFolders = false;
                cfg.SaveAsRealName = true;
                cfg.OverwriteExistingFile = true;
                cfg.TestDevelopment = true;
                cfg.WatermarkPath = Constants.WatermarkFile;
            });

            Assert.NotNull(file);
            Assert.Equal("E:\\Work\\Develope\\R8\\test\\R8.FileHandlers.Test\\assets\\test.jpg", file.FilePath);
        }

        [Fact]
        public void CallSave_ImageHierarchically()
        {
            // Assets
            var toDate = DateTime.UtcNow;
            var year = toDate.Year.ToString("D4");
            var month = toDate.Month.ToString("D2");
            var day = toDate.Day.ToString("D2");

            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            var file = fileStream.Save<MyFileConfigurationImage>("test.jpg", cfg =>
            {
                cfg.ImageEncoder = new JpegEncoder();
                cfg.Path = Constants.Assets;
                cfg.HierarchicallyDateFolders = true;
                cfg.SaveAsRealName = true;
                cfg.OverwriteExistingFile = true;
                cfg.TestDevelopment = true;
            });

            Assert.NotNull(file);
            Assert.Equal($"E:\\Work\\Develope\\R8\\test\\R8.FileHandlers.Test\\assets\\{year}\\{month}\\{day}\\test.jpg", file.FilePath);
        }

        [Fact]
        public void CallSaveImage_WithWatermark_Jpg()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new JpegEncoder(),
                Path = Constants.Assets,
                HierarchicallyDateFolders = false,
                SaveAsRealName = true,
                OverwriteExistingFile = true,
                TestDevelopment = true,
                WatermarkPath = Constants.WatermarkFile
            });

            Assert.NotNull(file);
            Assert.Equal("E:\\Work\\Develope\\R8\\test\\R8.FileHandlers.Test\\assets\\test.jpg", file.FilePath);
        }

        [Fact]
        public void CallSaveImage_NullStream()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => ((Stream)null).SaveImage(null, null));
        }

        [Fact]
        public void CallSaveImage_NullName()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            Assert.Throws<ArgumentNullException>(() => fileStream.SaveImage(null, null));
        }

        [Fact]
        public void CallSaveImage_Resize()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new JpegEncoder(),
                Path = Constants.Assets,
                HierarchicallyDateFolders = false,
                SaveAsRealName = true,
                OverwriteExistingFile = true,
                TestDevelopment = true,
                WatermarkPath = Constants.WatermarkFile,
                ResizeToSize = 300
            });

            Assert.NotNull(file);
            Assert.Equal("E:\\Work\\Develope\\R8\\test\\R8.FileHandlers.Test\\assets\\test.jpg", file.FilePath);
        }
    }
}