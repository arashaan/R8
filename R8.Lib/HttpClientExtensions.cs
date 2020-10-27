using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace R8.Lib
{
    public static class HttpClientExtensions
    {
        public static async Task<string> GetAsync(this HttpClientHandler clientHandler, string url)
        {
            var client = new HttpClient(clientHandler);
            var responseMessage = await client.GetAsync(url).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// Returns full information about specific ip address
        /// </summary>
        /// <param name="ipAddress">Ip Address that you want to parse</param>
        /// <returns></returns>
        public static async Task<IpViewModel> GetIpAddressAsync(string ipAddress)
        {
            using var clientHandler = new HttpClientHandler();
            var json = await clientHandler.GetAsync($"http://free.ipwhois.io/json/{ipAddress}").ConfigureAwait(false);
            var jsonObj = JsonConvert.DeserializeObject<IpViewModel>(json);
            clientHandler.Dispose();
            return jsonObj;
        }
    }
}