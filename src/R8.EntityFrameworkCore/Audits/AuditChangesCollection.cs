using System;
using System.Collections.Generic;
using System.Linq;

namespace R8.EntityFrameworkCore.Audits
{
    /// <summary>
    /// A collection of <see cref="AuditChange"/>.
    /// </summary>
    public class AuditChangesCollection : List<AuditChange>
    {
        /// <summary>
        /// A collection of <see cref="AuditChange"/>.
        /// </summary>
        public AuditChangesCollection()
        {
        }

        /// <summary>
        /// A collection of <see cref="AuditChange"/>.
        /// </summary>
        public AuditChangesCollection(IEnumerable<AuditChange> collection) : base(collection)
        {
        }

        /// <summary>
        /// Returns an <see cref="AuditChange"/> object that related to given property name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="AuditChange"/> object.</returns>
        public AuditChange this[string propertyName]
        {
            get
            {
                if (propertyName == null)
                    throw new ArgumentNullException(nameof(propertyName));

                return this.FirstOrDefault(x => x.Key.Equals(propertyName));
            }
        }

        /// <summary>
        /// Determines whether an <see cref="AuditChange"/> is in <see cref="AuditChangesCollection"/>.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool ContainsKey(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            return this.Any(x => x.Key.Equals(propertyName));
        }
    }
}