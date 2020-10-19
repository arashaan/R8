using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace R8.Lib
{
    public enum IpType
    {
        IPv4,
        IPv6
    }

    public class IPCountry
    {
        public string Name { get; set; }

        public int? Code { get; set; }

        public string Flag { get; set; }

        public string Capital { get; set; }

        public string CountryPhoneCode { get; set; }

        public List<string> Neighbours { get; set; }

        public string Region { get; set; }

        public string City { get; set; }

        public IPCountryTimeZone TimeZone { get; set; }
        public IPCountryCurrency Currency { get; set; }

        public async Task<Stream> GetFlagAsync()
        {
            if (string.IsNullOrEmpty(Flag))
                return null;

            using var clientHandler = new HttpClientHandler();
            var client = new HttpClient(clientHandler);
            var responseMessage = await client.GetAsync(Flag);
            if (!responseMessage.IsSuccessStatusCode)
                return null;

            var rawBytes = await responseMessage.Content.ReadAsByteArrayAsync();
            await using var ms = new MemoryStream(rawBytes) { Position = 0 };
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }
    }

    public class IPContinent
    {
        public string Name { get; set; }

        public int? Code { get; set; }
        public IPCountry Country { get; set; }
    }

    public class IPCoordinates
    {
        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }

    public class IPIsp
    {
        public string Asn { get; set; }

        public string Origin { get; set; }

        public string Isp { get; set; }
    }

    public class IPCountryTimeZone
    {
        public string TimeZone { get; set; }

        public string Name { get; set; }

        public int? DstOffset { get; set; }

        public int? GmtOffset { get; set; }

        public string Gmt { get; set; }
    }

    public class IPCountryCurrency
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Symbol { get; set; }

        public int? Rates { get; set; }

        public string Plural { get; set; }
    }

    public class IpViewModel
    {
        [JsonConstructor]
        public IpViewModel([JsonProperty("ip")] string ip, [JsonProperty("success")] bool success, [JsonProperty("flags")] string type,
          [JsonProperty("continent")] string continent, [JsonProperty("continent_code")] string continentCode,
          [JsonProperty("country")] string country, [JsonProperty("country_code")] string countryCode, [JsonProperty("country_flag")] string countryFlag,
          [JsonProperty("country_capital")] string countryCapital, [JsonProperty("country_phone")] string countryPhone,
          [JsonProperty("country_neighbours")] string countryNeighbours, [JsonProperty("region")] string region, [JsonProperty("city")] string city,
          [JsonProperty("currency")] string currency, [JsonProperty("currency_code")] string currencyCode, [JsonProperty("currency_symbol")] string currencySymbol,
          [JsonProperty("currency_rates")] int? currencyRates, [JsonProperty("currency_plural")] string currencyPlural, [JsonProperty("timezone")] string timezone,
          [JsonProperty("timezone_name")] string timezoneName, [JsonProperty("timezone_dstoffset")] int? timezoneDstOffset, [JsonProperty("timezone_gmtoffset")] int? timezoneGmtOffset,
          [JsonProperty("timezone_gmt")] string timezoneGmt, [JsonProperty("asn")] string asn, [JsonProperty("org")] string origin, [JsonProperty("isp")] string isp,
          [JsonProperty("latitude")] double? latitude, [JsonProperty("longitude")] double? longitude)
        {
            Ip = ip;
            Success = success;

            var hasValidType = Enum.TryParse<IpType>(type, out var ipType);
            Type = hasValidType ? ipType : (IpType?)null;

            Continent = new IPContinent
            {
                Name = continent,
                Code = int.TryParse(continentCode, out var continentCodeInt) ? continentCodeInt : (int?)null,
                Country = new IPCountry
                {
                    Name = country,
                    Code = int.TryParse(countryCode, out var countryCodeInt) ? countryCodeInt : (int?)null,
                    Flag = countryFlag,
                    Capital = countryCapital,
                    CountryPhoneCode = countryPhone,
                    Neighbours = !string.IsNullOrEmpty(countryNeighbours) ? countryNeighbours.Split(",")?.ToList() : new List<string>(),
                    Region = region,
                    City = city,
                    Currency = new IPCountryCurrency
                    {
                        Name = currency,
                        Code = currencyCode,
                        Symbol = currencySymbol,
                        Rates = currencyRates,
                        Plural = currencyPlural
                    },
                    TimeZone = new IPCountryTimeZone
                    {
                        TimeZone = timezone,
                        Name = timezoneName,
                        DstOffset = timezoneDstOffset,
                        GmtOffset = timezoneGmtOffset,
                        Gmt = timezoneGmt
                    }
                }
            };
            Coordinates = new IPCoordinates
            {
                Latitude = latitude,
                Longitude = longitude
            };
            Isp = new IPIsp
            {
                Asn = asn,
                Origin = origin,
                Isp = isp
            };
        }

        public string Ip { get; set; }

        public bool Success { get; set; }

        public IpType? Type { get; set; }
        public IPContinent Continent { get; set; }
        public IPCoordinates Coordinates { get; set; }
        public IPIsp Isp { get; set; }

        public override string ToString()
        {
            return Continent.Country.Name;
        }
    }
}