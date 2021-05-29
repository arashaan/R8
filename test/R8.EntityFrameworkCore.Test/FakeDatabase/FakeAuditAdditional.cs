using Newtonsoft.Json;

using R8.Lib.JsonConverters;

using System.Globalization;

namespace R8.EntityFrameworkCore.Test.FakeDatabase
{
    public class FakeAuditAdditional
    {
        [JsonConverter(typeof(JsonCultureConverter))]
        public CultureInfo Culture { get; set; }

        public string Text { get; set; }
    }
}