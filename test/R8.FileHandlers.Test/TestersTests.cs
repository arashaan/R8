using System;
using System.IO;
using Xunit;

namespace R8.FileHandlers.Test
{
    public class TestersTests
    {
        [Fact]
        public void CallIsArchive_StreamNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((Stream)null).IsArchive(false));
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void CallIsImage_StreamEmpty()
        {
            // Act
            using var stream = new MemoryStream();
            var act = stream.IsImage();

            // Arrange
            Assert.False(act);
        }

        [Fact]
        public void CallIsImage_StreamNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((Stream)null).IsImage());
        }

        [Fact]
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

        [Fact]
        public void CallIsPdf_StreamNull()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => ((Stream)null).IsPdf());
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public void CallIsSvg_StreamNull()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => ((Stream)null).IsSvg());
        }

        [Fact]
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

        [Fact]
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