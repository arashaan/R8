using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace R8.EntityFrameworkCore.Audits
{
    /// <summary>
    /// An <see cref="IAuditDataProvider"/> interface to let <see cref="AuditProviderInterceptor"/> knows what to do on specified actions.
    /// </summary>
    public interface IAuditDataProvider
    {
        /// <summary>
        /// Adds user-defined information to <see cref="Audit.Additional"/> when using <see cref="DbContext.Add{TEntity}"/>.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        Task<object> OnAddAsync(EntityEntry entry);

        /// <summary>
        /// Adds user-defined information to <see cref="Audit.Additional"/> when using <see cref="DbContext.Remove{TEntity}"/>.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        Task<object> OnRemoveAsync(EntityEntry entry);

        /// <summary>
        /// Adds user-defined information to <see cref="Audit.Additional"/> when using <see cref="DbContext.Update{TEntity}"/>.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        Task<object> OnUpdateAsync(EntityEntry entry);

        /// <summary>
        /// Adds user-defined information to <see cref="Audit.Additional"/> when using <see cref="DbContextBaseExtensions.UnRemove{TDbContext,TSource}"/>.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        Task<object> OnUnRemoveAsync(EntityEntry entry);
    }
}