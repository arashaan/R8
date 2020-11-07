using Newtonsoft.Json;

namespace R8.Lib.JsonExtensions
{
    public static class CustomJsonSerializerSettings
    {
        /// <summary>
        /// A custom-defined <see cref="JsonSerializerSettings"/>
        /// </summary>
        public static JsonSerializerSettings Settings => new JsonSerializerSettings
        {
            Error = (serializer, err) => err.ErrorContext.Handled = true,
            DefaultValueHandling = DefaultValueHandling.Populate,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            ContractResolver = new NullToEmptyContractResolver(),
            Formatting = Formatting.None,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            TypeNameHandling = TypeNameHandling.Auto
        };
    }
}