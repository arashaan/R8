using System.Collections.Generic;
using System.Linq;

namespace R8.GoogleAnalytics.Models
{
    public class DimensionRequestCollection : List<string>
    {
        public DimensionRequestCollection(params string[] names)
        {
            if (names?.Any() != true)
                return;

            foreach (var name in names)
            {
                var dimension = name.StartsWith("ga:") ? name : $"ga:{name}";
                this.Add(dimension);
            }
        }
    }
}