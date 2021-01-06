using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace R8.AspNetCore.ModelBinders
{
    public class StringModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return context.Metadata.ModelType == typeof(string)
                ? new StringModelBinder()
                : null;
        }
    }
}