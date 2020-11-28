using System.Collections.Generic;

using Google.Apis.AnalyticsReporting.v4.Data;

using R8.GoogleAnalytics.Models;

namespace R8.GoogleAnalytics
{
    /// <summary>
    /// For better solutions you need to visit this page : https://ga-dev-tools.appspot.com/query-explorer/
    /// </summary>
    public class GoogleAnalyticsRequest
    {
        /// <summary>Dimension or metric filters that restrict the data returned for your request. To use the
        /// `filtersExpression`, supply a dimension or metric on which to filter, followed by the filter expression. For
        /// example, the following expression selects `ga:browser` dimension which starts with Firefox;
        /// `ga:browser=~^Firefox`. For more information on dimensions and metric filters, see [Filters
        /// reference](https://developers.google.com/analytics/devguides/reporting/core/v3/reference#filters).</summary>
        public Dictionary<string, string> Filters { get; set; }

        /// <summary>Sort order on output rows. To compare two rows, the elements of the following are applied in order
        /// until a difference is found.  All date ranges in the output get the same row order.</summary>
        public OrderCollection Orders { get; set; }

        public string Alias { get; set; }
        public bool IncludeEmptyRows { get; set; }
        public int PageSize { get; set; }

        /// <summary>
        /// The metrics requested. Requests must specify at least one metric. Requests can have a total of 10 metrics.
        /// </summary>
        public MetricRequestCollection MetricsRequest { get; set; }

        /// <summary>
        /// The dimensions requested. Requests can have a total of 7 dimensions.
        /// </summary>
        public DimensionRequestCollection DimensionsRequest { get; set; }

        public List<ReportRow> Rows { get; internal set; }

        public override string ToString()
        {
            return $"{string.Join(", ", MetricsRequest)} [ {string.Join(", ", DimensionsRequest)} ]";
        }
    }
}