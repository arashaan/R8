using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace R8.Lib.AspNetCore.Routing
{
    public class EmptyQueryStringValueProvider : QueryStringValueProvider
    {
        private readonly IQueryCollection _values;

        public EmptyQueryStringValueProvider(IQueryCollection values)
            : base(BindingSource.Query, values, CultureInfo.InvariantCulture)
        {
            _values = values;
        }

        public override ValueProviderResult GetValue(string key)
        {
            var result = base.GetValue(key);
            if (!result.Values.Any())
                return ValueProviderResult.None;

            return result;
        }
    }

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