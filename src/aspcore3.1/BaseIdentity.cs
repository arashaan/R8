﻿using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using R8.Lib;
using R8.Lib.Attributes;

namespace R8.AspNetCore
{
    public abstract class BaseIdentity : ValidatableObject
    {
        [HiddenInput]
        [JsonProperty("id")]
        [Order(0)]
        public string Id { get; set; }
    }

    public abstract class BaseIdentity<T> : ValidatableObject<T> where T : class
    {
        [HiddenInput]
        [JsonProperty("id")]
        [Order(0)]
        public string Id { get; set; }
    }
}