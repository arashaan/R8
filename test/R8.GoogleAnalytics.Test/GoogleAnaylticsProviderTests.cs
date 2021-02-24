using R8.AspNetCore.Test;
using R8.GoogleAnalytics.Models;

using System;
using System.Collections.Generic;

using Xunit;

namespace R8.GoogleAnalytics.Test
{
    public class GoogleAnaylticsProviderTests
    {
        [Fact]
        public async System.Threading.Tasks.Task CallProviderAsync()
        {
            // Assets
            var startDate = new DateTime(2018, 01, 01);
            var endDate = DateTime.Now;
            var serviceRequests = new List<GoogleAnalyticsRequest>
            {
                new GoogleAnalyticsRequest
                {
                    MetricsRequest = new MetricRequestCollection("uniquePageviews", "users"),
                    DimensionsRequest = new DimensionRequestCollection("year", "month"),
                    Orders = new OrderCollection
                    {
                        {"year", GoogleAnalyticsOrderType.Descending},
                        {"month", GoogleAnalyticsOrderType.Descending},
                    },
                    PageSize = 20,
                },
                new GoogleAnalyticsRequest
                {
                    MetricsRequest = new MetricRequestCollection("users"),
                    DimensionsRequest = new DimensionRequestCollection("fullReferrer"),
                    PageSize = 20,
                },
            };

            // Act
            var instance = new GoogleAnalyticsProvider(Constants.GoogleJson, "187208166");
            await instance.ExecuteAsync(startDate, endDate, serviceRequests);

            // Arrange
            Assert.NotNull(serviceRequests);
            Assert.NotEmpty(serviceRequests);
            Assert.NotNull(serviceRequests[0].Rows);
            Assert.NotEmpty(serviceRequests[0].Rows);
            Assert.NotNull(serviceRequests[0].Rows[0].Metrics);
            Assert.NotEmpty(serviceRequests[0].Rows[0].Metrics);
            Assert.NotNull(serviceRequests[0].Rows[0].Dimensions);
            Assert.NotEmpty(serviceRequests[0].Rows[0].Dimensions);
            Assert.Equal(serviceRequests[0].DimensionsRequest.Count, serviceRequests[0].Rows[0].Dimensions.Count);
            // Assert.Equal(serviceRequests[0].MetricsRequest.Count, serviceRequests[0].Rows[0].Metrics.Count);
        }
    }
}