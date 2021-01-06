using Humanizer;

using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

using System;
using System.Drawing.Imaging;
using System.IO;

namespace R8.FileHandlers
{
    public static class Extensions
    {
        /// <summary>
        /// Represents Aspect Ratio for desired screen size
        /// </summary>
        /// <param name="width">Screen's width</param>
        /// <param name="height">Screen's height</param>
        /// <returns>A <see cref="AspectRatio"/> instance representing scale like <example>1:1</example></returns>
        public static AspectRatio FindAspectRatio(int width, int height)
        {
            if (width < height)
            {
                var temp = width;
                width = height;
                height = temp;
            }

            while (true)
            {
                var remainder = width % height;
                if (remainder == 0)
                {
                    var ar = height;
                    return new AspectRatio(width / ar, height / ar);
                }

                width = height;
                height = remainder;
            }
        }

        ///// <summary>
        ///// Represents an existing directory, or create that instead
        ///// </summary>
        ///// <param name="serverRootPath">An string for server root path like <c>http://localhosts:8080/</c></param>
        ///// <param name="path">Full path for directory</param>
        ///// <exception cref="ArgumentException"></exception>
        //public static void GetOrCreateDirectory(string serverRootPath, string path)
        //{
        //    if (string.IsNullOrEmpty(path))
        //        throw new ArgumentException(path);

        //    var dirArrays = path.Split("\\");
        //    for (var i = 0; i < dirArrays.Length; i++)
        //    {
        //        var inHostPath = string.Join("\\", dirArrays.Take(i + 1));
        //        var thisPath = Path.Combine(serverRootPath, inHostPath);
        //        var directory = new DirectoryInfo(thisPath);
        //        if (!directory.Exists)
        //            directory.Create();
        //    }
        //}

        /// <summary>
        /// Represents a Unique FileName.
        /// Search for filename and returns an unique filename when there are many duplicate files exists
        /// </summary>
        /// <param name="filePath">An string value contains <c>Absolute Path\FileName.xyz</c></param>
        /// <returns>An string value, maybe same as <c>Absolute Path\FileName.xyz</c>, or maybe like <c>Absolute Path\FileName_2.xyz</c></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetUniqueFileName(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            var existCheckDone = false;
            var index = 2;
            var tempFileName = filePath;
            var folder = Directory.GetParent(filePath).ToString();

            do
            {
                var isExist = File.Exists(tempFileName);
                if (isExist)
                {
                    var fileExt = Path.GetExtension(filePath);
                    var file = Path.GetFileNameWithoutExtension(filePath);

                    tempFileName = $"{folder}\\{file}_{index}{fileExt}";
                    index++;
                }
                else
                {
                    existCheckDone = true;
                }
            } while (!existCheckDone);

            tempFileName = tempFileName.Replace("\\", "/");
            return tempFileName;
        }

        /// <summary>
        /// Represents an truncated filePath
        /// </summary>
        /// <param name="fileName">Original filename</param>
        /// <returns>An string like <c>4d2b37e0-1ecd-4cb0-9e37-0ab9... .jpg</c></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string TruncateFileName(string fileName, int length = 10)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            var name = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);

            var result = $"{name.Truncate(length)}... {ext}";
            return result;
        }

        /// <summary>
        /// Represents a hamunized string of <see cref="Stream"/> Length
        /// </summary>
        /// <param name="stream">An <see cref="Stream"/> instance to get Length</param>
        /// <param name="byte">A default string value to <c>byte</c></param>
        /// <param name="kiloByte">A default string value to <c>Kilobyte</c></param>
        /// <param name="megaByte">A default string value to <c>Megabyte</c></param>
        /// <param name="gigaByte">A default string value to <c>Gigabyte</c></param>
        /// <param name="teraByte">A default string value to <c>Terabyte</c></param>
        /// <param name="petaByte">A default string value to <c>Petabyte</c></param>
        /// <param name="exaByte">A default string value to <c>Exabyte</c></param>
        /// <returns>An string like <c>3 مگابایت</c></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string HumanizeSize(this Stream stream, string @byte = "بایت", string kiloByte = "کیلوبایت",
            string megaByte = "مگابایت", string gigaByte = "گیگابایت", string teraByte = "ترابایت",
            string petaByte = "پتابایت", string exaByte = "اگزابایت")
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            var i = stream.Length;
            return HumanizeSize(i, @byte, kiloByte, megaByte, gigaByte, teraByte, petaByte, exaByte);
        }

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="filePath">An <see cref="string"/> value that representing full file path</param>
        /// <returns>An <see cref="bool"/> value that shows if file is deleted or no</returns>
        public static bool Delete(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                File.Delete(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Represents a hamunized string of specific Length
        /// </summary>
        /// <param name="length">A long value</param>
        /// <param name="byte">A default string value to <c>byte</c></param>
        /// <param name="kiloByte">A default string value to <c>Kilobyte</c></param>
        /// <param name="megaByte">A default string value to <c>Megabyte</c></param>
        /// <param name="gigaByte">A default string value to <c>Gigabyte</c></param>
        /// <param name="teraByte">A default string value to <c>Terabyte</c></param>
        /// <param name="petaByte">A default string value to <c>Petabyte</c></param>
        /// <param name="exaByte">A default string value to <c>Exabyte</c></param>
        /// <returns>An string like <c>3 مگابایت</c></returns>
        public static string HumanizeSize(long length, string @byte = "بایت", string kiloByte = "کیلوبایت",
            string megaByte = "مگابایت", string gigaByte = "گیگابایت", string teraByte = "ترابایت",
            string petaByte = "پتابایت", string exaByte = "اگزابایت")
        {
            var sign = (length < 0 ? "-" : "");
            double readable;
            string suffix;
            if (length >= 0x1000000000000000) // Exabyte
            {
                suffix = exaByte;
                readable = length >> 50;
            }
            else if (length >= 0x4000000000000) // Petabyte
            {
                suffix = petaByte;
                readable = length >> 40;
            }
            else if (length >= 0x10000000000) // Terabyte
            {
                suffix = teraByte;
                readable = length >> 30;
            }
            else if (length >= 0x40000000) // Gigabyte
            {
                suffix = gigaByte;
                readable = length >> 20;
            }
            else if (length >= 0x100000) // Megabyte
            {
                suffix = megaByte;
                readable = length >> 10;
            }
            else if (length >= 0x400) // Kilobyte
            {
                suffix = kiloByte;
                readable = length;
            }
            else
            {
                return length.ToString(sign + $"0 {@byte}"); // Byte
            }

            readable /= 1024;
            return sign + readable.ToString("0.### ") + suffix;
        }

        /// <summary>
        /// Represents an <see cref="string"/> extension by <see cref="ImageFormat"/>
        /// </summary>
        /// <param name="format"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="string"/> value representing image extension</returns>
        public static string GetImageExtension(this ImageFormat format)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            if (format.Equals(ImageFormat.Jpeg))
                return "jpg";

            if (format.Equals(ImageFormat.Bmp))
                return "bmp";

            if (format.Equals(ImageFormat.Gif))
                return "gif";

            if (format.Equals(ImageFormat.Png))
                return "png";

            throw new ArgumentOutOfRangeException(nameof(format));
        }

        /// <summary>
        /// Represents an <see cref="ImageFormat"/> by specific <see cref="IImageEncoder"/>
        /// </summary>
        /// <param name="encoder"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns>An <see cref="ImageFormat"/> instance</returns>
        /// <exception cref="ArgumentNullException"><paramref name="encoder"/> is <c>null</c>.</exception>
        public static ImageFormat GetImageFormat(this IImageEncoder encoder)
        {
            if (encoder == null)
                throw new ArgumentNullException(nameof(encoder));

            var imageFormat = encoder switch
            {
                JpegEncoder _ => ImageFormat.Jpeg,
                PngEncoder _ => ImageFormat.Png,
                BmpEncoder _ => ImageFormat.Bmp,
                GifEncoder _ => ImageFormat.Gif,
                _ => throw new ArgumentOutOfRangeException(
                    $"Unable to recognize {nameof(encoder)} encoder of type {typeof(IImageEncoder)}")
            };
            return imageFormat;
        }
    }
}