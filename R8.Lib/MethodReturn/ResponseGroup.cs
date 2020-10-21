using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;

namespace R8.Lib.MethodReturn
{
    public class ResponseGroup : IResponseBase
    {
        [JsonIgnore]
        public List<IResponseDatabase> Results { get; set; }

        public void AddChild<T>(Response<T> model) where T : class
        {
            Results ??= new List<IResponseDatabase>();
            Results.Add(model);
        }

        public bool Success => Results != null && Results.Any() && Results.All(x => x.Success);
        public ValidatableResultCollection Errors => (ValidatableResultCollection)Results?.SelectMany(x => x.Errors).ToList();
    }
}