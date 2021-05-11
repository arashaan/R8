using Newtonsoft.Json;

using R8.Lib.IPProcess;

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
        /// Returns full information about specific ip address
        /// </summary>
        /// <param name="ipAddress">Ip Address that you want to parse</param>
        /// <returns>A <see cref="Task{TResult}"/> object representing the asynchronous operation.</returns>
        public static async Task<IPAddressFull> GetIPAddressInformationAsync(this IPAddress ipAddress) =>
            await GetIPAddressInformationAsync(ipAddress.ToString()).ConfigureAwait(false);

        /// <summary>
        /// Returns full information about specific ip address
        /// </summary>
        /// <param name="ipAddress">Ip Address that you want to parse</param>
        /// <returns>A <see cref="Task{TResult}"/> object representing the asynchronous operation.</returns>
        public static async Task<IPAddressFull> GetIPAddressInformationAsync(string ipAddress)
        {
            using var clientHandler = new HttpClientHandler();

            HttpContent? httpContent;
            try
            {
                httpContent = await clientHandler.GetAsync($"http://free.ipwhois.io/json/{ipAddress}").ConfigureAwait(false);
            }
            catch
            {
                return null;
            }

            if (httpContent == null)
                return null;

            var json = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
            var jsonObj = JsonConvert.DeserializeObject<IPAddressFull>(json);
            clientHandler.Dispose();
            return jsonObj;
        }

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
        /// Pings a website to determine if internet is connected.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static async Task<EndPoint> IsInternetConnectedAsync(string host = "8.8.8.8", int port = 65530)
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            await socket.ConnectAsync(host, port);
            var localEndPoint = socket.LocalEndPoint;
            socket.Close();
            return localEndPoint;
        }

        /// <summary>
        /// Retrieves <see cref="IPAddress"/> from according to current system network adapters.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="IPAddress"/> object.</returns>
        public static async Task<IPAddress> GetIPAddressAsync()
        {
            var endpoint = await IsInternetConnectedAsync();
            if (!(endpoint is IPEndPoint endPoint))
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