namespace R8.Lib.MethodReturn
{
    /// <summary>
    /// A base interface to determine errors.
    /// </summary>
    public interface IResponseErrors
    {
        /// <summary>
        /// Gets a collection of <see cref="ValidatableResult"/> that representing current response errors.
        /// </summary>
        ValidatableResultCollection Errors { get; }
    }
}