using Newtonsoft.Json;

namespace R8.Lib.Test.FakeObjects
{
    public class FakeJsonTest
    {
        [JsonProperty("nm")]
        public string Name { get; set; }
    }
}