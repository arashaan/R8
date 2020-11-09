using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace R8.Lib
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Request a POST method
        /// </summary>
        /// <param name="clientHandler">An <see cref="HttpClientHandler"/> </param>
        /// <param name="url">Url to send request</param>
        /// <param name="content">Payload content to send</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="string"/> response content</returns>
        public static async Task<HttpContent?> PostAsync(this HttpClientHandler clientHandler, string url, HttpContent content)
        {
            var client = new HttpClient(clientHandler);
            var responseMessage = await client.PostAsync(url, content).ConfigureAwait(false);
            return !responseMessage.IsSuccessStatusCode
                ? null
                : responseMessage.Content;
        }

        /// <summary>
        /// Request a GET method
        /// </summary>
        /// <param name="clientHandler">An <see cref="HttpClientHandler"/> </param>
        /// <param name="url">Url to send request</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="string"/> response content</returns>
        public static async Task<HttpContent?> GetAsync(this HttpClientHandler clientHandler, string url)
        {
            var client = new HttpClient(clientHandler);
            var responseMessage = await client.GetAsync(url).ConfigureAwait(false);
            return !responseMessage.IsSuccessStatusCode
                ? null
                : responseMessage.Content;
        }
    }
}