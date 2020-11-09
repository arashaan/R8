using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using R8.Lib.Enums;

namespace R8.Lib.AspNetCore.TagBuilders
{
    public class JsonValidatable
    {
        [JsonIgnore]
        public List<Attribute> Attributes { get; set; }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public ValueTypes ValueType { get; set; }
    }
}