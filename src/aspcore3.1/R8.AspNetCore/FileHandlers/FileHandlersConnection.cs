using Microsoft.Extensions.DependencyInjection;

using System;

namespace R8.AspNetCore.FileHandlers
{
    internal static class FileHandlersConnection
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

        public static FileHandlerConfiguration Options => _services.GetService<FileHandlerConfiguration>();
    }
}