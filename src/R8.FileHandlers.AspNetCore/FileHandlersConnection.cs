using System;
using Microsoft.Extensions.DependencyInjection;

namespace R8.FileHandlers.AspNetCore
{
    public static class FileHandlersConnection
    {
        private static IServiceProvider _services;

        public static IServiceProvider Services
        {
            set
            {
                if (_services != null)
                    return;

                _services = value;
            }
        }

        public static FileHandlerConfiguration Options => _services.GetService<FileHandlerConfiguration>()
                                                          ?? throw new NullReferenceException(
                                                              "File handlers required service must be registered.");
    }
}