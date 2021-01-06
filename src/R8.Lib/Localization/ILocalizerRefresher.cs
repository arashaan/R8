using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace R8.Lib.Localization
{
    internal interface ILocalizerRefresher
    {
        /// <summary>
        /// Refreshes internal dictionary in order to update to last changes.
        /// </summary>
        /// <param name="cultures"></param>
        Dictionary<string, LocalizerContainer> Refresh(IEnumerable<CultureInfo> cultures);

        /// <summary>
        /// Refreshes internal dictionary in order to update to last changes.
        /// </summary>
        /// <param name="cultures"></param>
        /// <returns>A <see cref="Task{TResult}"/> object that representing an asynchronous operation.</returns>
        Task<Dictionary<string, LocalizerContainer>> RefreshAsync(IEnumerable<CultureInfo> cultures);
    }
}