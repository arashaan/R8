using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;

namespace R8.AspNetCore.Test
{
    public class ApplicationWrapper<TContext> : IHttpApplication<TContext>
    {
        private readonly IHttpApplication<TContext> _application;
        private readonly Action _preProcessRequestAsync;

        public ApplicationWrapper(IHttpApplication<TContext> application, Action preProcessRequestAsync)
        {
            _application = application;
            _preProcessRequestAsync = preProcessRequestAsync;
        }

        public TContext CreateContext(IFeatureCollection contextFeatures)
        {
            return _application.CreateContext(contextFeatures);
        }

        public void DisposeContext(TContext context, Exception exception)
        {
            _application.DisposeContext(context, exception);
        }

        public Task ProcessRequestAsync(TContext context)
        {
            _preProcessRequestAsync();
            return _application.ProcessRequestAsync(context);
        }
    }
}