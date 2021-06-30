using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace R8.EntityFrameworkCore.Audits
{
    public sealed class AuditContext
    {
        public EntityEntry Entity { get; set; }
    }
}