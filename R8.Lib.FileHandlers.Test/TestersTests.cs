using R8.Lib.xUnit.TestOrder;

using System;
using System.IO;

using Xunit;

namespace R8.Lib.FileHandlers.Test
{
    [TestOrder(0)]
    public class TestersTests : TestClassBase
    {
        [Fact, TestOrder(0)]
        public void CallIsArchive_StreamNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((Stream)null).IsArchive(false));
        }

        [Fact, TestOrder(1)]
        public void CallIsArchive()
        {
            // Act
            using var stream = new MemoryStream();
            using var fileStream = new FileStream(Constants.ValidZipFile, FileMode.Open);
            fileStream.CopyTo(stream);
            var act = stream.IsArchive(false);

            // Arrange
            Assert.True(act);
        }

        [Fact, TestOrder(2)]
        public void CallIsArchive_EmptyFile()
        {
            // Act
            using var stream = new MemoryStream();
            using var fileStream = new FileStream(Constants.EmptyZipFile, FileMode.Open);
            fileStream.CopyTo(stream);
            var act = stream.IsArchive(true);

            // Arrange
            Assert.False(act);
        }

        [Fact, TestOrder(3)]
        public void CallIsArchive_InvalidFile()
        {
            // Act
            using var stream = new MemoryStream();
            using var fileStream = new FileStream(Constants.InvalidZipFile, FileMode.Open);
            fileStream.CopyTo(stream);
            var act = stream.IsArchive(true);

            // Arrange
            Assert.False(act);
        }

        [Fact, TestOrder(4)]
        public void CallIsImage_InvalidFormat()
        {
            // Act
            using var stream = new MemoryStream();
            using var fileStream = new FileStream(Constants.InvalidZipFile, FileMode.Open);
            fileStream.CopyTo(stream);
            var act = stream.IsImage();

            // Arrange
            Assert.False(act);
        }

        [Fact, TestOrder(5)]
        public void CallIsImage_StreamEmpty()
        {
            // Act
            using var stream = new MemoryStream();
            var act = stream.IsImage();

            // Arrange
            Assert.False(act);
        }

        [Fact, TestOrder(6)]
        public void CallIsImage_StreamNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((Stream)null).IsImage());
        }

        [Fact, TestOrder(7)]
        public void CallIsImage()
        {
            // Act
            using var stream = new MemoryStream();
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            fileStream.CopyTo(stream);
            var act = stream.IsImage();

            // Arrange
            Assert.True(act);
        }

        [Fact, TestOrder(8)]
        public void CallIsPdf_StreamNull()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => ((Stream)null).IsPdf());
        }

        [Fact, TestOrder(9)]
        public void CallIsPdf_InvalidFormat2()
        {
            // Act
            using var stream = new MemoryStream();
            using var fileStream = new FileStream(Constants.EmptyZipFile, FileMode.Open);
            fileStream.CopyTo(stream);
            var act = stream.IsPdf();

            // Arrange
            Assert.False(act);
        }

        [Fact, TestOrder(10)]
        public void CallIsSvg()
        {
            // Act
            using var stream = new MemoryStream();
            using var fileStream = new FileStream(Constants.ValidSvgFile, FileMode.Open);
            fileStream.CopyTo(stream);
            var act = stream.IsSvg();

            // Arrange
            Assert.True(act);
        }

        [Fact, TestOrder(11)]
        public void CallIsSvg_StreamNull()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => ((Stream)null).IsSvg());
        }

        [Fact, TestOrder(12)]
        public void CallIsSvg_InvalidFormat()
        {
            // Act
            using var stream = new MemoryStream();
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            fileStream.CopyTo(stream);
            var act = stream.IsSvg();

            // Arrange
            Assert.False(act);
        }

        [Fact, TestOrder(13)]
        public void CallIsPdf()
        {
            // Act
            using var stream = new MemoryStream();
            using var fileStream = new FileStream(Constants.ValidPdfFile, FileMode.Open);
            fileStream.CopyTo(stream);
            var act = stream.IsPdf();

            // Arrange
            Assert.True(act);
        }
    }
}