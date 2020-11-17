using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace R8.Lib.Localization
{
    /// <summary>
    /// Initializes an instance of <see cref="ILocalizerProvider"/>.
    /// </summary>
    public class LocalizerCustomProvider : ILocalizerProvider
    {
        public Expression<Func<Dictionary<string, LocalizerContainer>>> Dictionary { get; set; }
        public Expression<Func<Task<Dictionary<string, LocalizerContainer>>>> DictionaryAsync { get; set; }
        public List<CultureInfo> SupportedCultures { get; set; }
        public CultureInfo DefaultCulture { get; set; }
    }
}