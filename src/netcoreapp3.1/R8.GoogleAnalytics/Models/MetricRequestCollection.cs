using System.Collections.Generic;
using System.Linq;

namespace R8.GoogleAnalytics.Models
{
    public class MetricRequestCollection : List<MetricRequest>
    {
        public MetricRequestCollection(params string[] names)
        {
            if (names?.Any() != true)
                return;

            foreach (var name in names)
            {
                var metric = new MetricRequest(name);
                this.Add(metric);
            }
        }
    }
}