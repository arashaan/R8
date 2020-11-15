using R8.AspNetCore.Demo.Services.Enums;

using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace R8.AspNetCore.Demo.Services.Routing
{
    internal class HandlerResponse
    {
        public Dictionary<string, object> RouteData { get; set; }
        public bool IsNew { get; set; }
        public Flags Status { get; set; }
        public string Message { get; set; }

        #region Overrides of Object

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return RouteData?.Any() == true
                ? $"?{string.Join("&", RouteData.Select(x => $"{x.Key}={WebUtility.UrlDecode(x.Value.ToString())}").ToList())}"
                : Message;
        }

        #endregion Overrides of Object
    }
}