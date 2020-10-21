using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using R8.Lib.Attributes;

namespace R8.Lib.AspNetCore.Base
{
    public abstract class BaseIdentityModel : ValidatableObject
    {
        [HiddenInput]
        [JsonProperty("id")]
        [Order(0)]
        public string Id { get; set; }
    }

    public abstract class BaseIdentityModel<T> : ValidatableObject<T> where T : class
    {
        [HiddenInput]
        [JsonProperty("id")]
        [Order(0)]
        public string Id { get; set; }
    }
}