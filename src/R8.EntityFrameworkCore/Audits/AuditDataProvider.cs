using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System.Threading.Tasks;

namespace R8.EntityFrameworkCore.Audits
{
    /// <summary>
    /// A default instance of <see cref="IAuditDataProvider"/>.
    /// </summary>
    public class AuditDataProvider : IAuditDataProvider
    {
        public virtual Task<object> OnActionAsync(EntityState state, EntityEntry entry)
        {
            return Task.FromResult((object)null);
        }
    }
}