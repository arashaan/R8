using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R8.Lib.Localization
{
    /// <summary>
    /// An <see cref="ILocalizerProvider"/> interface.
    /// </summary>
    public interface ILocalizerProvider : ILocalizerCultureProvider
    {
        void Refresh(IServiceProvider serviceProvider, Dictionary<string, LocalizerContainer> dictionary);

        Task RefreshAsync(IServiceProvider serviceProvider, Dictionary<string, LocalizerContainer> dictionary);
    }
}