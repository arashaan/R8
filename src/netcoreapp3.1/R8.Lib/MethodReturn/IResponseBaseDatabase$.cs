using R8.Lib.Enums;

namespace R8.Lib.MethodReturn
{
    /// <summary>
    /// A base interface for <see cref="ResponseBaseDatabase{TStatus}"/>.
    /// </summary>
    /// <typeparam name="TStatus">A type that representing status type.</typeparam>
    public interface IResponseBaseDatabase<TStatus> : IResponseBase<TStatus>
    {
        /// <summary>
        /// Gets or sets state of saving changes into database.
        /// </summary>
        DatabaseSaveState? Save { get; set; }
    }
}