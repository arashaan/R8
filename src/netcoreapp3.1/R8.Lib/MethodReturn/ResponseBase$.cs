using Newtonsoft.Json;

using R8.Lib.Localization;

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

        public ResponseBase() : base()
        {
        }

        public ResponseBase(ILocalizer localizer) : base(localizer)
        {
        }

        public ResponseBase(TStatus status) : this(status, default)
        {
            Status = status;
        }

        public ResponseBase(ValidatableResultCollection errors) : this(default, errors)
        {
            Errors = errors;
        }

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