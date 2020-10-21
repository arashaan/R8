using Newtonsoft.Json;

using R8.Lib.MethodReturn;

using System;

namespace R8.Lib.AspNetCore.Api
{
    public class AuthorizeStatus : Response
    {
        [JsonIgnore]
        public UserResponse User { get; set; }

        [JsonIgnore]
        public Version Version { get; set; }

        [JsonIgnore]
        public OS UserOs { get; set; }

        [JsonIgnore]
        public Device Device { get; set; }
    }
}