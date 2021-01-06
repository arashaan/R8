using Newtonsoft.Json;

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace R8.Lib.IPProcess
{
    public static class Extensions
    {
        /// <summary>
        /// Returns full information about specific ip address
        /// </summary>
        /// <param name="ipAddress">Ip Address that you want to parse</param>
        /// <returns>A <see cref="Task{TResult}"/> object representing the asynchronous operation.</returns>
        public static async Task<IPAddressFull> GetIpAddressAsync(this IPAddress ipAddress) =>
            await GetIpAddressAsync(ipAddress.ToString()).ConfigureAwait(false);

        /// <summary>
        /// Returns full information about specific ip address
        /// </summary>
        /// <param name="ipAddress">Ip Address that you want to parse</param>
        /// <returns>A <see cref="Task{TResult}"/> object representing the asynchronous operation.</returns>
        public static async Task<IPAddressFull> GetIpAddressAsync(string ipAddress)
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
    }
}