using Microsoft.AspNetCore.Mvc.ModelBinding;

using R8.Lib.AspNetCore.Base;

using System.Threading.Tasks;

namespace R8.Lib.AspNetCore.Routing
{
    public class SeparatedQueryStringValueProviderFactory : IValueProviderFactory
    {
        private readonly string _separator;
        private readonly string _key;

        public SeparatedQueryStringValueProviderFactory() : this(null, SeparatedQueryStringValueProviderSeparator.Separator)
        {
        }

        public SeparatedQueryStringValueProviderFactory(string key, string separator)
        {
            _key = key;
            _separator = separator;
        }

        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            context.ValueProviders.Insert(0, new SeparatedQueryStringValueProvider(_key, context.ActionContext.HttpContext.Request.Query, _separator));
            return Task.CompletedTask;
        }
    }
}