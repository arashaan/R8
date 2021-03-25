using R8.AspNetCore.Test;
using R8.AspNetCore.Test.TestOrderers;

using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

using System;
using System.IO;

using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCollectionOrderer("R8.Test.Shared.TestOrderers.DisplayNameOrderer", "R8.Test.Shared")]

namespace R8.FileHandlers.Test
{
    [TestCaseOrderer("R8.Test.Shared.TestOrderers.PriorityOrderer", "R8.Test.Shared")]
    public class FileHandlersTests
    {
        [Fact, TestPriority(0)]
        public void CallSaveImage()
        {
            // Act
            using var fileStream = new FileStream(Constants.GetValidImageFile(), FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new JpegEncoder(),
                Path = Constants.GetAssetsFolder(),
                HierarchicallyDateFolders = false,
                SaveAsRealName = true,
                OverwriteExistingFile = true,
                TestDevelopment = true,
                WatermarkPath = Constants.GetWatermarkFile()
            });

            Assert.NotNull(file);
            Assert.Equal(Constants.GetAssetsFolder() + "\\test.jpg", file.FilePath);
        }

        [Fact, TestPriority(1)]
        public void CallSaveImage_WithWatermark_Png()
        {
            // Act
            using var fileStream = new FileStream(Constants.GetValidImageFile(), FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new PngEncoder(),
                Path = Constants.GetAssetsFolder(),
                HierarchicallyDateFolders = false,
                SaveAsRealName = true,
                OverwriteExistingFile = true,
                TestDevelopment = true,
                WatermarkPath = Constants.GetWatermarkFile()
            });

            Assert.NotNull(file);
            Assert.Equal(Constants.GetAssetsFolder() + "\\test.png", file.FilePath);
        }

        [Fact, TestPriority(2)]
        public void CallSaveImage_WithWatermark_Bmp()
        {
            // Act
            using var fileStream = new FileStream(Constants.GetValidImageFile(), FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new BmpEncoder(),
                Path = Constants.GetAssetsFolder(),
                HierarchicallyDateFolders = false,
                SaveAsRealName = true,
                OverwriteExistingFile = true,
                TestDevelopment = true,
                WatermarkPath = Constants.GetWatermarkFile()
            });

            Assert.NotNull(file);
            Assert.Equal(Constants.GetAssetsFolder() + "\\test.bmp", file.FilePath);
        }

        [Fact, TestPriority(3)]
        public void CallSave_StreamNull()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => ((FileStream)null).Save(null, null));
        }

        [Fact, TestPriority(4)]
        public void CallSave_NameNull()
        {
            // Act
            using var fileStream = new FileStream(Constants.GetValidImageFile(), FileMode.Open);
            Assert.Throws<ArgumentNullException>(() => fileStream.Save(null, null));
        }

        [Fact, TestPriority(5)]
        public void CallSaveImage_WithWatermark_Gif()
        {
            // Act
            using var fileStream = new FileStream(Constants.GetValidImageFile(), FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new GifEncoder(),
                Path = Constants.GetAssetsFolder(),
                HierarchicallyDateFolders = false,
                SaveAsRealName = true,
                OverwriteExistingFile = true,
                TestDevelopment = true,
                WatermarkPath = Constants.GetWatermarkFile()
            });

            Assert.NotNull(file);
            Assert.Equal(Constants.GetAssetsFolder() + "\\test.gif", file.FilePath);
        }

        [Fact, TestPriority(6)]
        public void CallSave_WithoutExtension()
        {
            // Act
            using var fileStream = new FileStream(Constants.GetValidImageFile(), FileMode.Open);
            Assert.Throws<NullReferenceException>(() => fileStream.Save("test", null));
        }

        [Fact, TestPriority(7)]
        public void CallSave_Zip()
        {
            // Act
            using var fileStream = new FileStream(Constants.GetValidZipFile2(), FileMode.Open);
            var file = fileStream.Save<MyFileConfiguration>("valid.zip", cfg =>
            {
                cfg.Path = Constants.GetAssetsFolder();
                cfg.HierarchicallyDateFolders = false;
                cfg.SaveAsRealName = true;
                cfg.OverwriteExistingFile = true;
                cfg.TestDevelopment = true;
            });

            Assert.NotNull(file);
            Assert.Equal(Constants.GetAssetsFolder() + "\\valid.zip", file.FilePath);
        }

        [Fact, TestPriority(8)]
        public void CallSave_Pdf()
        {
            // Act
            using var fileStream = new FileStream(Constants.GetValidPdfFile(), FileMode.Open);
            var file = fileStream.Save<MyFileConfigurationPdf>("test.pdf", cfg =>
            {
                cfg.GhostScriptDllPath = Constants.GetGhostScriptFile();
                cfg.Path = Constants.GetAssetsFolder();
                cfg.HierarchicallyDateFolders = false;
                cfg.SaveAsRealName = true;
                cfg.OverwriteExistingFile = true;
                cfg.TestDevelopment = false;
            });

            Assert.NotNull(file);
            Assert.Equal(Constants.GetAssetsFolder() + "\\test.pdf", file.FilePath);
            Assert.Equal(Constants.GetAssetsFolder() + "\\test_thumbnail.jpg", file.ThumbnailPath);
        }

        [Fact, TestPriority(9)]
        public void CallSave_Image()
        {
            // Act
            using var fileStream = new FileStream(Constants.GetValidImageFile(), FileMode.Open);
            var file = fileStream.Save<MyFileConfigurationImage>("test.jpg", cfg =>
            {
                cfg.ImageEncoder = new JpegEncoder();
                cfg.Path = Constants.GetAssetsFolder();
                cfg.HierarchicallyDateFolders = false;
                cfg.SaveAsRealName = true;
                cfg.OverwriteExistingFile = true;
                cfg.TestDevelopment = true;
                cfg.WatermarkPath = Constants.GetWatermarkFile();
            });

            Assert.NotNull(file);
            Assert.Equal(Constants.GetAssetsFolder() + "\\test.jpg", file.FilePath);
        }

        [Fact, TestPriority(10)]
        public void CallSave_ImageHierarchically()
        {
            // Assets
            var toDate = DateTime.UtcNow;
            var year = toDate.Year.ToString("D4");
            var month = toDate.Month.ToString("D2");
            var day = toDate.Day.ToString("D2");

            // Act
            using var fileStream = new FileStream(Constants.GetValidImageFile(), FileMode.Open);
            var file = fileStream.Save<MyFileConfigurationImage>("test.jpg", cfg =>
            {
                cfg.ImageEncoder = new JpegEncoder();
                cfg.Path = Constants.GetAssetsFolder();
                cfg.HierarchicallyDateFolders = true;
                cfg.SaveAsRealName = true;
                cfg.OverwriteExistingFile = true;
                cfg.TestDevelopment = true;
            });

            Assert.NotNull(file);
            Assert.Equal(Constants.GetAssetsFolder() + $"\\{year}\\{month}\\{day}\\test.jpg", file.FilePath);
        }

        [Fact, TestPriority(11)]
        public void CallSaveImage_WithWatermark_Jpg()
        {
            // Act
            using var fileStream = new FileStream(Constants.GetValidImageFile(), FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new JpegEncoder(),
                Path = Constants.GetAssetsFolder(),
                HierarchicallyDateFolders = false,
                SaveAsRealName = true,
                OverwriteExistingFile = true,
                TestDevelopment = true,
                WatermarkPath = Constants.GetWatermarkFile()
            });

            Assert.NotNull(file);
            Assert.Equal(Constants.GetAssetsFolder() + "\\test.jpg", file.FilePath);
        }

        [Fact, TestPriority(12)]
        public void CallSaveImage_NullStream()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => ((Stream)null).SaveImage(null, null));
        }

        [Fact, TestPriority(13)]
        public void CallSaveImage_NullName()
        {
            // Act
            using var fileStream = new FileStream(Constants.GetValidImageFile(), FileMode.Open);
            Assert.Throws<ArgumentNullException>(() => fileStream.SaveImage(null, null));
        }

        [Fact, TestPriority(14)]
        public void CallSaveImage_Resize()
        {
            // Act
            using var fileStream = new FileStream(Constants.GetValidImageFile(), FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new JpegEncoder(),
                Path = Constants.GetAssetsFolder(),
                HierarchicallyDateFolders = false,
                SaveAsRealName = true,
                OverwriteExistingFile = true,
                TestDevelopment = true,
                WatermarkPath = Constants.GetWatermarkFile(),
                ResizeToSize = 300
            });

            Assert.NotNull(file);
            Assert.Equal(Constants.GetAssetsFolder() + "\\test.jpg", file.FilePath);
        }
    }
}