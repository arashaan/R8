using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using ICSharpCode.SharpZipLib.Zip;

using SixLabors.ImageSharp;

namespace R8.FileHandlers
{
    public static class Testers
    {
        /// <summary>
        /// Represents a <see cref="bool"/> value that represents if <see cref="Stream"/> instance is a valid MSWord Document.
        /// </summary>
        /// <param name="stream">An instance of <see cref="Stream"/> that contains word document</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static bool IsWordDoc(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (stream.Length == 0)
                return false;

            try
            {
                using var wordDocument = WordprocessingDocument.Open(stream, false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Represents a <see cref="bool"/> value that represents if <see cref="Stream"/> instance is a valid MSExcel SpreadSheet.
        /// </summary>
        /// <param name="stream">An instance of <see cref="Stream"/> that contains excel file</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static bool IsExcel(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (stream.Length == 0)
                return false;

            try
            {
                using var wordDocument = SpreadsheetDocument.Open(stream, false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Represents a <see cref="bool"/> value that represents if <see cref="Stream"/> instance is a valid MSPowerPoint file.
        /// </summary>
        /// <param name="stream">An instance of <see cref="Stream"/> that contains powerpoint file</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static bool IsPowerPoint(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (stream.Length == 0)
                return false;

            try
            {
                using var wordDocument = PresentationDocument.Open(stream, false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Represents a <see cref="bool"/> value that represents if <see cref="Stream"/> instance is a valid <see cref="ZipFile"/>
        /// </summary>
        /// <param name="stream">An instance of <see cref="Stream"/> that contains archive data</param>
        /// <param name="deepCheck">Deep check for each data within <see cref="ZipFile"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="bool"/> that shows if <c>stream</c> is valid instance</returns>
        public static bool IsArchive(this Stream stream, bool deepCheck)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (stream.Length == 0)
                return false;

            stream.Seek(0, SeekOrigin.Begin);
            stream.Position = 0;

            try
            {
                var zipFile = new ZipFile(stream);
                return zipFile.TestArchive(deepCheck);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Represents a <see cref="bool"/> value that represents if <see cref="Stream"/> instance is a valid <see cref="Image"/>
        /// </summary>
        /// <param name="stream">An instance of <see cref="Stream"/> that contains image data</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static async Task<bool> IsImageAsync(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (stream.Length == 0)
                return false;

            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);

            bool isImage;
            try
            {
                using var img = await Image.LoadAsync(stream).ConfigureAwait(false);
                isImage = img.Width > 0;
                img.Dispose();
            }
            catch
            {
                isImage = false;
            }

            return isImage;
        }

        /// <summary>
        /// Represents a <see cref="bool"/> value that represents if <see cref="Stream"/> instance is a valid <see cref="Image"/>
        /// </summary>
        /// <param name="stream">An instance of <see cref="Stream"/> that contains image data</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="bool"/> that shows if <c>stream</c> is valid instance</returns>
        public static bool IsImage(this Stream stream)
        {
            var result = stream.IsImageAsync().GetAwaiter().GetResult();
            return result;
        }

        /// <summary>
        /// Represents a <see cref="bool"/> value that represents if <see cref="Stream"/> instance is a valid PDF
        /// </summary>
        /// <param name="stream">An instance of <see cref="Stream"/> that contains pdf data</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static async Task<bool> IsPdfAsync(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            stream.Seek(0, SeekOrigin.Begin);
            stream.Position = 0;

            const string pdfString = "%PDF-";
            var pdfBytes = Encoding.ASCII.GetBytes(pdfString);
            var len = pdfBytes.Length;
            var buf = new byte[len];
            var remaining = len;
            var pos = 0;

            while (remaining > 0)
            {
                var amtRead = await stream.ReadAsync(buf, pos, remaining).ConfigureAwait(false);
                if (amtRead == 0)
                    return false;

                remaining -= amtRead;
                pos += amtRead;
            }

            return pdfBytes.SequenceEqual(buf);
        }

        /// <summary>
        /// Represents a <see cref="bool"/> value that represents if <see cref="Stream"/> instance is a valid PDF
        /// </summary>
        /// <param name="stream">An instance of <see cref="Stream"/> that contains pdf data</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="bool"/> that shows if <c>stream</c> is valid instance</returns>
        public static bool IsPdf(this Stream stream)
        {
            var result = stream.IsPdfAsync().GetAwaiter().GetResult();
            return result;
        }

        /// <summary>
        /// Represents a <see cref="bool"/> value that represents if <see cref="Stream"/> instance is a valid SVG
        /// </summary>
        /// <param name="stream">An instance of <see cref="Stream"/> that contains pdf data</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="bool"/> that shows if <c>stream</c> is valid instance</returns>
        public static bool IsSvg(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            stream.Seek(0, SeekOrigin.Begin);
            stream.Position = 0;

            try
            {
                var xml = new XmlDocument();
                xml.Load(stream);

                var result = xml.DocumentElement.Name.Equals("svg", StringComparison.InvariantCultureIgnoreCase);
                return result;
            }
            catch
            {
                return false;
            }
        }
    }
}