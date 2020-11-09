using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace R8.Lib.AspNetCore.Routing
{
    public class EmptyQueryStringValueProviderFactory : IValueProviderFactory
    {
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            var queries = context.ActionContext.HttpContext.Request.Query;
            context.ValueProviders.Insert(0, new EmptyQueryStringValueProvider(queries));
            return Task.CompletedTask;
        }
    }
}