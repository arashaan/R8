using System;
using Microsoft.Extensions.DependencyInjection;
using R8.Lib.Localization;

namespace R8.Lib.Wrappers
{
    public class WrapperConnection
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

        public static ILocalizer? Options => _services?.GetService<ILocalizer>();
    }
}