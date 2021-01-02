using R8.Test.Constants;
using R8.Test.Constants.TestOrderers;

using System;
using System.IO;

using Xunit;

namespace R8.FileHandlers.Test
{
    [TestCaseOrderer("Tests.Orderers.PriorityOrderer", "Tests.Orderers")]
    public class TestersTests
    {
        [Fact, TestPriority(0)]
        public void CallIsArchive_StreamNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((Stream)null).IsArchive(false));
        }

        [Fact, TestPriority(1)]
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

        [Fact, TestPriority(2)]
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

        [Fact, TestPriority(3)]
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

        [Fact, TestPriority(4)]
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

        [Fact, TestPriority(5)]
        public void CallIsImage_StreamEmpty()
        {
            // Act
            using var stream = new MemoryStream();
            var act = stream.IsImage();

            // Arrange
            Assert.False(act);
        }

        [Fact, TestPriority(6)]
        public void CallIsImage_StreamNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((Stream)null).IsImage());
        }

        [Fact, TestPriority(7)]
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

        [Fact, TestPriority(8)]
        public void CallIsPdf_StreamNull()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => ((Stream)null).IsPdf());
        }

        [Fact, TestPriority(9)]
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

        [Fact, TestPriority(10)]
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

        [Fact, TestPriority(11)]
        public void CallIsSvg_StreamNull()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => ((Stream)null).IsSvg());
        }

        [Fact, TestPriority(12)]
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

        [Fact, TestPriority(13)]
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