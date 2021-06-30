using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System.Threading.Tasks;

namespace R8.EntityFrameworkCore.Audits
{
    /// <summary>
    /// An <see cref="IAuditDataProvider"/> interface to let <see cref="AuditProviderInterceptor"/> knows what to do on specified actions.
    /// </summary>
    public interface IAuditDataProvider
    {
        /// <summary>
        /// Adds user-defined information when any event triggered.
        /// </summary>
        /// <returns></returns>
        Task<object> OnActionAsync(EntityState state, EntityEntry entry);
    }
}