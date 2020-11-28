using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

namespace R8.GoogleAnalytics
{
    /// <summary>
    /// Initializes a provider for GoogleAnalyticsV4.
    /// </summary>
    public class GoogleAnalyticsProvider
    {
        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("project_id")]
        public string ProjectId { get; }

        [JsonProperty("private_key_id")]
        public string PrivateKeyId { get; }

        [JsonProperty("private_key")]
        public string PrivateKey { get; }

        [JsonProperty("client_email")]
        public string ClientEmail { get; }

        [JsonProperty("client_id")]
        public string ClientId { get; }

        [JsonProperty("auth_uri")]
        public string AuthUri { get; }

        [JsonProperty("token_uri")]
        public string TokenUri { get; }

        [JsonProperty("auth_provider_x509_cert_url")]
        public string AuthProviderX509CertUrl { get; }

        [JsonProperty("client_x509_cert_url")] public string ClientX509CertUrl { get; }
        public string ViewId { get; }

        /// <summary>
        /// Initializes a provider for GoogleAnalyticsV4.
        /// </summary>
        /// <param name="viewId">A <see cref="string"/> value that representing google analytics view id for specific project.</param>
        /// <param name="type"></param>
        /// <param name="projectId"></param>
        /// <param name="privateKeyId"></param>
        /// <param name="privateKey"></param>
        /// <param name="clientEmail"></param>
        /// <param name="clientId"></param>
        /// <param name="authUri"></param>
        /// <param name="tokenUri"></param>
        /// <param name="authProviderX509CertUrl"></param>
        /// <param name="clientX509CertUrl"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        [JsonConstructor]
        public GoogleAnalyticsProvider(string viewId, [JsonProperty("type")] string type,
            [JsonProperty("project_id")] string projectId, [JsonProperty("private_key_id")] string privateKeyId,
            [JsonProperty("private_key")] string privateKey, [JsonProperty("client_email")] string clientEmail,
            [JsonProperty("client_id")] string clientId, [JsonProperty("auth_uri")] string authUri,
            [JsonProperty("token_uri")] string tokenUri, [JsonProperty("auth_provider_x509_cert_url")]
            string authProviderX509CertUrl, [JsonProperty("client_x509_cert_url")] string clientX509CertUrl)
        {
            ViewId = viewId;
            Type = type;
            ProjectId = projectId;
            PrivateKeyId = privateKeyId;
            PrivateKey = privateKey;
            ClientEmail = clientEmail;
            ClientId = clientId;
            AuthUri = authUri;
            TokenUri = tokenUri;
            AuthProviderX509CertUrl = authProviderX509CertUrl;
            ClientX509CertUrl = clientX509CertUrl;
        }

        /// <summary>
        /// Initializes a provider for GoogleAnalyticsV4.
        /// </summary>
        /// <param name="path">A <see cref="string"/> value that representing json file that contains google analytics credential data.</param>
        /// <param name="viewId">A <see cref="string"/> value that representing google analytics view id for specific project.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public GoogleAnalyticsProvider(string path, string viewId)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (viewId == null)
                throw new ArgumentNullException(nameof(viewId));

            using var fileReader = new FileStream(path, FileMode.Open);
            using var streamReader = new StreamReader(fileReader);
            fileReader.Position = 0;
            fileReader.Seek(0, SeekOrigin.Begin);
            var json = streamReader.ReadToEnd();

            var result = JsonConvert.DeserializeObject<GoogleAnalyticsProvider>(json);
            this.AuthProviderX509CertUrl = result.AuthProviderX509CertUrl;
            this.AuthUri = result.AuthUri;
            this.ClientEmail = result.ClientEmail;
            this.ClientX509CertUrl = result.ClientX509CertUrl;
            this.ClientId = result.ClientId;
            this.PrivateKey = result.PrivateKey;
            this.PrivateKeyId = result.PrivateKeyId;
            this.ProjectId = result.ProjectId;
            this.TokenUri = result.TokenUri;
            this.Type = result.Type;
            this.ViewId = viewId;
        }

        /// <summary>
        /// Initializes a provider for GoogleAnalyticsV4.
        /// </summary>
        /// <param name="rootSection">Representing specific section that contains credential data in appsettings.json</param>
        /// <param name="viewId">A <see cref="string"/> value that representing google analytics view id for specific project.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public GoogleAnalyticsProvider(IConfigurationSection rootSection, string viewId)
        {
            if (rootSection == null)
                throw new ArgumentNullException(nameof(rootSection));

            var children = rootSection.GetChildren();
            var dictionary =
                children.ToDictionary<IConfigurationSection, string, object>(child => child.Key, child => child.Value);

            var json = JsonConvert.SerializeObject(dictionary);
            var result = JsonConvert.DeserializeObject<GoogleAnalyticsProvider>(json);

            this.AuthProviderX509CertUrl = result.AuthProviderX509CertUrl;
            this.AuthUri = result.AuthUri;
            this.ClientEmail = result.ClientEmail;
            this.ClientX509CertUrl = result.ClientX509CertUrl;
            this.ClientId = result.ClientId;
            this.PrivateKey = result.PrivateKey;
            this.PrivateKeyId = result.PrivateKeyId;
            this.ProjectId = result.ProjectId;
            this.TokenUri = result.TokenUri;
            this.Type = result.Type;
            this.ViewId = viewId;
        }

        private IEnumerable<ReportRequest> ResolveServiceRequests(DateTime startDate, DateTime endDate, IEnumerable<GoogleAnalyticsRequest> serviceRequests)
        {
            if (serviceRequests == null || !serviceRequests.Any())
                throw new ArgumentNullException(nameof(serviceRequests));

            var dateRange = new DateRange
            {
                StartDate = startDate.Date.ToString("yyyy-MM-dd"),
                EndDate = endDate.Date.ToString("yyyy-MM-dd")
            };
            var result = from serviceRequest in serviceRequests
                         where serviceRequest.MetricsRequest.Count > 0
                         select new ReportRequest
                         {
                             DateRanges = new List<DateRange> { dateRange },
                             Metrics = serviceRequest.MetricsRequest.Select(x => new Metric() { Expression = x.Name }).ToList(),
                             Dimensions = serviceRequest.DimensionsRequest.Select(x => new Dimension { Name = x }).ToList(),
                             ViewId = ViewId,
                             OrderBys = ResolveOrders(serviceRequest.Orders),
                             IncludeEmptyRows = serviceRequest.IncludeEmptyRows,
                             PageSize = ResolvePageSize(serviceRequest.PageSize),
                             FiltersExpression = ResolveFilters(serviceRequest.Filters)
                         };
            return result;
        }

        private static IList<OrderBy> ResolveOrders(Dictionary<string, GoogleAnalyticsOrderType> dic)
        {
            if (dic == null || !dic.Any())
                return new List<OrderBy>();

            var result = dic?.Select(order => new OrderBy
            {
                FieldName = order.Key.StartsWith("ga:") ? order.Key : $"ga:{order.Key}",
                SortOrder = order.Value.ToString().ToUpper()
            }).ToList();
            return result;
        }

        private static int? ResolvePageSize(int pageSize)
        {
            var result = pageSize <= 0 ? 10 : pageSize;
            return result;
        }

        private static string ResolveFilters(Dictionary<string, string> dic)
        {
            if (dic == null || !dic.Any())
                return null;

            var tempDic = dic.Select(x => $"{(x.Key.StartsWith("ga:") ? x.Key : "ga:" + x.Key)}=={x.Value}").ToList();
            var result = string.Join(";", tempDic);
            return result;
        }

        public Task ExecuteAsync(DateTime startDate, IEnumerable<GoogleAnalyticsRequest> serviceRequests)
        {
            return ExecuteAsync(startDate, DateTime.Now, serviceRequests);
        }

        public async Task ExecuteAsync(DateTime startDate, DateTime endDate, IEnumerable<GoogleAnalyticsRequest> serviceRequests)
        {
            if (serviceRequests == null || !serviceRequests.Any())
                throw new ArgumentNullException(nameof(serviceRequests));
            var googleAnalyticsRequests = serviceRequests.ToList();

            var httpClientInitializer = GoogleCredential
                .FromJson(JsonConvert.SerializeObject(this))
                .CreateScoped(AnalyticsReportingService.Scope.AnalyticsReadonly);
            var initializer = new BaseClientService.Initializer { HttpClientInitializer = httpClientInitializer };
            var service = new AnalyticsReportingService(initializer);

            var list = ResolveServiceRequests(startDate, endDate, googleAnalyticsRequests).ToList();
            var requestBody = new GetReportsRequest { ReportRequests = list };
            var request = service.Reports.BatchGet(requestBody);
            var response = await request.ExecuteAsync();
            var reports = response.Reports.ToList();
            for (var i = 0; i < reports.Count; i++)
            {
                var report = reports[i];
                var serviceRequest = googleAnalyticsRequests.ToList()[i];
                foreach (var metric in serviceRequest.MetricsRequest)
                {
                    var reportMetric =
                        report.ColumnHeader.MetricHeader.MetricHeaderEntries.First(x => x.Name.Equals(metric.Name));
                    metric.SetType(reportMetric.Type);
                }

                if (report.Data?.Rows?.Any() == true)
                    serviceRequest.Rows = report.Data.Rows.ToList();
            }
        }
    }
}