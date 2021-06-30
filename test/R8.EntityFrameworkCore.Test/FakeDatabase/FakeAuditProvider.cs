using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Memory;

using R8.EntityFrameworkCore.Audits;

using System.Globalization;
using System.Threading.Tasks;

namespace R8.EntityFrameworkCore.Test.FakeDatabase
{
    public class FakeAuditProvider : IAuditDataProvider
    {
        private readonly IMemoryCache _memoryCache;

        public FakeAuditProvider(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task<object> OnActionAsync(EntityState state, EntityEntry entry)
        {
            return Task.FromResult((object)new FakeAuditAdditional
            {
                Culture = CultureInfo.CurrentCulture,
                Text = _memoryCache.GetType().Name
            });
        }
    }
}