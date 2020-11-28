namespace R8.GoogleAnalytics.Models
{
    public class MetricRequest
    {
        public MetricRequest(string name, string type) : this(name)
        {
            Type = type;
        }

        public MetricRequest(string name)
        {
            Name = name.StartsWith("ga:") ? name : $"ga:{name}";
        }

        public string Name { get; }
        public string Type { get; private set; }

        public void SetType(string type) => Type = type;

        public override string ToString()
        {
            return Name;
        }
    }
}