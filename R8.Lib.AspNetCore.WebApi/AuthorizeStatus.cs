using System;
using Newtonsoft.Json;
using R8.Lib.MethodReturn;

namespace R8.Lib.AspNetCore.WebApi
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