using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace R8.Lib.AspNetCore.ModelBinders
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

  public class StringModelBinder : IModelBinder
  {
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
      if (bindingContext == null)
        throw new ArgumentNullException(nameof(bindingContext));

      if (bindingContext.ModelType != typeof(string))
        return Task.FromResult(ModelBindingResult.Failed());

      var key = bindingContext.ModelName;
      if (string.IsNullOrEmpty(key))
        return Task.CompletedTask;

      var getValue = bindingContext.ValueProvider.GetValue(key);

      var value = getValue.FirstValue;
      if (string.IsNullOrEmpty(value))
        return Task.CompletedTask;

      value = value.FixUnicodeNumbers().Trim();
      bindingContext.Result = ModelBindingResult.Success(value);
      return Task.CompletedTask;
    }
  }
}