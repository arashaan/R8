using Newtonsoft.Json;

using System;
using System.IO;
using System.Threading.Tasks;

namespace R8.Lib
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Deserializes stream content as given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static async Task<T> ReadAsDeserializedObjectAsync<T>(this Stream stream) where T : class
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var json = await stream.ReadAsStringAsync();
            return !string.IsNullOrEmpty(json)
                ? JsonConvert.DeserializeObject<T>(json)
                : null;
        }

        /// <summary>
        /// Returns stream content as string.
        /// </summary>
        /// <param name="stream"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static async Task<string> ReadAsStringAsync(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);

            using var sw = new StreamReader(stream);
            var body = await sw.ReadToEndAsync();
            return body;
        }
    }
}