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
        /// <exception cref="HttpRequestException"></exception>
        /// <returns>A <see cref="Task{TResult}"/> object representing the asynchronous operation.</returns>
        public static async Task<IPAddressFull> GetIPAddressInformationAsync(string ipAddress)
        {
            var url = $"http://free.ipwhois.io/json/{ipAddress}";
            using var clientHandler = new HttpClientHandler();
            using var client = new HttpClient(clientHandler);
            var responseMessage = await client.GetAsync(url).ConfigureAwait(false);
            if (!responseMessage.IsSuccessStatusCode)
                return null;

            var json = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var jsonObj = JsonConvert.DeserializeObject<IPAddressFull>(json);
            return jsonObj;
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
        /// <returns>A <see cref="IPAddress"/> object.</returns>
        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;

            return IPAddress.None;
        }
    }
}