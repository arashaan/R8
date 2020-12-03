using System;

using Newtonsoft.Json;

namespace R8.AspNetCore.WebApi
{
    public abstract class AuthorizeResponseBase<TStatus> : TokenResponseBase<TStatus>
    {
        [JsonIgnore]
        public virtual Version Version { get; set; }

        [JsonIgnore]
        public virtual OS UserOs { get; set; }

        [JsonIgnore]
        public virtual Device Device { get; set; }
    }
}