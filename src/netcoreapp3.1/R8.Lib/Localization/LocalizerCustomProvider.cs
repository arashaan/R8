using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace R8.Lib.Localization
{
    /// <summary>
    /// A provider to being used as custom provider in type of <see cref="ILocalizerProvider"/>.
    /// </summary>
    public class LocalizerCustomProvider : ILocalizerProvider, ILocalizerRefresher
    {
        /// <summary>
        /// A provider to being used as custom provider in type of <see cref="ILocalizerProvider"/>.
        /// </summary>
        public LocalizerCustomProvider()
        {
        }

        /// <summary>
        /// Gets or sets a <see cref="Func{TResult}"/> to being used as synchronous operation.
        /// </summary>
        public virtual Func<Dictionary<string, LocalizerContainer>> Dictionary { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Func{TResult}"/> to being used as asynchronous operation.
        /// </summary>
        public virtual Func<Task<Dictionary<string, LocalizerContainer>>> DictionaryAsync { get; set; }

        public Dictionary<string, LocalizerContainer> Refresh(IEnumerable<CultureInfo> cultures)
        {
            if (Dictionary == null)
                throw new NullReferenceException($"{nameof(Dictionary)} is null.");

            return Dictionary.Invoke();
        }

        public async Task<Dictionary<string, LocalizerContainer>> RefreshAsync(IEnumerable<CultureInfo> cultures)
        {
            if (DictionaryAsync == null)
                throw new NullReferenceException($"{nameof(DictionaryAsync)} is null.");

            return await DictionaryAsync.Invoke().ConfigureAwait(false);
        }
    }
}