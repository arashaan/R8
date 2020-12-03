using System;

using Newtonsoft.Json;

using R8.Lib.MethodReturn;

namespace R8.AspNetCore.WebApi
{
    public abstract class AuthorizeResponseBase<TStatus> : ResponseBase<TStatus>, IWebApiCredential
    {
        [JsonIgnore]
        public UserResponse User { get; set; }

        [JsonIgnore]
        public virtual Version Version { get; set; }

        [JsonIgnore]
        public virtual OS UserOs { get; set; }

        [JsonIgnore]
        public virtual Device Device { get; set; }
    }
}