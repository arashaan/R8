using System;
using System.Diagnostics.CodeAnalysis;

namespace R8.EntityFrameworkCore.Audits
{
    /// <summary>
    /// Configuration of <see cref="AuditProviderInterceptor"/>.
    /// </summary>
    public class AuditProviderConfiguration
    {
        /// <summary>
        /// Configuration of <see cref="AuditProviderInterceptor"/>.
        /// </summary>
        public AuditProviderConfiguration()
        {
        }

        /// <summary>
        /// Type of a audit provider based on <see cref="IAuditDataProvider"/>.
        /// </summary>
        [NotNull]
        public Type ProviderType { get; set; }

        /// <summary>
        /// <para>Checks whether should permanently delete entity on .Remove() or just change <c>Delete</c> column to <c>true</c>.</para>
        /// <para>Also, <see cref="IAuditDataProvider.OnRemoveAsync"/> and <see cref="IAuditDataProvider.OnUnRemoveAsync"/> in <see cref="ProviderType"/> class will remain unreachable.</para>
        /// </summary>
        /// <remarks>default: <c>false</c></remarks>
        public bool PermanentDelete { get; set; } = false;

        /// <summary>
        /// Name of the <c>Id</c> column in entity.
        /// </summary>
        /// <remarks>default: <c>Id</c></remarks>
        [NotNull]
        public string IdColumn { get; set; } = "Id";

        /// <summary>
        /// <para>
        ///     Name of the <c>Delete</c> column in entity.
        ///     This is useful when you set <see cref="PermanentDelete"/> to <c>false</c>.
        /// </para>
        /// <para>This can be left blank, if there is no Delete column in entity.</para>
        /// </summary>
        /// <remarks>default: <c>IsDeleted</c></remarks>
        public string DeleteColumn { get; set; } = "IsDeleted";

        /// <summary>
        /// <para>An array of column those should not been tracked by <see cref="AuditProviderInterceptor"/>.</para>
        /// <para><see cref="IdColumn"/> value value will be added to this array, automatically.</para>
        /// </summary>
        /// <remarks>This is useful for secret columns.</remarks>
        public string[] UntrackableColumns { get; set; }
    }
}