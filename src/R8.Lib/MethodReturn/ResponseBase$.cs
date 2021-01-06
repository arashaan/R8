using Newtonsoft.Json;

using R8.Lib.Localization;
using R8.Lib.Validatable;

namespace R8.Lib.MethodReturn
{
    /// <summary>
    /// An abstract class for presenting Returning status of a method.
    /// </summary>
    /// <typeparam name="TStatus">Type of status property.</typeparam>
    public abstract class ResponseBase<TStatus> : ResponseStatus<TStatus>, IResponseBase<TStatus>
    {
        [JsonIgnore]
        public virtual ValidatableResultCollection Errors { get; protected set; }

        /// <summary>
        /// An abstract class for presenting Returning status of a method.
        /// </summary>
        public ResponseBase() : base()
        {
        }

        /// <summary>
        /// An abstract class for presenting Returning status of a method.
        /// </summary>
        /// <param name="localizer">A working instance of <see cref="ILocalizer"/>.</param>
        public ResponseBase(ILocalizer localizer) : base(localizer)
        {
        }

        /// <summary>
        /// An abstract class for presenting Returning status of a method.
        /// </summary>
        /// <param name="status">A value that representing status.</param>
        public ResponseBase(TStatus status) : this(status, default)
        {
            Status = status;
        }

        /// <summary>
        /// An abstract class for presenting Returning status of a method.
        /// </summary>
        /// <param name="errors">A collection of <see cref="ValidatableResultCollection"/> that representing errors.</param>
        public ResponseBase(ValidatableResultCollection errors) : this(default, errors)
        {
            Errors = errors;
        }

        /// <summary>
        /// An abstract class for presenting Returning status of a method.
        /// </summary>
        /// <param name="status">A value that representing status.</param>
        /// <param name="errors">A collection of <see cref="ValidatableResultCollection"/> that representing errors.</param>
        public ResponseBase(TStatus status, ValidatableResultCollection errors) : base(status)
        {
            Errors = errors;
        }

        public void AddErrors(ValidatableResultCollection errors)
        {
            Errors ??= new ValidatableResultCollection();
            Errors.AddRange(errors);
        }
    }
}