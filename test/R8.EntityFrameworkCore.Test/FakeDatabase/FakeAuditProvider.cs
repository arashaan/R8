using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Memory;

using System.Globalization;
using System.Threading.Tasks;
using R8.EntityFrameworkCore.Audits;

namespace R8.EntityFrameworkCore.Test.FakeDatabase
{
    public class FakeAuditProvider : IAuditDataProvider
    {
        private readonly IMemoryCache _memoryCache;

        public FakeAuditProvider(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<object> OnAddAsync(EntityEntry entry)
        {
            return new FakeAuditAdditional
            {
                Culture = CultureInfo.CurrentCulture,
                Text = _memoryCache.GetType().Name
            };
        }

        public async Task<object> OnRemoveAsync(EntityEntry entry)
        {
            return new FakeAuditAdditional
            {
                Culture = CultureInfo.CurrentCulture
            };
        }

        public async Task<object> OnUpdateAsync(EntityEntry entry)
        {
            return new FakeAuditAdditional
            {
                Culture = CultureInfo.CurrentCulture
            };
        }

        public async Task<object> OnUnRemoveAsync(EntityEntry entry)
        {
            return new FakeAuditAdditional
            {
                Culture = CultureInfo.CurrentCulture
            };
        }
    }
}