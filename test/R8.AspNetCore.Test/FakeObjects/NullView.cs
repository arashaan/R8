using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

using System;
using System.IO;
using System.Threading.Tasks;

namespace R8.AspNetCore.Test.FakeObjects
{
    public class NullView : IView
    {
        public static readonly NullView Instance = new NullView();
        public string Path => string.Empty;

        public Task RenderAsync(ViewContext context)
        {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            return Task.CompletedTask;
        }
    }
}