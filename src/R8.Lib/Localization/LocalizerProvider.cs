using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace R8.Lib.Localization
{
    /// <summary>
    /// A provider to being used as custom provider in type of <see cref="ILocalizerProvider"/>.
    /// </summary>
    public class LocalizerProvider : ILocalizerProvider, ILocalizerRefresher
    {
        /// <summary>
        /// A provider to being used as custom provider in type of <see cref="ILocalizerProvider"/>.
        /// </summary>
        public LocalizerProvider()
        {
        }

        /// <summary>
        /// Gets or sets a <see cref="Func{TResult}"/> to being used as asynchronous operation.
        /// </summary>
        public virtual Func<Task<Dictionary<string, LocalizerContainer>>> Dictionary { get; set; }

        public virtual async Task<Dictionary<string, LocalizerContainer>> RefreshAsync(IEnumerable<CultureInfo> cultures)
        {
            if (Dictionary == null)
                throw new NullReferenceException($"{nameof(Dictionary)} is null.");

            return await Dictionary.Invoke().ConfigureAwait(false);
        }
    }
}