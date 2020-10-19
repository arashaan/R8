using Newtonsoft.Json;

namespace R8.Lib
{
    public static class JsonSettingsExtensions
    {
        public static JsonSerializerSettings JsonNetSettings => new JsonSerializerSettings
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