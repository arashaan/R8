using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace R8.Lib.IPProcess
{
    public class ISPFull
    {
        public ISPFull()
        {
        }

        public ISPFull(string asn)
        {
            Asn = asn;
        }

        [JsonProperty("asn")]
        public string Asn { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("allocated")]
        public DateTimeOffset? Allocated { get; set; }

        [JsonProperty("registry")]
        public string Registry { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("num_ips")]
        public long NumberOfIPs { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("prefixes")]
        public IspPrefix[] Prefixes { get; set; }

        [JsonProperty("prefixes6")]
        public IspPrefix[] Prefixes6 { get; set; }

        [JsonProperty("peers")]
        public long[] Peers { get; set; }

        [JsonProperty("upstreams")]
        public long[] Upstreams { get; set; }

        [JsonProperty("downstreams")]
        public long[] Downstreams { get; set; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Name)
                ? $"You should use {GetInformationAsync()} method to receive information."
                : Name;
        }

        /// <summary>
        /// Retrieves all information about current ASN
        /// </summary>
        /// <returns>An <see cref="Task"/> object representing asynchronous operation.</returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task GetInformationAsync()
        {
            if (string.IsNullOrEmpty(Asn))
                throw new NullReferenceException($"{Asn} expected to be entered to gathering data.");

            const string url = "https://ipinfo.io/asn-api";
            var dic = new Dictionary<string, string> { { "input", "AS50810" } };
            var payload = new FormUrlEncodedContent(dic);
            using var clientHandler = new HttpClientHandler();
            var httpContent = await clientHandler.PostAsync(url, payload).ConfigureAwait(false);
            if (httpContent == null)
                throw new NullReferenceException($"Unable to receive information for ASN {Asn}.");

            var json = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
            var jsonObj = JsonConvert.DeserializeObject<ISPFull>(json);

            jsonObj.CopyTo(this);
        }
    }

    public class IspPrefix
    {
        [JsonProperty("netblock")]
        public string Netblock { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("size")]
        // [JsonConverter(typeof(ParseStringConverter))]
        public long Size { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}