using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace R8.Lib
{
    public static class HttpExtensions
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
        /// Retrieves <see cref="IPAddress"/> from according to current system network adapters.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="IPAddress"/> object.</returns>
        public static IPAddress GetIPAddress()
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            if (!(socket.LocalEndPoint is IPEndPoint endPoint))
                return null;

            var localIp = endPoint.Address.ToString();
            return !string.IsNullOrEmpty(localIp)
                ? IPAddress.Parse(localIp)
                : IPAddress.None;
        }

        /// <summary>
        /// Retrieves <see cref="IPAddress"/> from according to current system network adapters.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="IPAddress"/> object.</returns>
        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;

            return IPAddress.None;
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