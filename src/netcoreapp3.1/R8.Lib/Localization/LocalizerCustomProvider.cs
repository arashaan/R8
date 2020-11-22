using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace R8.Lib.Localization
{
    public class LocalizerCustomProvider : ILocalizerProvider
    {
        public virtual List<CultureInfo> SupportedCultures { get; set; }
        public virtual CultureInfo DefaultCulture { get; set; }
        public virtual Func<IServiceProvider, Dictionary<string, LocalizerContainer>> Expression { get; set; }
        public virtual Func<IServiceProvider, Task<Dictionary<string, LocalizerContainer>>> ExpressionAsync { get; set; }

        public virtual void Refresh(IServiceProvider serviceProvider, Dictionary<string, LocalizerContainer> dictionary)
        {
            var tempDic = Expression.Invoke(serviceProvider);
            if (tempDic == null || !tempDic.Any())
                tempDic = Expression.Invoke(serviceProvider);

            if (tempDic?.Any() != true)
                return;

            dictionary.Clear();
            foreach (var (key, localizerContainer) in tempDic)
                dictionary.Add(key, localizerContainer);
        }

        public virtual async Task RefreshAsync(IServiceProvider serviceProvider, Dictionary<string, LocalizerContainer> dictionary)
        {
            var tempDic = await ExpressionAsync.Invoke(serviceProvider);
            if (tempDic == null || !tempDic.Any())
                tempDic = await ExpressionAsync.Invoke(serviceProvider).ConfigureAwait(false);

            if (tempDic?.Any() == true)
            {
                dictionary.Clear();
                foreach (var (key, localizerContainer) in tempDic)
                    dictionary.Add(key, localizerContainer);
            }
        }
    }
}