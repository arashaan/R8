using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace R8.Lib.IPProcess
{
    /// <summary>
    /// Provides in Internet Protocol (IP) address with additional data.
    /// </summary>
    public class IPAddressFull
    {
        [JsonConstructor]
        public IPAddressFull([JsonProperty("ip")] string ip, [JsonProperty("success")] bool success, [JsonProperty("flags")] string type,
          [JsonProperty("continent")] string continent, [JsonProperty("continent_code")] string continentCode,
          [JsonProperty("country")] string country, [JsonProperty("country_code")] string countryCode, [JsonProperty("country_flag")] string countryFlag,
          [JsonProperty("country_capital")] string countryCapital, [JsonProperty("country_phone")] string countryPhone,
          [JsonProperty("country_neighbours")] string countryNeighbours, [JsonProperty("region")] string region, [JsonProperty("city")] string city,
          [JsonProperty("currency")] string currency, [JsonProperty("currency_code")] string currencyCode, [JsonProperty("currency_symbol")] string currencySymbol,
          [JsonProperty("currency_rates")] double? currencyRates, [JsonProperty("currency_plural")] string currencyPlural, [JsonProperty("timezone")] string timezone,
          [JsonProperty("timezone_name")] string timezoneName, [JsonProperty("timezone_dstoffset")] int? timezoneDstOffset, [JsonProperty("timezone_gmtoffset")] int? timezoneGmtOffset,
          [JsonProperty("timezone_gmt")] string timezoneGmt, [JsonProperty("asn")] string asn, [JsonProperty("org")] string origin, [JsonProperty("isp")] string isp,
          [JsonProperty("latitude")] double? latitude, [JsonProperty("longitude")] double? longitude)
        {
            IPAddress = IPAddress.Parse(ip);

            var ipCurrency = new IPCountryCurrency(currency, currencyCode, currencySymbol, currencyRates, currencyPlural);

            ISPAsn = asn;
            ISPName = isp;
            Country = country;
            Code = int.TryParse(countryCode, out var countryCodeInt) ? countryCodeInt : (int?)null;
            Flag = countryFlag;
            Capital = countryCapital;
            CountryPhoneCode = countryPhone;
            Neighbours = !string.IsNullOrEmpty(countryNeighbours) ? countryNeighbours.Split(",")?.ToList() : new List<string>();
            Region = region;
            City = city;
            TimeZone = Dates.GetNodaTimeZone(timezone, false);
            Currency = ipCurrency;

            if (latitude != null && longitude != null)
                Coordinates = new[] { latitude.Value, longitude.Value };
            Continent = continent;
        }

        /// <summary>
        /// Represents an <see cref="System.Net.IPAddress"/> object
        /// </summary>
        [JsonIgnore]
        public IPAddress IPAddress { get; }

        public string ISPAsn { get; }
        public string ISPName { get; }

        public string Flag { get; }
        public string Continent { get; }
        public string Capital { get; }
        public string Country { get; }

        /// <summary>
        /// Gets coordinates of server by the following order <c>latitude, longitude</c>
        /// </summary>
        public double[] Coordinates { get; }

        public int? Code { get; }
        public string CountryPhoneCode { get; }

        public List<string> Neighbours { get; }

        public string Region { get; }

        public string City { get; }

        [JsonIgnore]
        public DateTimeZoneMore TimeZone { get; }

        public IPCountryCurrency Currency { get; }

        /// <summary>
        /// Retrieves Flag icon as <see cref="Stream"/>.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>An <see cref="Task"/> object representing asynchronous operation.</returns>
        public async Task<Stream> GetFlagAsync()
        {
            if (string.IsNullOrEmpty(Flag))
                throw new NullReferenceException($"{Flag} expected to filled with flag icon url.");

            using var clientHandler = new HttpClientHandler();
            using var client = new HttpClient(clientHandler);
            var responseMessage = await client.GetAsync(Flag).ConfigureAwait(false);
            if (!responseMessage.IsSuccessStatusCode)
                return null;

            return await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
        }

        public override string ToString()
        {
            return IPAddress.ToString();
        }
    }
}