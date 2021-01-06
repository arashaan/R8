using Microsoft.AspNetCore.Mvc.ModelBinding;

using System;

namespace R8.AspNetCore.ModelBinders
{
    public class DateTimeUtcModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return context.Metadata.ModelType == typeof(DateTime) || context.Metadata.ModelType == typeof(DateTime?)
                ? new DateTimeUtcModelBinder()
                : null;
        }
    }
}