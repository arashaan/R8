namespace R8.EntityFrameworkCore
{
    /// <summary>
    /// An enumerator constant that represents state of audit.
    /// </summary>
    public enum AuditFlags
    {
        /// <summary>
        /// This will be used when Entity is newly created.
        /// </summary>
        Created = 0,

        /// <summary>
        /// This will be used when Entity is changed.
        /// </summary>
        Changed = 1,

        /// <summary>
        /// This will be used when Entity is deleted.
        /// </summary>
        Deleted = 2,

        /// <summary>
        /// This will be used when Entity is undeleted.
        /// </summary>
        Undeleted = 3
    }
}