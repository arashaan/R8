using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace R8.Lib.AspNetCore.Attributes
{
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class NavBarAttribute : Attribute, IAsyncPageFilter
  {
    private readonly Type _pageType;

    public NavBarAttribute(Type pageType)
    {
      _pageType = pageType;
    }

    public NavBarAttribute()
    {
    }

    public const string Key = "NavHelper";

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
      var value = _pageType != null ? _pageType.FullName : context.HandlerInstance.GetType().FullName;
      context.HttpContext.Items[Key] = value;
      return Task.FromResult(0);
    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
      await next.Invoke();
    }
  }
}