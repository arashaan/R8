namespace R8.AspNetCore3_1.Demo.Services.Routing
{
    // internal class HandlerResponse
    // {
    //     public Dictionary<string, object> RouteData { get; set; }
    //     public bool IsNew { get; set; }
    //     public Flags Status { get; set; }
    //     public string Message { get; set; }
    //
    //     #region Overrides of Object
    //
    //     /// <summary>Returns a string that represents the current object.</summary>
    //     /// <returns>A string that represents the current object.</returns>
    //     public override string ToString()
    //     {
    //         return RouteData?.Any() == true
    //             ? $"?{string.Join("&", RouteData.Select(x => $"{x.Key}={WebUtility.UrlDecode(x.Value.ToString())}").ToList())}"
    //             : Message;
    //     }
    //
    //     #endregion Overrides of Object
    // }
}