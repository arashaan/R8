using R8.Lib.MethodReturn;

namespace R8.EntityFrameworkCore
{
    /// <summary>
    /// A base interface for <see cref="ResponseBaseDatabase{TStatus}"/>.
    /// </summary>
    public interface IResponseBaseDatabase
    {
        /// <summary>
        /// Gets or sets state of saving changes into database.
        /// </summary>
        DatabaseSaveState? Save { get; set; }
    }
}