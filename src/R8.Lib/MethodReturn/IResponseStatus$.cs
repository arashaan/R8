using R8.Lib.Localization;

namespace R8.Lib.MethodReturn
{
    /// <summary>
    /// A base interface for <see cref="ResponseStatus{TStatus}"/>.
    /// </summary>
    /// <typeparam name="TStatus">A type that representing status type.</typeparam>
    public interface IResponseStatus<TStatus>
    {
        /// <summary>
        /// Gets status of procedure.
        /// </summary>
        TStatus Status { get; }

        /// <summary>
        /// Gets succession of process.
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// Adds a dependency of <see cref="ILocalizer"/> to object.
        /// </summary>
        /// <param name="localizer">A working <see cref="ILocalizer"/> object.</param>
        /// <remarks>No need to use this method if you've already added <see cref="ILocalizer"/> to dependencies.</remarks>
        void SetLocalizer(ILocalizer localizer);

        /// <summary>
        /// Returns an instance of <see cref="ILocalizer"/>.
        /// </summary>
        /// <returns>A <see cref="ILocalizer"/> object.</returns>
        ILocalizer GetLocalizer();

        /// <summary>
        /// Sets status of procedure.
        /// </summary>
        /// <param name="status"></param>
        void SetStatus(TStatus status);
    }
}