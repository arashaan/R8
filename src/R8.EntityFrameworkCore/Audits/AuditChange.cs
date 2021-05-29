using Microsoft.EntityFrameworkCore;

using System;
using System.Threading.Tasks;

namespace R8.EntityFrameworkCore.Audits
{
    public class AuditChange
    {
        public AuditChange()
        {
        }

        public AuditChange(string key, object oldValue, object newValue)
        {
            Key = key;
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Name of the property that had changes.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="string"/> that representing value changed to a new one.
        /// </summary>
        public object OldValue { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="string"/> that representing old value changed to this.
        /// </summary>
        public object NewValue { get; set; }

        internal string NavigationalEntity { get; set; }

        /// <summary>
        /// <para>This methods works when current <see cref="AuditChange"/> contains an foreign-key and navigation entity name.</para>
        /// <para>This is for sometimes you change an foreign-key ID.</para>
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public async Task<object> GetNavigationEntityObjectAsync<TDbContext>(TDbContext dbContext) where TDbContext : DbContext
        {
            if (string.IsNullOrEmpty(NavigationalEntity))
                return null;

            if (!(NewValue is Guid id))
                return null;

            var entityType = dbContext.Model.FindEntityType(NavigationalEntity).ClrType;
            var obj = await dbContext.FindAsync(entityType, id);
            return obj;
        }

        public override string ToString()
        {
            return $"[{Key}] FROM [{OldValue}] TO [{NewValue}]";
        }
    }
}