using Newtonsoft.Json;

using R8.Lib.Enums;

using System.Collections.Generic;
using System.Linq;

namespace R8.Lib.MethodReturn
{
    public class ResponseGroup : IResponseBase
    {
        public ResponseGroup()
        {
        }

        public ResponseGroup(Flags status)
        {
            this.AddChild(new ResponseDatabase(status));
        }

        [JsonIgnore]
        public List<IResponseDatabase> Results { get; set; }

        public void AddChild(IResponseDatabase model)
        {
            Results ??= new List<IResponseDatabase>();
            Results.Add(model);
        }

        public void AddChild<T>(Flags status) where T : class
        {
            AddChild(new Response<T>(status));
        }

        public bool Success => Results != null && Results.Any() && Results.All(x => x.Success);

        public static explicit operator ResponseGroup(Flags status)
        {
            return new ResponseGroup(status);
        }

        public static implicit operator bool(ResponseGroup response)
        {
            return response.Success;
        }

        public ValidatableResultCollection Errors =>
            (ValidatableResultCollection)Results?.SelectMany(x => x.Errors).ToList();
    }
}